using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FlaUI.Core.Definitions;
using NUnit.Framework;
using FlaUI.Core.Conditions;
using FlaUI.Core.Tools;
using WeatherAppAutomation.Helpers;

namespace WeatherAppAutomation
{
    //[TestFixture(AutomationType.UIA3)]
    public class BaseTestFixture
    {
        public Window MainWindow { get; private set; }
        protected static Application _weatherApp => SetupFixture.WeatherApp;
        protected static UIA3Automation _automation => SetupFixture.UIAutomation;

        public Window? GetMainWindowByCriteria(string expectedName, string expectedClassName)
        {
            var desktop = _automation.GetDesktop();
            var windows = desktop.FindAllChildren();
            //var windows = _weatherApp.GetAllTopLevelWindows(_automation);

            var main = windows.FirstOrDefault(w => string.Equals(w.Name, expectedName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(w.ClassName, expectedClassName, StringComparison.Ordinal));


            if (main == null)
            {
                TestContext.WriteLine($"No window found with name '{expectedName}' and ClassName '{expectedClassName}'. Available windows:");
                foreach (var w in windows)
                {
                    TestContext.WriteLine($" - {w.Name} (Class: {w.ClassName})");
                }
            }
            else
            {
                TestContext.WriteLine($"Main window found: '{main.Name}' with ClassName '{main.ClassName}'.");
            }

            return main?.AsWindow();
        }

        [SetUp]
        public virtual void Setup()
        {
            //var mainWindow = _weatherApp.GetMainWindow(_automation);
            var retryResult = Retry.WhileNull(
                () => GetMainWindowByCriteria("Weather", "ApplicationFrameWindow"),
                timeout: TimeSpan.FromSeconds(5),
                interval: TimeSpan.FromMilliseconds(500));

            if (!retryResult.Success || retryResult == null)
            {
                Assert.Fail("Main window could not be found.");
            }
            MainWindow = retryResult.Result;
            Assert.IsNotNull(MainWindow, $"MainWindow of {typeof(Window)} could not be found");

            if (MainWindow.Patterns.Window.IsSupported)
            {
                MainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
            }
            //MainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
            MainWindow.WaitUntilClickable(TimeSpan.FromSeconds(10));
        }


        [TearDown]
        public virtual void TearDown()
        {
            if (MainWindow != null) 
            {
                try
                {
                    MainWindow.Capture();
                    MainWindow.Close();
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Exception during teardown: {ex.Message}");
                }
            }
            TestContext.WriteLine($"Test teardown completed from {nameof(BaseTestFixture)}");

        }

    }
}