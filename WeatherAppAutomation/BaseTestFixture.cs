using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FlaUI.Core.Definitions;
using NUnit.Framework;
using FlaUI.Core.Conditions;

namespace WeatherAppAutomation
{
    [TestFixture(AutomationType.UIA3)]
    public class BaseTestFixture : SetupFixture
    {
        public Window MainWindow { get; private set; }
        public Application WeatherApp => base.WeatherApp;

        [SetUp]
        public virtual void Setup()
        {
            MainWindow = WeatherApp.GetMainWindow(automation);

        }


        [TearDown]
        public virtual void TearDown()
        {

            MainWindow.Close();

        }

    }
}