using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System.Windows.Documents;

namespace WeatherAppAutomation;

public class SystemTests : BaseTestFixture
{
    public AutomationElement _mainWindow => base.MainWindow;

    public AutomationElement? TitleBar => 
        _mainWindow.FindFirstDescendant(cf =>
             cf.ByAutomationId("TitleBar")
             .And(cf.ByControlType(ControlType.Window))
        );

    [SetUp]
    public override void Setup()
    {
        base.Setup();
    }

    [Test]
    public void TestMinimise()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var titleBar = TitleBar!;
        Assert.IsNotNull(titleBar, "TitleBar of window not found");
        var btnMinimize = titleBar.FindFirstDescendant(cf.ByAutomationId("Minimize"))?.AsButton();
        btnMinimize?.Click();
        Assert.That(_mainWindow.Properties.IsOffscreen, Is.EqualTo(true), "Main window is not offscreen after minimize");
        var windowPattern = _mainWindow.Patterns.Window.Pattern;
        TestContext.WriteLine("Current visual state of the Weather app window: " + windowPattern.WindowVisualState);
        Thread.Sleep(1000); // Wait for the window to be fully minimized
        Retry.WhileTrue(
            () => windowPattern.WindowVisualState != WindowVisualState.Minimized,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(250)
        );
        TestContext.WriteLine($"Window state minimized. VisualState: {windowPattern.WindowVisualState}");
        //while (windowPattern.WindowVisualState == WindowVisualState.Minimized)
        //{
        //    _mainWindow.WaitUntilClickable(TimeSpan.FromSeconds(5));
        //    _mainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
        //    TestContext.WriteLine("Waiting for window to be maximised...");
        //    Thread.Sleep(500);
        //    break;
        //}
        _mainWindow.Focus();
        windowPattern.SetWindowVisualState(WindowVisualState.Normal);
        Retry.WhileTrue(
            () => windowPattern.WindowVisualState != WindowVisualState.Normal,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(250)
        );
        _mainWindow.SetForeground();
        Assert.That(_mainWindow.Properties.IsOffscreen, Is.EqualTo(false), "Main window is still offscreen after being restored");
        var btnMaximise = titleBar.FindFirstDescendant(cf.ByAutomationId("Maximize")).AsButton();
        btnMaximise?.Click();
        Assert.That(windowPattern.WindowVisualState, Is.EqualTo(WindowVisualState.Maximized), "Window is not maximized after clicking maximize button");
        Assert.Pass();
    }

    [Test]
    public void TestRestore()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var titleBar = TitleBar!;
        Assert.IsNotNull(titleBar, "TitleBar of window not found");
        _mainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
        var btnRestore = titleBar.FindFirstDescendant(cf.ByAutomationId("Maximise"))?.AsButton();
        btnRestore?.Click();
        Assert.That(_mainWindow.Properties.IsOffscreen, Is.EqualTo(false), "Main window is offscreen after restore");
        _mainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        Assert.Pass();
    }

    [Test]
    public void TestClose() 
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var titlebar = TitleBar!;
        Assert.IsNotNull(titlebar, "TitleBar of window not found");
        var btnClose = titlebar.FindFirstDescendant(cf.ByAutomationId("Close"))?.AsButton();
        btnClose?.Click();
        Thread.Sleep(1000); // Wait for the app to close
        Assert.That(_weatherApp.HasExited, Is.EqualTo(true), "Weather app process has not exited after close");
        var newWeatherApp = Application.LaunchStoreApp("Microsoft.BingWeather_8wekyb3d8bbwe!App");
        var windowResult = Retry.WhileNull(
            () => GetMainWindowByCriteria("Weather", "ApplicationFrameWindow"),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(500)).Result;
        Assert.Pass();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
    }
}
