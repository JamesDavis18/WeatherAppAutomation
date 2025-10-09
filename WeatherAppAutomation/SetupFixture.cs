using System;
using System.Threading.Tasks;
using NUnit.Framework;
using FlaUI.Core;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using System.Runtime.Versioning;
using WeatherAppAutomation.Helpers;
//using OperatingSystem = FlaUI.Core.WindowsAPI.OperatingSystem;

namespace WeatherAppAutomation
{
	[SetUpFixture, Timeout(5000)]
	public class SetupFixture
	{
        public static Application WeatherApp { get; private set; }
        public static UIA3Automation UIAutomation { get; private set; }

        //private void WriteLineToTestLog(string message) 
        //{
        //    StreamWriter writer = File.AppendText("C:\\Windows\\Temp\\TestLog.txt");
        //    writer.WriteLine($"{DateTime.Now}: {message}");
        //    writer.Close();
        //}

		public Window[] GetTopLevelWindows() 
		{
            if (WeatherApp == null || UIAutomation == null)
            {
            	return Array.Empty<Window>();
            }

            var windows = WeatherApp.GetAllTopLevelWindows(UIAutomation);

            TestContext.WriteLine($"Found {windows.Length} top-level windows for the Weather app.");
            LogWriter.WriteLineToTestLog($"Found {windows.Length} top-level windows for the Weather app.");
            foreach (var w in windows)
            {
                TestContext.WriteLine($" - Window '{w.Name}', ClassName='{w.ClassName}', IsOffscreen={w.Properties.IsOffscreen}");
                //WriteLineToTestLog($" - Window '{w.Name}', ClassName='{w.ClassName}', IsOffscreen={w.Properties.IsOffscreen}");
            }
            return windows;
        }


        public Window? GetMainWindowByCriteria(string expectedName, string expectedClassName, bool isOffScreen) 
		{
            var desktop = UIAutomation.GetDesktop();
            var windows = desktop.FindAllChildren();

			//var windows = WeatherApp.GetAllTopLevelWindows(UIAutomation);
			var main = windows.FirstOrDefault(w => string.Equals(w.Name, expectedName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(w.ClassName, expectedClassName, StringComparison.OrdinalIgnoreCase) &&
            bool.Equals(w.Properties.IsOffscreen, isOffScreen));


            if (main == null)
			{
                TestContext.WriteLine($"No window found with name '{expectedName}' and ClassName '{expectedClassName}'. Available windows:");
                foreach (var w in windows)
                {
                    TestContext.WriteLine($" - {w.Name} Class: {w.ClassName}");
                    LogWriter.WriteLineToTestLog($" - {w.Name} Class: {w.ClassName}");
                }
            }
            else
            {
                TestContext.WriteLine($"Main window found: '{main.Name}' with ClassName '{main.ClassName}'.");
                LogWriter.WriteLineToTestLog($"Main window found: '{main.Name}' with ClassName '{main.ClassName}'.");
            }

            return main.AsWindow();
        }

        [OneTimeSetUp]
        //Code to run before any tests in the assembly
        public virtual void OneTimeSetUp()
		{
			UIAutomation = new UIA3Automation();
            var desktop = UIAutomation.GetDesktop();
            foreach (var win in desktop.FindAllChildren())
            {
                TestContext.WriteLine($"Desktop Window: '{win.Name}', ClassName='{win.ClassName}', IsOffscreen={win.Properties.IsOffscreen}");
                LogWriter.WriteLineToTestLog($"Desktop Window: '{win.Name}', ClassName='{win.ClassName}', IsOffscreen={win.Properties.IsOffscreen}");
            }   
            //bool isStoreApp = true;
            ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = @"bingweather://",
				UseShellExecute = true
			};
			try
			{
                WeatherApp = Application.LaunchStoreApp("Microsoft.BingWeather_8wekyb3d8bbwe!App");

                if (WeatherApp == null)
                {
                    TestContext.WriteLine($"LaunchStoreApp failed, attempting to launch via ProcessStartInfo");
                    WeatherApp = Application.Launch(processStartInfo);
                }
            }
			catch (Exception ex)
			{
				Assert.Fail($"LaunchStoreApp failed: {ex.Message}");
			}
            TestContext.WriteLine($"LaunchedWeaherAp with ProcessID:{WeatherApp.ProcessId}");
            //WeatherApp?.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(10));
            WeatherApp?.WaitWhileBusy();

            //var retryResult = Retry.WhileNull(
            //	() => WeatherApp.GetAllTopLevelWindows(UIAutomation),

            //	timeout: TimeSpan.FromSeconds(5),
            //	interval: TimeSpan.FromMilliseconds(500));

            //Assert.That(retryResult.Success, Is.True, "Failed to find main window of Weather app.");
            //Thread.Sleep(2000);
            //WeatherApp.GetMainWindow(UIAutomation);
            WeatherApp?.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(10));
            WeatherApp?.WaitWhileBusy();


            var topWindows = GetTopLevelWindows();
            var mainWindow = Retry.WhileNull(
                () => GetMainWindowByCriteria("Weather", "ApplicationFrameWindow", false),
                timeout: TimeSpan.FromSeconds(5),
            	interval: TimeSpan.FromMilliseconds(500)).Result;
            //CollectionAssert.IsNotEmpty(topWindows, "No top-level windows found for Weather app.");
            //Assert.IsNotNull(mainWindow, "Main window could not be found.");

            //         if (!retryResult.Success || retryResult.Result == null)
            //{
            //	Assert.Fail("Main window could not be found.");
            //         }
            TestContext.WriteLine("Global setup for tests.");
        }

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			try
			{
				if (WeatherApp != null && !WeatherApp.HasExited)
				{
					WeatherApp.Close();
                }
                WeatherApp?.Dispose();
				UIAutomation?.Dispose();
            }
			catch (Exception ex)
			{
				TestContext.WriteLine($"Teardown failed: {ex.Message}");
			}
			// Code to run after all tests in the assembly
			TestContext.WriteLine("Ran OneTimeTearDown and closed test instance");
		}
    }
}
