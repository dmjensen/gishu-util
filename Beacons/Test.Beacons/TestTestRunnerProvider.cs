// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons
{
    [TestFixture]
    public class TestTestRunnerProvider
    {
        private Mock<TestRunnerFactory> _mockFactory;
        private TestRunnerProvider _provider;

        [SetUp]
        public void BeforeEachTest()
        {
            _mockFactory = new Mock<TestRunnerFactory>();
            _provider = new TestRunnerProvider(_mockFactory.Object, new Mock<IDE>().Object);
        }

        //[Test]
        //public void CreatesTestRunnerOnRequest()
        //{
        //    Assert.That(_provider.UnitTestRunner, Is.Null);
        //    var mockTestRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(
        //                                RunnerType.NUnitResultsFileWatcher, TestConstants.A_TESTRESULTS_FOLDERPATH, 
        //                                _mockFactory);

        //    _provider.ConfigureTestRunnerFor(
        //                RunnerType.NUnitResultsFileWatcher, TestConstants.A_TESTRESULTS_FOLDERPATH);

        //    Assert.That(_provider.UnitTestRunner, Is.SameAs(mockTestRunner.Object), "Test Runner should now be available since reqd. configuration has been set");
        //}

        //[Test]
        //public void DisposesCurrentRunnerOnRequest()
        //{
        //    GivenThat.TestRunnerIsConfigured(_provider, _mockFactory);

        //    _provider.ClearTestRunner();

        //    Assert.That(_provider.UnitTestRunner, Is.Null, "Should have disposed and cleared the existing runner");
        //}

        [ExpectedException(typeof(ArgumentException))]
        [TestCase(null, TestName = "Null results file path")]
        [TestCase("    ", TestName = "Blank results file path")]
        public void ThrowsArgExceptionForNullOrBlankInput(string testResultsFilePath)
        {
            _provider.ConfigureTestRunnerFor(RunnerType.NUnitResultsFileWatcher, testResultsFilePath);
        }

        [Test]
        public void NotifiesObserversWhenTestRunnerIsCreated()
        {
            SetupMock.TestRunnerFactoryToCreateRunnerFor(
                            RunnerType.NUnitResultsFileWatcher,TestConstants.A_TESTRESULTS_FOLDERPATH, _mockFactory);
            var anObserver = new NotificationListener();
            _provider.TestRunnerCreated += anObserver.Handler;

            _provider.ConfigureTestRunnerFor(
                        RunnerType.NUnitResultsFileWatcher, TestConstants.A_TESTRESULTS_FOLDERPATH);

            Assert.That(anObserver.NotificationReceived, "Provider should have notified observers of test runner creation");
        }

        [Test]
        public void NotifiesObserversBeforeExistingTestRunnerIsDisposed()
        {
            var oldRunner = GivenThat.TestRunnerIsConfigured(_provider, _mockFactory);
            var anObserver = new NotificationListener();
            _provider.TestRunnerDisposing += anObserver.Handler;

            _provider.ClearTestRunner();

            Assert.That(anObserver.NotificationReceived, "provider should notify observers before disposing runner so that they can unsubscribe e.g.");
            oldRunner.Verify(runner => runner.Dispose(), Times.Once());
        }


        [Test]
        public void DoesNotNotifyObserversIfThereIsNoExistingRunnerToDispose()
        {
            var anObserver = new NotificationListener();
            _provider.TestRunnerDisposing += anObserver.Handler;

            _provider.ClearTestRunner();

            Assert.That(!anObserver.NotificationReceived, "Should not have notified observers since there is no configured runner to dispose");
        }

        
    }
    [TestFixture]
    public class GivenTestRunnerProviderConfiguredWithTestRunner
    {
        private TestRunnerProvider _provider;
        private Mock<TestRunnerFactory> _mockFactory;
        private Mock<IDE> _ide;
        private Mock<TestRunner> _testRunner;

        [SetUp]
        public void ConfigureTestRunner()
        {
            _mockFactory = new Mock<TestRunnerFactory>();
            _ide = new Mock<IDE>();
            _provider = new TestRunnerProvider(_mockFactory.Object, _ide.Object);

            _testRunner = GivenThat.TestRunnerIsConfigured(_provider, _mockFactory); 
        }
        [Test]
        public void RunsTestsOnEverySuccessfulBuild()
        {
            SimulateA.SuccessfulBuild(_ide);
            
            _testRunner.Verify(runner => runner.RunTests());
        }

        [Test]
        public void DisposeUnsubscribesFromIdeNotifications()
        {
            _provider.Dispose();

            SimulateA.SuccessfulBuild(_ide);
            
            _testRunner.Verify(runner => runner.RunTests(), Times.Never());
        }
    }
}
