using MyTelegram.Domain.Aggregates.Theme;
using MyTelegram.Domain.Commands.Theme;

namespace MyTelegram.Domain.CommandHandlers.Theme;

public class CreateThemeCommandHandler : CommandHandler<ThemeAggregate, ThemeId, IExecutionResult, CreateThemeCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(ThemeAggregate aggregate,
        CreateThemeCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.RequestInfo,
            command.CreatorUserId,
            command.Title,
            command.Slug,
            command.DocumentId,
            command.Emoticon,
            command.IsDefault,
            command.ForChat);
        return Task.FromResult(ExecutionResult.Success());
    }
}

public class UpdateThemeCommandHandler : CommandHandler<ThemeAggregate, ThemeId, IExecutionResult, UpdateThemeCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(ThemeAggregate aggregate,
        UpdateThemeCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Update(command.RequestInfo,
            command.Title,
            command.Slug,
            command.DocumentId,
            command.Emoticon);
        return Task.FromResult(ExecutionResult.Success());
    }
}
