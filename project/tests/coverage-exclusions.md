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

## List of Excluded Methods

| Method Name                             | Reason for Exclusion                                                                                   |
|-----------------------------------------|--------------------------------------------------------------------------------------------------------|
| `InitializeTimersAndNotificationHelper`  | **Cannot test when timers are null in the constructor.** The method creates new instances of `DispatcherTimerService`, which contains a `RealDispatcherTimer`. Since `RealDispatcherTimer` wraps around a platform-specific `DispatcherTimer`, which we cannot mock due to its reliance on the Windows Runtime (WinRT), it leads to COM exceptions during testing. |

---

This file serves as a reference for what was excluded and why, especially during evaluation.
