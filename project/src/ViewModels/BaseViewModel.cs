using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CourseApp.ViewModels
{
    public class BaseViewModel : IBaseViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        void IBaseViewModel.OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        bool IBaseViewModel.SetProperty<T>(ref T field, T value, string propertyName)
        {
            throw new System.NotImplementedException();
        }
    }
}
