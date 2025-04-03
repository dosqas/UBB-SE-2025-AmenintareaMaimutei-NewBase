using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;

namespace CourseApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly CourseService courseService;
        private readonly CoinsService coinsService;
        private string searchText = string.Empty;
        private bool isPremium;
        private bool isFree;
        private bool isEnrolled;
        private bool isNotEnrolled;


        public ObservableCollection<Course> Courses { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }

        public int CoinBalance
        {
            get => coinsService.GetUserCoins(0);

        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (value.Length <= 100 && searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged();
                    FilterCourses();
                }
            }
        }

        public bool IsPremium
        {
            get => isPremium;
            set
            {
                if (isPremium != value)
                {
                    isPremium = value;
                    OnPropertyChanged();
                    FilterCourses();
                }
            }
        }

        public bool IsFree
        {
            get => isFree;
            set
            {
                if (isFree != value)
                {
                    isFree = value;
                    OnPropertyChanged();
                    FilterCourses();
                }
            }
        }

        public bool IsEnrolled
        {
            get => isEnrolled;
            set
            {
                if (isEnrolled != value)
                {
                    isEnrolled = value;
                    OnPropertyChanged();
                    FilterCourses();
                }
            }
        }

        public bool IsNotEnrolled
        {
            get => isNotEnrolled;
            set
            {
                if (isNotEnrolled != value)
                {
                    isNotEnrolled = value;
                    OnPropertyChanged();
                    FilterCourses();
                }
            }
        }

        public ICommand ClearFiltersCommand { get; set; }

        public MainViewModel()
        {
            courseService = new CourseService();
            coinsService = new CoinsService();
            coinsService.GetUserCoins(0);
            // Initially load all courses and tags.
            Courses = new ObservableCollection<Course>(courseService.GetCourses());
            var tagList = courseService.GetTags();
            Tags = new ObservableCollection<Tag>(tagList);

            // Subscribe to each tag's property changes to re-filter when selection changes.
            foreach (var tag in Tags)
            {
                tag.PropertyChanged += Tag_PropertyChanged;
            }

            ClearFiltersCommand = new RelayCommand(ExecuteClearFilters);
        }

        public bool CheckUserDailyLogin()
        {
            bool ok = coinsService.checkUserDailyLogin();
            OnPropertyChanged(nameof(CoinBalance));
            return ok;
        }

        private void Tag_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tag.IsSelected))
            {
                FilterCourses();
            }
        }

        private void ExecuteClearFilters(object? parameter)
        {
            SearchText = string.Empty;
            IsPremium = false;
            IsFree = false;
            IsEnrolled = false;
            IsNotEnrolled = false;
            foreach (var tag in Tags)
            {
                tag.IsSelected = false;
            }
            FilterCourses();
        }

        /// <summary>
        /// Applies all active filters and updates the Courses collection.
        /// </summary>
        private void FilterCourses()
        {
            // Get list of selected tag IDs.
            var selectedTagIds = Tags.Where(t => t.IsSelected).Select(t => t.TagId).ToList();

            var filteredCourses = courseService.GetFilteredCourses(SearchText, IsPremium, IsFree, IsEnrolled, IsNotEnrolled, selectedTagIds);

            // Update the Courses collection.
            Courses.Clear();
            foreach (var course in filteredCourses)
            {
                Courses.Add(course);
            }
        }
    }
}
