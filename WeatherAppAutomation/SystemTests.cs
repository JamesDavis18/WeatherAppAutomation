using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

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
    public void Test1()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var titleBar = TitleBar!;
        Assert.IsNotNull(titleBar, "TitleBar of window not found");
        var btnMinimize = titleBar.FindFirstDescendant(cf.ByAutomationId("Minimize"))?.AsButton();
        btnMinimize?.Click();
        Assert.That(_mainWindow.Properties.IsOffscreen, Is.EqualTo(true), "Main window is not offscreen after minimize");
        _mainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        Assert.Pass();
    }

    [Test]
    public void TestRestore()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var titleBar = TitleBar!;
        Assert.IsNotNull(titleBar, "TitleBar of window not found");
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
    }
}
