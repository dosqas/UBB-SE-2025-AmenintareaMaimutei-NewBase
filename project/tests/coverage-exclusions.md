# Coverage Exclusions

This file documents the classes and/or methods that have been intentionally excluded from code coverage.

We are using the `[ExcludeFromCodeCoverage]` attribute to mark classes or methods that are either:

- Not relevant for testing (e.g., purely UI logic with no meaningful backend interaction),
- Not feasible to test in a meaningful way (e.g., tightly coupled to platform-specific behavior),
- Automatically generated or boilerplate code (e.g., designer files).

This helps keep our coverage metrics meaningful and focused on testable, logic-driven code.

## Examples of Excluded Code

- **UI classes or Windows Forms / WPF / XAML-backed code**  
  These are typically excluded due to their dependency on visual components and event-based behavior.

- **App entry points or auto-generated methods**

- **Helpers or wrappers that just forward calls without logic**

All such exclusions are annotated directly in the code with `[ExcludeFromCodeCoverage]`.

Method-level exclusion is supported and used, and it follows the same rationale.

---

## List of Excluded Classes

| Class Name                            | Reason for Exclusion                                                                                   |
|--------------------------------------|--------------------------------------------------------------------------------------------------------|
| `RealDispatcherTimer`                | Wrapper around `DispatcherTimer` from UWP/WinUI which cannot be properly mocked or tested. This class is tightly coupled to the Windows Runtime (WinRT), and leads to COM exceptions during testing. It provides no additional logic beyond forwarding calls. |
| `App`								   | The main entry point for the application. It is auto-generated and contains no logic that can be tested. |
| `MainPage`                           | The main page of the application. It is auto-generated and contains no logic that can be tested. |
| `MainWindow`						   | This is the root window of the application, mostly hosting UI elements and binding to view models. Like other UI components, it is tightly coupled to the visual layer and does not contain logic suitable for unit testing. |
| `CoursePage`						   | This page is tightly coupled to the WinUI framework, handling navigation, UI events, and data binding logic. It does not contain any logic that can be meaningfully unit tested without a UI testing framework. |
| `ModulePage`						   | Similar to CoursePage, ModulePage is responsible for rendering UI and initializing its view model based on navigation parameters. It is heavily dependent on the WinUI framework for page lifecycle and has no logic that is appropriate for unit testing. |
| `DataLink`						   | Provides a static method to create a SqlConnection using a hardcoded connection string. Contains no business logic and is tightly coupled to platform-specific and environment-specific configurations.|
| `CourseModelView`					   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
| `EnrollmentModelView`				   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
| `ModuleModelView`					   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
| `ProgressModelView`				   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
| `RewardModelView`					   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
| `TagModelView`					   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
| `UserWalletModelView` 			   | This class tightly couples SQL logic and database connections, making it unsuitable for unit testing. |
## List of Excluded Methods

> *None listed individually at this time. All exclusions are at the class level.*

---

This file serves as a reference for what was excluded and why, especially during evaluation.
