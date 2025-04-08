namespace CourseApp.Services
{
    public interface IModuleCompletionService
    {
        void OpenModule(int moduleId);
        bool IsModuleCompleted(int moduleId);
        bool ClickModuleImage(int moduleId);
        bool IsModuleInProgress(int moduleId);
    }
}