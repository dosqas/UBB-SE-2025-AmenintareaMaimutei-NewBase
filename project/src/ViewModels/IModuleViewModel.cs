using System.Windows.Input;
using CourseApp.Models;

public interface IModuleViewModel : IBaseViewModel
{
    Module CurrentModule { get; }
    bool IsCompleted { get; }
    ICommand CompleteModuleCommand { get; }
    ICommand ModuleImageClickCommand { get; set; }
    string TimeSpent { get; }
    int CoinBalance { get; }
    void HandleModuleImageClick(object? obj);
    void ExecuteModuleImageClick(object? obj);
}