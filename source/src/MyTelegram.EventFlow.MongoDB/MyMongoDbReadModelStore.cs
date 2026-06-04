using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.Core.RetryStrategies;
using EventFlow.Extensions;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;
using EventFlow.Exceptions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MyTelegram.EventFlow.MongoDB;

public class MyMongoDbReadModelStore<TReadModel>(
    ILogger<MongoDbReadModelStore<TReadModel>> logger,
    IReadModelDescriptionProvider readModelDescriptionProvider,
    ITransientFaultHandler<IOptimisticConcurrencyRetryStrategy> transientFaultHandler,
    IMongoDbContext mongoDbContext)
    :
        MyMongoDbReadModelStore<TReadModel, IMongoDbContext>(logger, readModelDescriptionProvider,
            transientFaultHandler, mongoDbContext)
    where TReadModel : class, IMongoDbReadModel;

public class MyMongoDbReadModelStore<TReadModel, TDbContext> : IMyMongoDbReadModelStore<TReadModel>
    where TReadModel : class, IMongoDbReadModel
    where TDbContext : IMongoDbContext
{
    private readonly ILogger<MongoDbReadModelStore<TReadModel>> _logger;
    private readonly IReadModelDescriptionProvider _readModelDescriptionProvider;
    private readonly ITransientFaultHandler<IOptimisticConcurrencyRetryStrategy> _transientFaultHandler;
    private readonly TDbContext _dbContext;
    private readonly IMongoDatabase _database;

    public MyMongoDbReadModelStore(
        ILogger<MongoDbReadModelStore<TReadModel>> logger,
        IReadModelDescriptionProvider readModelDescriptionProvider,
        ITransientFaultHandler<IOptimisticConcurrencyRetryStrategy> transientFaultHandler,
        TDbContext dbContext)
    {
        _logger = logger;
        _readModelDescriptionProvider = readModelDescriptionProvider;
        _transientFaultHandler = transientFaultHandler;
        _dbContext = dbContext;
        _database = dbContext.GetDatabase();
    }

    private IMongoDatabase GetDatabase() => _database;

    public async Task UpdateAsync(
        IReadOnlyCollection<ReadModelUpdate> readModelUpdates,
        IReadModelContextFactory readModelContextFactory,
        Func<IReadModelContext, IReadOnlyCollection<IDomainEvent>, ReadModelEnvelope<TReadModel>, CancellationToken, Task<ReadModelUpdateResult<TReadModel>>> updateReadModel,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace(
            "UpdateAsync called with {Count} read model updates for type '{ReadModel}'",
            readModelUpdates.Count,
            typeof(TReadModel).PrettyPrint());

        foreach (var readModelUpdate in readModelUpdates)
        {
            _logger.LogTrace(
                "Processing read model update for '{ReadModelId}' with {EventCount} domain events",
                readModelUpdate.ReadModelId,
                readModelUpdate.DomainEvents.Count);

            // Retry logic with reload - each retry will fetch fresh data from MongoDB
            await _transientFaultHandler.TryAsync(
                async c =>
                {
                    await UpdateReadModelAsync(
                        readModelUpdate.ReadModelId,
                        readModelUpdate,
                        readModelContextFactory,
                        updateReadModel,
                        c);
                    return 0; // Success
                },
                Label.Named("mongodb-read-model-update"),
                cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task UpdateReadModelAsync(
        string readModelId,
        ReadModelUpdate readModelUpdate,
        IReadModelContextFactory readModelContextFactory,
        Func<IReadModelContext, IReadOnlyCollection<IDomainEvent>, ReadModelEnvelope<TReadModel>, CancellationToken, Task<ReadModelUpdateResult<TReadModel>>> updateReadModel,
        CancellationToken cancellationToken)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        try
        {
            // Try to get existing read model - ALWAYS reload from database to get fresh version
            var filter = Builders<TReadModel>.Filter.Eq("_id", readModelId);
            var existingReadModel = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            
            _logger.LogTrace(
                "Loaded read model '{ReadModelId}' from MongoDB - Exists: {Exists}, Version: {Version}",
                readModelId,
                existingReadModel != null,
                existingReadModel?.Version);

            ReadModelEnvelope<TReadModel> readModelEnvelope;
            long? originalVersion = existingReadModel?.Version; // Save original version BEFORE modification
            
            if (existingReadModel == null)
            {
                // Create new read model
                _logger.LogTrace(
                    "Creating new read model '{ReadModelId}' of type '{ReadModel}'",
                    readModelId,
                    typeof(TReadModel).PrettyPrint());

                readModelEnvelope = ReadModelEnvelope<TReadModel>.With(readModelId, default(TReadModel));
            }
            else
            {
                // Update existing read model
                _logger.LogTrace(
                    "Updating existing read model '{ReadModelId}' of type '{ReadModel}' with version '{Version}'",
                    readModelId,
                    typeof(TReadModel).PrettyPrint(),
                    existingReadModel.Version);

                readModelEnvelope = ReadModelEnvelope<TReadModel>.With(readModelId, existingReadModel);
            }

            _logger.LogTrace(
                "Before applying events - ReadModel '{ReadModelId}' envelope Version: '{EnvelopeVersion}', IsNew: {IsNew}",
                readModelId,
                readModelEnvelope.ReadModel?.Version,
                existingReadModel == null);

            var readModelContext = readModelContextFactory.Create(readModelId, isNew: existingReadModel == null);
            var result = await updateReadModel(readModelContext, readModelUpdate.DomainEvents, readModelEnvelope, cancellationToken);

            if (!result.IsModified)
            {
                _logger.LogTrace(
                    "Read model '{ReadModelId}' was not modified",
                    readModelId);
                return;
            }

            _logger.LogTrace(
                "After applying {EventCount} events: ReadModel '{ReadModelId}' new version will be '{NewVersion}', was '{OldVersion}'",
                readModelUpdate.DomainEvents.Count,
                readModelId,
                result.Envelope.ReadModel?.Version,
                readModelEnvelope.ReadModel?.Version);

            // Save the updated read model
            if (existingReadModel == null)
            {
                // Insert new document
                try
                {
                    await collection.InsertOneAsync(result.Envelope.ReadModel, cancellationToken: cancellationToken);
                    
                    _logger.LogTrace(
                        "Inserted new read model '{ReadModelId}' with version '{Version}'",
                        readModelId,
                        result.Envelope.ReadModel.Version);
                }
                catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
                {
                    // Another process inserted it concurrently, throw exception to retry with reload
                    _logger.LogWarning(
                        ex,
                        "Duplicate key error when inserting read model '{ReadModelId}'. Another process created it concurrently. Throwing OptimisticConcurrencyException for retry.",
                        readModelId);

                    // Add a small delay to reduce contention on retry
                    await Task.Delay(50, cancellationToken);

                    throw new OptimisticConcurrencyException($"Read model '{readModelId}' created by another process", ex);
                }
            }
            else
            {
                // Update existing document with optimistic concurrency check
                _logger.LogTrace(
                    "Preparing to update - originalVersion: {OriginalVersion}, existingReadModel.Version (after Apply): {ExistingVersion}, result.Envelope.ReadModel.Version: {ResultVersion}, Are they same object: {SameObject}",
                    originalVersion,
                    existingReadModel.Version,
                    result.Envelope.ReadModel.Version,
                    ReferenceEquals(existingReadModel, result.Envelope.ReadModel));

                // В фильтр берём originalVersion (зафиксированную до вызова Apply), а не existingReadModel.Version, которую Apply уже изменил
                var updateFilter = Builders<TReadModel>.Filter.And(
                    Builders<TReadModel>.Filter.Eq("_id", readModelId),
                    Builders<TReadModel>.Filter.Eq(r => r.Version, originalVersion));

                var replaceResult = await collection.ReplaceOneAsync(
                    updateFilter,
                    result.Envelope.ReadModel,
                    new ReplaceOptions { IsUpsert = false },
                    cancellationToken);

                if (replaceResult.MatchedCount == 0)
                {
                    // Document was modified by another process - reload and retry
                    var currentInDb = await collection.Find(Builders<TReadModel>.Filter.Eq("_id", readModelId)).FirstOrDefaultAsync(cancellationToken);
                    
                    _logger.LogWarning(
                        "Optimistic concurrency conflict for read model '{ReadModelId}'. We loaded version '{LoadedVersion}' and tried to save version '{NewVersion}', but database now has version '{CurrentVersion}'. Throwing OptimisticConcurrencyException to reload and retry. Events count: {EventsCount}",
                        readModelId,
                        originalVersion,  // Use the original version we tried to match
                        result.Envelope.ReadModel.Version,
                        currentInDb?.Version,
                        readModelUpdate.DomainEvents.Count);

                    // Add a small delay to reduce contention on retry
                    await Task.Delay(50, cancellationToken);

                    throw new OptimisticConcurrencyException($"Read model '{readModelId}' updated by another process");
                }

                _logger.LogTrace(
                    "Updated read model '{ReadModelId}' from version '{OldVersion}' to '{NewVersion}'",
                    readModelId,
                    originalVersion,
                    result.Envelope.ReadModel.Version);
            }
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            _logger.LogWarning(
                ex,
                "Duplicate key error for read model '{ReadModelId}'. Another process likely modified it concurrently. Throwing OptimisticConcurrencyException for retry.",
                readModelId);

            throw new OptimisticConcurrencyException($"Read model '{readModelId}' updated by another process", ex);
        }
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);
        
        var filter = Builders<TReadModel>.Filter.Eq("_id", id);
        return collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<ReadModelEnvelope<TReadModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);
        
        var filter = Builders<TReadModel>.Filter.Eq("_id", id);
        var readModel = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        
        return readModel == null 
            ? ReadModelEnvelope<TReadModel>.Empty(id) 
            : ReadModelEnvelope<TReadModel>.With(id, readModel);
    }

    public Task<IAggregateFluent<TResult>> AggregateAsync<TResult, TKey>(
        Expression<Func<TReadModel, bool>> filter,
        Expression<Func<TReadModel, TKey>> id,
        Expression<Func<IGrouping<TKey, TReadModel>, TResult>> group,
        AggregateOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        return Task.FromResult(collection.Aggregate()
                .Match(filter)
                .Group(id, group))
            ;
    }

    public async Task<IAsyncCursor<TResult>> FindAsync<TResult>(Expression<Func<TReadModel, bool>> filter, FindOptions<TReadModel, TResult>? options = null, CancellationToken cancellationToken = default)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        _logger.LogTrace(
            "Finding read model '{ReadModel}' with expression '{Filter}' from collection '{RootCollectionName}'",
            typeof(TReadModel).PrettyPrint(),
            filter,
            readModelDescription.RootCollectionName);

        return await collection.FindAsync(filter, options, cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<TReadModel, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        return collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<IAsyncCursor<TReadModel>> FindAsync(
        Expression<Func<TReadModel, bool>> filter,
        FindOptions<TReadModel, TReadModel>? options = null,
        CancellationToken cancellationToken = default)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        _logger.LogTrace(
            "Finding read model '{ReadModel}' with expression '{Filter}' from collection '{RootCollectionName}'",
            typeof(TReadModel).PrettyPrint(),
            filter,
            readModelDescription.RootCollectionName);

        return await collection.FindAsync(filter, options, cancellationToken);
    }

    public IQueryable<TReadModel> AsQueryable()
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        return collection.AsQueryable();
    }

    public Task DeleteAllAsync(CancellationToken cancellationToken)
    {
        var readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TReadModel>();
        var collection = GetDatabase().GetCollection<TReadModel>(readModelDescription.RootCollectionName.Value);

        _logger.LogWarning(
            "Deleting all documents from collection '{CollectionName}'",
            readModelDescription.RootCollectionName);

        return collection.DeleteManyAsync(Builders<TReadModel>.Filter.Empty, cancellationToken);
    }
}
