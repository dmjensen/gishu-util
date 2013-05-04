using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TestRunnerConfigTests
{
    public abstract class TestRunnerConfigurationBase
    {
        protected Mock<TestRunnerFactory> _mockRunnerFactory;
        protected TestRunnerProvider _testRunnerProvider;
        protected Mock<FileSystem> _mockFileSystem;
        protected TestRunnerConfiguration _configurationVM;

        [SetUp]
        public void CreateConfigVM()
        {
            _mockRunnerFactory = new Mock<TestRunnerFactory>();
            _testRunnerProvider = new TestRunnerProvider(_mockRunnerFactory.Object, new Mock<IDE>().Object);
            _mockFileSystem = new Mock<FileSystem>();
            _configurationVM = new TestRunnerConfiguration(_testRunnerProvider, _mockFileSystem.Object,
                                                new MockUiUpdater());
        }

        protected void ConfigureRunner(RunnerType typeOfRunner, string configArguments)
        {
            _configurationVM.SelectedRunnerType = typeOfRunner;
            _configurationVM.ConfigArguments = configArguments;
        }
    }
}