namespace CourseApp.Services
{
    public interface IRewardService
    {
        bool ClaimCompletionReward(int courseId);
        bool ClaimTimedReward(int courseId, int timeSpent);
    }
}