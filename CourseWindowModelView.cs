using System;
using System.Collections.Generic;
using System.Linq;

namespace Duo
{
    public class CourseWindowModelView
    {
        private readonly CourseService courseService;
        private readonly ModuleService moduleService;

        private Course course;
        private List<Module> modules;
        private List<Topic> topics;
        private bool isEnrolled;

        public CourseWindowModelView(Course course, CourseService courseService, ModuleService moduleService)
        {
            this.course = course ?? throw new ArgumentNullException(nameof(course));
            this.courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            this.moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
            LoadCourseData();
        }

        private void LoadCourseData()
        {
            isEnrolled = courseService.IsUserEnrolled(course.CourseId);
            modules = moduleService.GetModulesForCourse(course.CourseId);
            topics = courseService.GetTopicsByCourse(course.CourseId);
        }

        public void DisplayCourseInformation()
        {
            Console.WriteLine($"Course: {course.CourseName}");
            Console.WriteLine($"Description: {course.Description}");
            Console.WriteLine($"Difficulty: {course.DifficultyLevel}");
            Console.WriteLine($"Type: {(course.IsPremium ? "Premium" : "Free")}");
            Console.WriteLine($"Enrolled: {(isEnrolled ? "Yes" : "No")}");
        }

        public void DisplayModules()
        {
            Console.WriteLine("Modules:");
            foreach (var module in modules.OrderBy(m => m.ModuleOrder))
            {
                string bonusText = module.IsBonus ? $"(Bonus - Unlock: {module.BonusUnlockCost} coins)" : "";
                Console.WriteLine($"{module.ModuleOrder}. {module.ModuleName} {bonusText}");
            }
        }

        public void ToggleEnrollment()
        {
            if (isEnrolled)
            {
                courseService.ToggleEnrollment(course.CourseId);
                isEnrolled = false;
                Console.WriteLine("You have been unenrolled.");
            }
            else
            {
                if (course.IsPremium && !courseService.HasUserPurchasedCourse(course.CourseId))
                {
                    Console.WriteLine($"This course requires {course.CoinCost} coins.");
                    return;
                }

                courseService.ToggleEnrollment(course.CourseId);
                isEnrolled = true;
                Console.WriteLine("You are now enrolled.");
            }
        }

        public void HandleSelectedModule(int moduleId)
        {
            if (!isEnrolled)
            {
                Console.WriteLine("You must enroll to access modules.");
                return;
            }

            var selectedModule = modules.FirstOrDefault(m => m.ModuleId == moduleId);
            if (selectedModule == null)
            {
                Console.WriteLine("Module not found.");
                return;
            }

            if (selectedModule.IsBonus && !moduleService.IsBonusModuleUnlocked(moduleId))
            {
                Console.WriteLine($"Bonus module locked. Requires {selectedModule.BonusUnlockCost} coins.");
                return;
            }

            if (!selectedModule.IsBonus && !ArePreviousModulesCompleted(selectedModule.ModuleOrder))
            {
                Console.WriteLine("Please complete previous modules first.");
                return;
            }

            Console.WriteLine($"Opening module: {selectedModule.ModuleName}");
            // Launch ModuleWindowModelView in UI
        }

        private bool ArePreviousModulesCompleted(int currentOrder)
        {
            var previousModules = modules.Where(m => !m.IsBonus && m.ModuleOrder < currentOrder);
            foreach (var module in previousModules)
            {
                string status = moduleService.GetModuleStatus(module.ModuleId);
                if (status != "Completed")
                    return false;
            }
            return true;
        }

        public string GetCourseProgress()
        {
            var progressList = moduleService.GetUserModuleProgress(course.CourseId);//todo exclude bonus modules and send to service
            int completed = progressList.Count(p => p.Status == "Completed" && !p.IsBonus);
            int total = modules.Count(m => !m.IsBonus);
            return $"{completed}/{total} modules completed";
        }

        public List<Topic> GetTopics()
        {
            return topics;
        }

        public bool GetEnrollmentStatus()
        {
            return isEnrolled;
        }

        public void HandleBackToMain()
        {
            Console.WriteLine("Returning to main course list...");
            // In real UI, would navigate to MainWindow
        }
    }
}
