using System.Threading.Channels;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;

namespace MyTelegram.Services.Services;

public class QueuedCommandExecutor<TAggregate, TIdentity, TExecutionResult>(ICommandBus commandBus, ILogger<QueuedCommandExecutor<TAggregate, TIdentity, TExecutionResult>> logger)
    : IQueuedCommandExecutor<TAggregate, TIdentity, TExecutionResult>
    where TAggregate : IAggregateRoot<TIdentity>
    where TIdentity : IIdentity
    where TExecutionResult : IExecutionResult
{
    private readonly Channel<ICommand<TAggregate, TIdentity, TExecutionResult>> _commands = Channel.CreateUnbounded<ICommand<TAggregate, TIdentity, TExecutionResult>>();

    public Task ProcessCommandAsync()
    {
        Task.Run(async () =>
        {
            while (await _commands.Reader.WaitToReadAsync())
            {
                while (_commands.Reader.TryRead(out var command))
                {
                    try
                    {
                        await commandBus.PublishAsync(command, default);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Publish command failed");
                    }
                }
            }
        });

        return Task.CompletedTask;
    }

    public void Enqueue(
        ICommand<TAggregate, TIdentity, TExecutionResult> command)
    {
        _commands.Writer.TryWrite(command);
    }
}