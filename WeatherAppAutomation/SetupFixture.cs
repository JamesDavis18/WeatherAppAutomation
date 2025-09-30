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
//using OperatingSystem = FlaUI.Core.WindowsAPI.OperatingSystem;

namespace WeatherAppAutomation
{
	[SetUpFixture]
	public class SetupFixture
	{
        public static Application WeatherApp { get; private set; }
        public static UIA3Automation UIAutomation { get; private set; }

        [OneTimeSetUp]
        //Code to run before any tests in the assembly
        public virtual void OneTimeSetUp()
		{
			UIAutomation = new UIA3Automation();
			//bool isStoreApp = true;
			//ProcessStartInfo processStartInfo = new ProcessStartInfo
			//{
			//	FileName = @"bingweather://",
			//	UseShellExecute = true
			//};
			try
			{
                WeatherApp = Application.LaunchStoreApp("Microsoft.BingWeather_8wekyb3d8bbwe!App");
            }
			catch (Exception ex)
			{
				TestContext.WriteLine($"LaunchStoreApp failed: {ex.Message}");
				WeatherApp = null;
			}
			//if (WeatherApp == null)
			//{
			//	TestContext.WriteLine($"LaunchStoreApp failed, attempting to launch via ProcessStartInfo");
			//	WeatherApp = Application.Launch(processStartInfo);
			//}
			WeatherApp.WaitWhileMainHandleIsMissing();

			var retryResult = Retry.WhileNull(
				() => WeatherApp.GetMainWindow(UIAutomation), 
				timeout: TimeSpan.FromSeconds(10),
				interval: TimeSpan.FromMilliseconds(500));

			Assert.That(retryResult.Success, Is.True, "Failed to find main window of Weather app.");
            //Thread.Sleep(2000);
            //WeatherApp.IsStoreApp = isStoreApp;

            if (!retryResult.Success || retryResult.Result == null)
			{
				Assert.Fail("Main window could not be found.");
            }
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
