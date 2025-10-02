using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using WeatherAppAutomation.Helpers;

namespace WeatherAppAutomation;

[TestFixture]
[NonParallelizable]
public class MenuTests : BaseTestFixture
{
    public AutomationElement _mainWindow => base.MainWindow;

    public AutomationElement? PaneWindow =>
        _mainWindow.FindFirstDescendant(cf =>
            cf.ByAutomationId("PaneRoot")
                .And(cf.ByControlType(ControlType.Window))
                .And(cf.ByClassName("SplitViewPane"))
        );

    public AutomationElement? SideNavBar =>
        PaneWindow?.FindFirstDescendant(cf =>
            cf.ByAutomationId("SideNavigationBar")
                .And(cf.ByControlType(ControlType.Group))
        );

    public AutomationElement? AccountWindow =>
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

        var navBtnList = sideNavBar?.FindFirstChild(cf.ByAutomationId("SideNavigationBar"));
        Assert.That(navBtnList?.IsOffscreen, Is.EqualTo(false), "Navigation button list is offscreen");
        var navListChildren = sideNavBar?.FindAllChildren(cf.ByControlType(ControlType.ListItem));
        

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
    public void TestSignOutButton()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var appHeader = _mainWindow.FindFirstDescendant(cf.ByAutomationId("ChromeHeader"));
        appHeader?.Focus();
        var paneWindow = PaneWindow;
        var sideNavBar = SideNavBar;

        var signOutBtn = SideNavBar?
            .FindFirstDescendant(cf.ByAutomationId("SignOutButton")
            .And(cf.ByControlType(ControlType.Button)));
        signOutBtn?.Focus();
        var signOutBtnText = signOutBtn?.FindFirstDescendant(cf.ByAutomationId("SignOutText").And(cf.ByClassName("TextBlock")));
        Assert.That(signOutBtnText?.IsOffscreen, Is.EqualTo(true), "Sign out button text is visible, side pane should not be expanded");
        signOutBtn?.Click();
        var signOutWindow = MainWindow.FindFirstDescendant(cf.ByName("Account").And(cf.ByClassName("ApplicationFrameWindow")));
        signOutWindow?.Focus();
        var TextHeader = signOutWindow.FindFirstDescendant(cf.ByAutomationId("AccountTitle"));
        Assert.That(TextHeader?.Properties.IsOffscreen, Is.EqualTo(false), "Sign out window header text is not visible");
        Assert.That(TextHeader?.Name, Is.EqualTo("Account you’ve added to this app"), "Sign out window header text does not match expected value");
        var CloseButton = signOutWindow.FindFirstDescendant(cf.ByAutomationId("CloseButton").And(cf.ByControlType(ControlType.Button)));
        CloseButton?.Click();
        Assert.That(signOutWindow.Properties.IsOffscreen, Is.EqualTo(true), "Sign out window is still visible after closing");
        Assert.Pass();
    }

    [Test]
    public void TestSettingsButton()
    {
        var cf = new ConditionFactory(new UIA3PropertyLibrary());
        var appHeader = _mainWindow.FindFirstDescendant(cf.ByAutomationId("ChromeHeader"));
        appHeader?.Focus();
        var sideNavBar = SideNavBar;
        var settingsBtn = SideNavBar?.FindFirstDescendant(cf.ByAutomationId("SettingsButton").And(cf.ByControlType(ControlType.Button)));
        settingsBtn?.Focus();
        var settingsBtnText = settingsBtn?.FindFirstDescendant(cf.ByAutomationId("SettingsText").And(cf.ByClassName("TextBlock")));
        Assert.That(settingsBtnText?.IsOffscreen, Is.EqualTo(true), "Settings button text is visible, side pane should not be expanded");
        settingsBtn?.Click();
        var settingsTabControl = MainWindow.FindFirstChild(cf.ByControlType(ControlType.Tab).And(cf.ByClassName("Pivot")));
        settingsTabControl?.Focus();
        var tabItems = settingsTabControl?.FindAllChildren(cf.ByControlType(ControlType.TabItem)).ToList();
        List<string> tabItemsCollection = new List<string>();
        List<string> expectedTabTitles = new List<string>
        {
            "General",
            "Privacy Statement",
            "Terms of use",
            "Credits",
            "About"
        };

        foreach ( var tabItem in tabItems)
        {
            var tab = tabItem.AsTabItem();
            tab.Select();
            tab.Focus();
            tabItemsCollection.Add(tabItem.Name);
            TestContext.WriteLine($"Setting tab added {tabItem.Name}");
        }
        CollectionAssert.AreEquivalent(expectedTabTitles, tabItemsCollection, "Settings tab titles do not match expected values");

        var settingsWindow = MainWindow.FindFirstDescendant(cf.ByName("Settings").And(cf.ByClassName("ApplicationFrameWindow")));
        settingsWindow?.Focus();
        var TextHeader = settingsWindow?.FindFirstDescendant(cf.ByAutomationId("SettingsTitle"));
        Assert.That(TextHeader?.Properties.IsOffscreen, Is.EqualTo(false), "Settings window header text is not visible");
        Assert.That(TextHeader?.Name, Is.EqualTo("Settings"), "Settings window header text does not match expected value");
        var CloseButton = settingsWindow?.FindFirstDescendant(cf.ByAutomationId("CloseButton").And(cf.ByControlType(ControlType.Button))).AsButton();
        CloseButton?.Click();
        Assert.That(settingsWindow?.Properties.IsOffscreen, Is.EqualTo(true), "Settings window is still visible after closing");
        Assert.Pass();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
        TestContext.WriteLine($"Teardown method run from derived class {nameof(MenuTests)}");
    }
}
