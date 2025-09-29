using System;
using NUnit.Framework;
using FlaUI.Core;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using FlaUI.Core.AutomationElements;

namespace WeatherAppAutomation
{
	[SetUpFixture]
	public abstract class SetupFixture
	{
        public Application WeatherApp { get; private set; }
        public UIA3Automation automation { get; private set; }
		public Window MainWindow { get; private set; }

        [OneTimeSetUp]
        //Code to run before any tests in the assembly
        public virtual void OneTimeSetup()
		{
			
			//bool isStoreApp = true;
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = @"weather://",
				UseShellExecute = true
			};

			WeatherApp = Application.Launch(processStartInfo);
			Assert.That(WeatherApp.Equals(null), Is.False, "Failed to launch Weather app.");
            //WeatherApp.IsStoreApp = isStoreApp;
            automation = new UIA3Automation();
			TestContext.WriteLine("Global setup for tests.");
        }

		[OneTimeTearDown]
		public virtual void OneTimeTeardown()
		{
			automation.Dispose();
			if (!WeatherApp.HasExited)
			{
				WeatherApp.Close();
            }
			// Code to run after all tests in the assembly
			TestContext.WriteLine("Global teardown for tests.");
		}
    }
}
