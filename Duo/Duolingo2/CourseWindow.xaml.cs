using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Duo
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
