//Aici e un CourseWindow.xaml.cs ajutator
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duolingo2
{
    public sealed partial class CourseWindow : Window
    {
        public CourseWindow(string courseTitle)
        {
            this.InitializeComponent();
            TitleText.Text = courseTitle;
        }
    }
}
