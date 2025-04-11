using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;
using Windows.System.Threading;

namespace CourseApp.ViewModels
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private const int CurrentUserId = 0;

        private readonly ICourseService courseService;
        private readonly ICoinsService coinsService;

        private string searchQuery = string.Empty;
        private bool filterByPremium;
        private bool filterByFree;
        private bool filterByEnrolled;
        private bool filterByNotEnrolled;

        public ObservableCollection<Course> DisplayedCourses { get; private set; }
        public ObservableCollection<Tag> AvailableTags { get; private set; }

        public int UserCoinBalance => coinsService.GetCoinBalance(CurrentUserId);

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                if (value.Length <= 100 && searchQuery != value)
                {
                    searchQuery = value;
                    OnPropertyChanged();
                    ApplyAllFilters();
                }
            }
        }

        public bool FilterByPremium
        {
            get => filterByPremium;
            set
            {
                if (filterByPremium != value)
                {
                    filterByPremium = value;
                    OnPropertyChanged();
                    ApplyAllFilters();
                }
            }
        }

        public bool FilterByFree
        {
            get => filterByFree;
            set
            {
                if (filterByFree != value)
                {
                    filterByFree = value;
                    OnPropertyChanged();
                    ApplyAllFilters();
                }
            }
        }

        public bool FilterByEnrolled
        {
            get => filterByEnrolled;
            set
            {
                if (filterByEnrolled != value)
                {
                    filterByEnrolled = value;
                    OnPropertyChanged();
                    ApplyAllFilters();
                }
            }
        }

        public bool FilterByNotEnrolled
        {
            get => filterByNotEnrolled;
            set
            {
                if (filterByNotEnrolled != value)
                {
                    filterByNotEnrolled = value;
                    OnPropertyChanged();
                    ApplyAllFilters();
                }
            }
        }

        public ICommand ResetAllFiltersCommand { get; private set; }

        public MainViewModel(ICourseService? courseService = null, ICoinsService? coinsService = null, ICourseService? courseService1 = null)
        {
            this.courseService = new CourseService();
            this.coinsService = new CoinsService();

            DisplayedCourses = new ObservableCollection<Course>(courseService.GetCourses());
            AvailableTags = new ObservableCollection<Tag>(courseService.GetTags());

            foreach (var tag in AvailableTags)
            {
                tag.PropertyChanged += OnTagSelectionChanged;
            }

            ResetAllFiltersCommand = new RelayCommand(ResetAllFilters);

            this.courseService = courseService;
            this.coinsService = coinsService;
        }

        public bool TryDailyLoginReward()
        {
            bool loginRewardGranted = coinsService.ApplyDailyLoginBonus();
            OnPropertyChanged(nameof(UserCoinBalance));
            return loginRewardGranted;
        }

        private void OnTagSelectionChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(Tag.IsSelected))
            {
                ApplyAllFilters();
            }
        }

        private void ResetAllFilters(object? parameter)
        {
            SearchQuery = string.Empty;
            FilterByPremium = false;
            FilterByFree = false;
            FilterByEnrolled = false;
            FilterByNotEnrolled = false;

            foreach (var tag in AvailableTags)
            {
                tag.IsSelected = false;
            }

            ApplyAllFilters();
        }

        private void ApplyAllFilters()
        {
            var selectedTagIds = AvailableTags
                .Where(tag => tag.IsSelected)
                .Select(tag => tag.TagId)
                .ToList();

            var filteredCourses = courseService.GetFilteredCourses(
                searchQuery,
                filterByPremium,
                filterByFree,
                filterByEnrolled,
                filterByNotEnrolled,
                selectedTagIds);

            DisplayedCourses.Clear();
            foreach (var course in filteredCourses)
            {
                DisplayedCourses.Add(course);
            }
        }
    }
}
