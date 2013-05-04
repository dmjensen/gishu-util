using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TestRunnerConfigTests
{
    [TestFixture]
    public class TestTestRunnerConfiguration : TestRunnerConfigurationBase
    {
        [Test]
        public void DefaultsToNUnitConsole()
        {
            Assert.That(_configurationVM.SelectedRunnerType, Is.EqualTo(RunnerType.NUnitConsole));
        }

        [TestCase(RunnerType.NUnitResultsFileWatcher, "Enter test results folder : ")]
        [TestCase(RunnerType.NUnitConsole, "Enter console command line to run tests : ")]
        public void ShowsAppropriateConfigDescriptionBasedOnRunnerType(RunnerType type, string expectedDescription)
        {
            _configurationVM.SelectedRunnerType = type;

            Assert.That(_configurationVM.ConfigArgumentsDescription, Is.EqualTo(expectedDescription),
                            string.Format("Wrong Config Description for {0}", type));
        }

        [Test]
        public void NotifiesChangeInConfigArguments()
        {
            ConfigureRunner(RunnerType.NUnitResultsFileWatcher, "Some folder path");
            var changeListener = new PropertyChangeListener(_configurationVM);

            _configurationVM.ConfigArguments = string.Empty;

            Assert.That(changeListener.HasReceivedChangeNotificationFor("ConfigArguments"), 
                        "should have received prop change notification");
        }

        [Test]
        public void NotifiesChangeInConfigDescription_WhenSelectedRunnerTypeChanges()
        {
            var changeListener = new PropertyChangeListener(_configurationVM);

            _configurationVM.SelectedRunnerType = RunnerType.NUnitResultsFileWatcher;
            _configurationVM.SelectedRunnerType = RunnerType.NUnitConsole;
            Assert.That(changeListener.HasReceivedChangeNotificationFor("ConfigArgumentsDescription"),
                            "should change label for config arguments based on runner type");
        }

        [TestCase(RunnerType.NUnitResultsFileWatcher, "SomeFolderPath", RunnerType.NUnitConsole, "", 
            TestName = "Gui -> Console")]
        [TestCase(RunnerType.NUnitConsole, "shell command", RunnerType.NUnitResultsFileWatcher, "", 
            TestName = "Console -> Gui")]
        [TestCase(RunnerType.NUnitConsole, "shell command", RunnerType.NUnitConsole, "shell command", 
            TestName = "Console -> Console")]
        [TestCase(RunnerType.NUnitResultsFileWatcher, "SomeFolderPath", RunnerType.NUnitResultsFileWatcher, "SomeFolderPath", 
            TestName = "Gui -> Gui")]
        public void ResetsConfiguration_WhenSelectedRunnerTypeChanges(RunnerType existingRunnerType, string existingConfig, RunnerType newRunnerType, string expectedConfig)
        {
            SetupMock.TestRunnerFactoryToCreateRunnerFor(existingRunnerType, existingConfig, _mockRunnerFactory);
            ConfigureRunner(existingRunnerType, existingConfig);

            _configurationVM.SelectedRunnerType = newRunnerType;
            Assert.That(_configurationVM.ConfigArguments, Is.EqualTo(expectedConfig), "should clear configuration when runner type is changed");

            


        }
    }


}