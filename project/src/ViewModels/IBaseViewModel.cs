using System.ComponentModel;
using System.Runtime.CompilerServices;

public interface IBaseViewModel : INotifyPropertyChanged
{
    void OnPropertyChanged([CallerMemberName] string propertyName = "");
    bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "");
}