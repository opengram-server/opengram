using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using MyTelegram.Domain.Aggregates.BotCallbackQuery;
using MyTelegram.Domain.Commands.BotCallbackQuery;

namespace MyTelegram.Domain.CommandHandlers.BotCallbackQuery;

/// <summary>
/// Command handler for answering bot callback queries
/// </summary>
public class AnswerBotCallbackQueryCommandHandler : CommandHandler<BotCallbackQueryAggregate, BotCallbackQueryId, IExecutionResult, AnswerBotCallbackQueryCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(
        BotCallbackQueryAggregate aggregate,
        AnswerBotCallbackQueryCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AnswerCallbackQuery(
            command.BotUserId,
            command.QueryId,
            command.Text,
            command.ShowAlert,
            command.Url,
            command.CacheTime);

        return Task.FromResult(ExecutionResult.Success());
    }
}
