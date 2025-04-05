using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CourseApp.Models
{
    public class Tag : INotifyPropertyChanged
    {
        public int TagId { get; set; }
        public string Name { get; set; } = string.Empty;

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
