using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace WeatherAppAutomation;

[TestFixture]
[NonParallelizable]
public class MenuTests : BaseTestFixture
{
    public AutomationElement _mainWindow => base.MainWindow;

    public AutomationElement SideNavBar =>
        _mainWindow.FindFirstDescendant(cf =>
            cf.ByName("Account")
                .And(cf.ByControlType(ControlType.Window))
                .And(cf.ByClassName("ApplicationFrameWindow"))
        );

    public AutomationElement AccountWindow =>
        _mainWindow.FindFirstDescendant(cf =>
            cf.ByAutomationId("AccountSettingsPane")
                .And(cf.ByControlType(ControlType.Window))
        );

    [SetUp]
    public override void Setup()
    {
        base.Setup();
    }

    [Test]
    public void TestMenuOptions()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var appHeader = _mainWindow.FindFirstDescendant(cf.ByAutomationId("ChromeHeader"));
        var burgerMenu = appHeader?.FindFirstDescendant(cf.ByAutomationId("Expand Navigation"))?.AsButton();
        burgerMenu?.Click();
        var appBar = MainWindow.FindFirstDescendant(cf.ByName("Forecast").And(cf.ByControlType(ControlType.AppBar)));
        Assert.That(appBar?.IsOffscreen, Is.EqualTo(false));
        appBar?.Focus();
        var sideNavBar = SideNavBar;


        List<string> ExpectedListItemTitles = new List<string>
        {
            "Forecast - Not Selected",
            "Maps - Not Selected",
            "Hourly Forecast - Not Selected",
            "Monthly Forecast - Not Selected",
            "Pollen - Not Selected",
            "Life - Not Selected",
            "Historical Weather - Not Selected",
            "Send Feedback - Not Selected"
        };
        List<string> ActualListItemTitles = new List<string>();

        var navBtnList = sideNavBar.FindFirstChild(cf.ByAutomationId("SideNavigationBar"));
        var navListChildren = sideNavBar.FindAllChildren(cf.ByControlType(ControlType.ListItem));

        foreach (var childListItem in navListChildren)
        {
            childListItem.Focus();
            childListItem.Click();
            ActualListItemTitles.Add(childListItem.Name);
            TestContext.WriteLine($"AppBar Button: {childListItem.Name}");
        }
        CollectionAssert.AreEquivalent(ExpectedListItemTitles, ActualListItemTitles, "AppBar button titles do not match expected values");
        var burgerMenuClose = MainWindow.FindFirstDescendant(cf.ByAutomationId("NavBurgerButton"))?.AsButton();
        burgerMenuClose?.Click();
        Assert.Pass();
    }

    [Test]
    public void TestHomeBadges()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var appHeader = _mainWindow.FindFirstDescendant(cf.ByAutomationId("ChromeHeader"));
        appHeader?.Focus();
        var sideNavBar = SideNavBar;

        var signOutBtn = SideNavBar.FindFirstDescendant(cf.ByAutomationId("SignOutButton").And(cf.ByControlType(ControlType.Button)));
        signOutBtn?.Focus();
        var signOutBtnText = signOutBtn?.FindFirstDescendant(cf.ByAutomationId("SignOutText").And(cf.ByClassName("TextBlock")));
        Assert.That(signOutBtnText.IsOffscreen, Is.EqualTo(true), "Sign out button text is visible, side pane should not be expanded");
        signOutBtn?.Click();

        Assert.Pass();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
    }
}
