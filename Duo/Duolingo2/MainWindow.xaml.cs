using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using System.Collections.Generic;
using Duolingo2.Models;

namespace Duolingo2
{
    public sealed partial class MainWindow : Window
    {
        private MainWindowModelView _viewModel;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void SetViewModel(MainWindowModelView viewModel)
        {
            _viewModel = viewModel;
            ApplyCurrentFilters();
            LoadCourses();

            CoinAmountText.Text = _viewModel.UserCoins.ToString();
        }

        private void LoadCourses()
        {
            CoursesPanel.Children.Clear();

            foreach (var courseVM in _viewModel.CourseViews)
            {
                var border = new Border
                {
                    Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 242, 233, 228)),
                    CornerRadius = new CornerRadius(40),
                    Padding = new Thickness(25),
                    Margin = new Thickness(0, 10, 0, 10)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Imagine în cerc
                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri($"ms-appx:///{courseVM.Course.ImagePath}")),
                    Stretch = Stretch.UniformToFill
                };

                var ellipse = new Ellipse
                {
                    Width = 70,
                    Height = 70,
                    Fill = imageBrush,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(ellipse, 0);
                grid.Children.Add(ellipse);

                // Continut curs
                var contentStack = new StackPanel { Margin = new Thickness(20, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };

                var title = new TextBlock
                {
                    Text = courseVM.Course.Name,
                    FontSize = 30,
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 28, 27, 58))
                };
                contentStack.Children.Add(title);

                var line = new Rectangle
                {
                    Height = 1,
                    Fill = new SolidColorBrush(Microsoft.UI.Colors.Black),
                    Margin = new Thickness(0, 5, 0, 5)
                };
                contentStack.Children.Add(line);

                var topicsStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
                foreach (var topic in courseVM.Topics.Take(5))
                {
                    topicsStack.Children.Add(new TextBlock
                    {
                        Text = topic.Name,
                        FontSize = 16,
                        Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 74, 78, 105))
                    });
                }
                contentStack.Children.Add(topicsStack);
                Grid.SetColumn(contentStack, 1);
                grid.Children.Add(contentStack);

                // Dot status înscriere
                var dot = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = courseVM.IsEnrolled
                        ? new SolidColorBrush(Microsoft.UI.Colors.DarkGreen)
                        : new SolidColorBrush(Microsoft.UI.Colors.DarkRed),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(dot, 2);
                grid.Children.Add(dot);

                border.Child = grid;

                //Hover effect
                var originalBackground = border.Background;
                border.PointerEntered += (s, e) =>
                {
                    border.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 230, 220, 210));
                };
                border.PointerExited += (s, e) =>
                {
                    border.Background = originalBackground;
                };

                //Deschide fereastra cu titlul cursului
                border.Tapped += (s, e) =>
                {
                    var courseWindow = new CourseWindow(courseVM.Course.Name);
                    courseWindow.Activate();
                };

                CoursesPanel.Children.Add(border);
            }
        }

        private void ApplyCurrentFilters()
        {
            if (_viewModel == null) return;

            var selectedTopics = TopicsPanel.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => new Topic(cb.Content.ToString()))
                .ToList();

            bool enrolled = EnrolledCheckBox.IsChecked == true;
            bool notEnrolled = NotEnrolledCheckBox.IsChecked == true;
            bool free = FreeCheckBox.IsChecked == true;
            bool paid = PaidCheckBox.IsChecked == true;

            _viewModel.ApplyFilters(selectedTopics, enrolled, notEnrolled, free, paid, userId: 1);
            LoadCourses();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.SetSearchKeyword(SearchBox.Text);
            LoadCourses();
        }

        private void EnrollmentChanged(object sender, RoutedEventArgs e)
        {
            if (sender == EnrolledCheckBox && EnrolledCheckBox.IsChecked == true)
                NotEnrolledCheckBox.IsChecked = false;
            else if (sender == NotEnrolledCheckBox && NotEnrolledCheckBox.IsChecked == true)
                EnrolledCheckBox.IsChecked = false;

            ApplyCurrentFilters();
        }

        private void PricingChanged(object sender, RoutedEventArgs e)
        {
            if (sender == FreeCheckBox && FreeCheckBox.IsChecked == true)
                PaidCheckBox.IsChecked = false;
            else if (sender == PaidCheckBox && PaidCheckBox.IsChecked == true)
                FreeCheckBox.IsChecked = false;

            ApplyCurrentFilters();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyCurrentFilters();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";

            foreach (var cb in TopicsPanel.Children.OfType<CheckBox>())
                cb.IsChecked = false;

            EnrolledCheckBox.IsChecked = false;
            NotEnrolledCheckBox.IsChecked = false;
            FreeCheckBox.IsChecked = false;
            PaidCheckBox.IsChecked = false;

            _viewModel.ClearFilters();
            LoadCourses();
        }
    }
}
