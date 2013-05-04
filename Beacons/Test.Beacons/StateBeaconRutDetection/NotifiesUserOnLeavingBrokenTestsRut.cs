using System;
using ManiaX.Beacons;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconRutDetection
{
    [TestFixture]
    public class NotifiesUserOnLeavingBrokenTestsRut : TestStateBeaconRutDetectionBase
    {
        private readonly Tuple<CodebaseState, int>[] TRANSITIONS_FOR_SUCCESSFUL_BUILD = new[]{
                            Tuple.Create( CodebaseState.Red, 15),
                            Tuple.Create( CodebaseState.Compiling, 10),
                            Tuple.Create( CodebaseState.NoCompileErrors, 10)};

        [SetUp]
        public void GivenUserIsInBrokenTestsRut()
        {
            _mockTestRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(RunnerType.NUnitResultsFileWatcher, "SomePath",
                                                                            _mockTestRunnerFactory);
            _testRunnerProvider.ConfigureTestRunnerFor(RunnerType.NUnitResultsFileWatcher, "SomePath");

            SimulateABrokenTestsRut();
        }

        [Test]
        public void ExitsBrokenTestsRut_WhenAllTestsPass()
        {
            AppendTransitions(TRANSITIONS_FOR_SUCCESSFUL_BUILD);

            SimulateA.SuccessfulTestRun(_mockIDE, _mockTestRunner);

            Assert.That(_stateBeacon.InBrokenTestsRut, Is.False, "should have exited rut since all tests now pass");
        }

        [Test]
        public void NotifiesUserOnExitingRut()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            AppendTransitions(TRANSITIONS_FOR_SUCCESSFUL_BUILD);

            SimulateA.SuccessfulTestRun(_mockIDE, _mockTestRunner);

            _mockNotifier.Verify(notifier => notifier.PostMessage(It.IsAny<string>()),
                                "should have posted a message indicating that the user is now out of a broken tests rut");
            Assert.IsTrue(listener.HasReceivedChangeNotificationFor("InBrokenTestsRut"));
        }

        [Test]
        public void DoesNotNotifyAgainOnSubsequentGreenBuilds()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            AppendTransitions(TRANSITIONS_FOR_SUCCESSFUL_BUILD);
            SimulateA.SuccessfulTestRun(_mockIDE, _mockTestRunner);

            AppendTransitions(Tuple.Create(CodebaseState.Green, 20),
                                Tuple.Create(CodebaseState.Compiling, 10));
            SimulateA.SuccessfulTestRun(_mockIDE, _mockTestRunner);

            Assert.That(listener.GetCountOfChangeNotificationsFor("InBrokenTestsRut"), Is.EqualTo(1),
                            "should raise only one exit rut notification per rut");
        }
    }
}
