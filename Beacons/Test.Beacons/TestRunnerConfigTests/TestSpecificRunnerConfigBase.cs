using System;
using System.Collections.Generic;
using System.Linq;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TestRunnerConfigTests
{
    public abstract class TestSpecificRunnerConfigBase : TestRunnerConfigurationBase
    {
        private RunnerType _runnerType;
        private string _arguments;
        private string _diffArguments;

        [SetUp]
        public void LoadRunnerSpecificConfiguration()
        {
            _runnerType = GetRunnerType();
            _arguments = GetConfigArguments();
            _diffArguments = GetAnotherSetOfConfigArguments();
        }

        [Test]
        public void ConfiguresTestRunner()
        {
            var testRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(
                _runnerType, _arguments,
                _mockRunnerFactory);
            TestRunner runner = null;

            _testRunnerProvider.TestRunnerCreated += (sender, args) => runner = args.UnitTestRunner;
            ConfigureRunner(_runnerType, _arguments);

            Assert.That(runner, Is.SameAs(testRunner.Object), "provider should now be exposing a valid test runner");
        }

        [Test]
        public void ConfiguringAnotherRunnerDisposesExistingOne()
        {
            var testRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(
                                            _runnerType, _arguments,
                                            _mockRunnerFactory);
            SetupAdditionalExpectations(_arguments);

            ConfigureRunner(_runnerType, _arguments);

            SetupMock.TestRunnerFactoryToCreateRunnerFor(
                                            RunnerType.NUnitResultsFileWatcher, _diffArguments,
                                            _mockRunnerFactory);
            SetupAdditionalExpectations(_diffArguments);
            
            ConfigureRunner(_runnerType, _diffArguments);

            testRunner.Verify(runner => runner.Dispose(), Times.Once(), 
                                "existing runner should have been disposed before switching to the new one");
        }

        [TestCase(null, TestName = "Null file path should be rejected")]
        [TestCase("   ", TestName = "Blank file path should be rejected")]
        public void IgnoresNullOrEmptyFilePaths(string arguments)
        {
            
            ConfigureRunner(_runnerType, arguments);

            Assert.That(_configurationVM["ConfigArguments"], Is.EqualTo(GetValidationErrorIfRunnerIsNotConfigured()),
                "prompt user to enter proper Configuration");

            _mockRunnerFactory.Verify(f => f.CreateTestRunner(It.IsAny<RunnerType>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void DoesNotShowValidationErrorForValidConfiguration()
        {
            SetupMock.TestRunnerFactoryToCreateRunnerFor(_runnerType, _arguments, _mockRunnerFactory);
            SetupAdditionalExpectations(_arguments);

            ConfigureRunner(_runnerType, _arguments);

            Assert.That(_configurationVM.Error, Is.Null, "should not be showing any validation errors");
        }

        [TestCaseSource("GetInvalidArguments")]
        public void ClearsExistingRunnerIfInvalidFilepathIsSet(string arguments)
        {
            SetupMock.TestRunnerFactoryToCreateRunnerFor(_runnerType, _arguments, _mockRunnerFactory);
            SetupAdditionalExpectations(_arguments);
            ConfigureRunner(_runnerType, _arguments);
            
            ConfigureRunner(_runnerType, arguments);

            _mockRunnerFactory.Verify(factory => factory.CreateTestRunner(It.IsAny<RunnerType>(), arguments),
                                      Times.Never());
        }

        [Test]
        public void ConfiguringTestRunnerAddsItToMRUStack()
        {
            var fiveValidConfigurations = GetFiveValidConfigurations();
            _mockRunnerFactory.Setup(f => f.CreateTestRunner(_runnerType, It.IsAny<string>()))
                .Returns(new Mock<TestRunner>().Object);

            _configurationVM.SelectedRunnerType = _runnerType;
            foreach (var path in fiveValidConfigurations)
            {
                _mockFileSystem.Setup(fs => fs.FolderExists(path)).Returns(true);
                _configurationVM.ConfigArguments = path;
            }

            Assert.That(_configurationVM.MRUPaths.Count, Is.EqualTo(5), "MRU list should contain max 5 elements");
            Assert.That(_configurationVM.MRUPaths[0], Is.EqualTo(fiveValidConfigurations.Last()), 
                            "Top of MRU should be recently used path");
            Assert.That(_configurationVM.MRUPaths[4], Is.EqualTo(fiveValidConfigurations.First()), 
                            "End of MRU should be oldest used path");
        }

        [Test]
        public void PromoteFilepathToTopOfMRUStack_IfExistingMRUItemIsUsedAgain()
        {
            _mockRunnerFactory.Setup(f => f.CreateTestRunner(_runnerType, It.IsAny<string>()))
                                .Returns(new Mock<TestRunner>().Object);

            var validConfigurations = GetFiveValidConfigurations();
            var thirdValue = validConfigurations.ElementAt(3);
            var arguments = new List<string>(GetFiveValidConfigurations()) { thirdValue };

            _configurationVM.SelectedRunnerType = _runnerType;
            foreach (var value in arguments)
            {
                SetupAdditionalExpectations(value);
                _configurationVM.ConfigArguments = value;
            }

            Assert.That(_configurationVM.MRUPaths.Count, Is.EqualTo(5), "MRU should contain 5 unique entries");
            Assert.That(_configurationVM.MRUPaths[0], Is.EqualTo(thirdValue), "");
            Assert.That(_configurationVM.MRUPaths.Where(path => path.Equals(thirdValue)).Count(),
                            Is.EqualTo(1), 
                            "Mru must not contain duplicates");
        }

        [Test]
        public void NotifiesChangeInCollectionMRUPaths()
        {
            SetupAdditionalExpectations(_arguments);
            var addedPathsCount = 0;
            _configurationVM.MRUPaths.CollectionChanged += (sender, args) => addedPathsCount = args.NewItems.Count;

            _configurationVM.SelectedRunnerType = _runnerType;
            _configurationVM.ConfigArguments = _arguments;

            Assert.That(addedPathsCount, Is.EqualTo(1), "Should have notified observers of change in MRUPaths");
        }


        protected abstract RunnerType GetRunnerType();
        protected abstract string GetConfigArguments();
        protected abstract string GetAnotherSetOfConfigArguments();

        protected abstract string GetValidationErrorIfRunnerIsNotConfigured();
        protected abstract IEnumerable<string> GetFiveValidConfigurations();

        protected virtual IEnumerable<TestCaseData> GetInvalidArguments()
        {
            yield return new TestCaseData(null).SetName("Null arguments");
            yield return new TestCaseData(null).SetName("Blank arguments");
            
        }

        /// <summary>
        /// Setup any additional expectations that need to be met for successfully configuring
        /// a specific test runner - e.g. the folder name specified in arguments must exist. etc.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual void SetupAdditionalExpectations(string arguments)
        {}
    }
}