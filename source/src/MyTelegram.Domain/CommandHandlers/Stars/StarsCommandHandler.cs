using MyTelegram.Domain.Aggregates.Stars;
using MyTelegram.Domain.Commands.Stars;

namespace MyTelegram.Domain.CommandHandlers.Stars;

public class StarsCommandHandler :
    ICommandHandler<StarsAggregate, StarsId, IExecutionResult, CreateStarsAccountCommand>,
    ICommandHandler<StarsAggregate, StarsId, IExecutionResult, AddStarsCommand>,
    ICommandHandler<StarsAggregate, StarsId, IExecutionResult, SpendStarsCommand>,
    ICommandHandler<StarsAggregate, StarsId, IExecutionResult, RefundStarsCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(StarsAggregate aggregate, CreateStarsAccountCommand command, CancellationToken cancellationToken)
    {
        aggregate.Create(command.PeerId);
        return Task.FromResult(ExecutionResult.Success());
    }

    public Task<IExecutionResult> ExecuteCommandAsync(StarsAggregate aggregate, AddStarsCommand command, CancellationToken cancellationToken)
    {
        aggregate.AddStars(command.Amount, command.TransactionId, command.Reason);
        return Task.FromResult(ExecutionResult.Success());
    }

    public Task<IExecutionResult> ExecuteCommandAsync(StarsAggregate aggregate, SpendStarsCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[StarsCommandHandler] Executing SpendStarsCommand. Amount: {command.Amount}, TransactionId: {command.TransactionId}");

        // Если агрегат ещё не создан, списывать звёзды нельзя
        if (aggregate.IsNew)
        {
            Console.WriteLine($"[StarsCommandHandler] StarsAggregate is new! Cannot spend stars before account is created.");
            throw new InvalidOperationException("Stars account does not exist. Please create account first.");
        }

        aggregate.SpendStars(command.Amount, command.TransactionId, command.Reason);
        Console.WriteLine($"[StarsCommandHandler] SpendStarsCommand executed successfully");
        return Task.FromResult(ExecutionResult.Success());
    }

    public Task<IExecutionResult> ExecuteCommandAsync(StarsAggregate aggregate, RefundStarsCommand command, CancellationToken cancellationToken)
    {
        aggregate.RefundStars(command.Amount, command.OriginalTransactionId, command.RefundTransactionId);
        return Task.FromResult(ExecutionResult.Success());
    }
}
