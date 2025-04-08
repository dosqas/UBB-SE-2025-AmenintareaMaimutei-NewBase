using CourseApp.Services;
using CourseApp.ViewModels;
using System;

internal class NotificationHelper : IDisposable
{
    private readonly CourseViewModel parentViewModel;
    private readonly ITimerService timer;

    public NotificationHelper(CourseViewModel parentViewModel, ITimerService timerService)
    {
        this.parentViewModel = parentViewModel ?? throw new ArgumentNullException(nameof(parentViewModel));
        this.timer = timerService ?? throw new ArgumentNullException(nameof(timerService));
        this.timer.Tick += OnNotificationTimerTick;
    }

    public void ShowTemporaryNotification(string message)
    {
        parentViewModel.NotificationMessage = message;
        parentViewModel.ShowNotification = true;
        timer.Interval = TimeSpan.FromSeconds(CourseViewModel.NotificationDisplayDurationInSeconds);
        timer.Start();
    }

    private void OnNotificationTimerTick(object sender, EventArgs eventArgs)
    {
        parentViewModel.ShowNotification = false;
        timer.Stop();
    }

    public void Dispose()
    {
        timer.Tick -= OnNotificationTimerTick;
    }
}