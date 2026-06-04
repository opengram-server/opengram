using System.Linq.Expressions;

namespace MyTelegram.EventFlow.ReadStores;

public record SortOptions<TReadModel>(Expression<Func<TReadModel, object>> Sort, SortType SortType);