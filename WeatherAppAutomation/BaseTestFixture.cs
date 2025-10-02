using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FlaUI.Core.Definitions;
using NUnit.Framework;
using FlaUI.Core.Conditions;

namespace WeatherAppAutomation
{
    //[TestFixture(AutomationType.UIA3)]
    public class BaseTestFixture
    {
        public Window MainWindow { get; private set; }
        protected static Application _weatherApp => SetupFixture.WeatherApp;
        protected static UIA3Automation _automation => SetupFixture.UIAutomation;

        [SetUp]
        public virtual void Setup()
        {
            var mainWindow = _weatherApp.GetMainWindow(_automation);
            if (mainWindow == null)
            {
                Assert.Fail("Main window could not be found.");
            }
            MainWindow = mainWindow!;
            Assert.IsNotNull(MainWindow);
            mainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
            //MainWindow.WaitUntilClickable(TimeSpan.FromSeconds(10));
        }


        [TearDown]
        public virtual void TearDown()
        {

            MainWindow.Capture();
            MainWindow.Close();
            TestContext.WriteLine($"Test teardown completed from {nameof(BaseTestFixture)}");

        }

    }
}