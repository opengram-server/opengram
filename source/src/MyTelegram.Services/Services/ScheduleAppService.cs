using Cube.Timer;

namespace MyTelegram.Services.Services;
public interface IScheduleAppService
{
    Task<TimerTaskHandle> ExecuteAsync(Func<Task> func, TimeSpan timeSpan);

    TimerTaskHandle Execute(Action action, TimeSpan delayTimeSpan);
}

public class ScheduleAppService : IScheduleAppService, ISingletonDependency
{
    private readonly HashedWheelTimer _timer = new();

    public Task<TimerTaskHandle> ExecuteAsync(Func<Task> func, TimeSpan timeSpan)
    {
        var result = _timer.AddTask(timeSpan, new TimerTask(func));
        return Task.FromResult(result);
    }

    public TimerTaskHandle Execute(Action action, TimeSpan delayTimeSpan)
    {
        return _timer.AddTask(delayTimeSpan, action);
    }
}

public class TimerTask(Func<Task> func) : ITimerTask
{
    public Task RunAsync()
    {
        return func();
    }
}