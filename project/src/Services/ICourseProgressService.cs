namespace CourseApp.Services
{
    public interface ICourseProgressService
    {
        void RecordTime(int courseId, int seconds);
        int GetTimeSpent(int courseId);
        int GetTimeRemaining(int courseId);
        void SaveProgress(int courseId);
        void ResetProgress(int courseId);
        CourseProgressData GetCourseProgress(int courseId);
    }

    public record CourseProgressData
    {
        public int TimeSpent { get; init; }
        public int TimeRemaining { get; init; }
    }
}