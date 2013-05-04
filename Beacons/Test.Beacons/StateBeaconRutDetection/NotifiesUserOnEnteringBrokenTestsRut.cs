using System;
using System.Collections;
using ManiaX.Beacons;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconRutDetection
{
    [TestFixture]
    public class NotifiesUserOnEnteringBrokenTestsRut : TestStateBeaconRutDetectionBase
    {
        [SetUp]
        public void GivenThatAllTestsHaveNeverPassed()
        {
            SimulateA.BuildFailure(_mockIDE);
            
            _mockTestRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(RunnerType.NUnitResultsFileWatcher, "SomePath",                                                                                  _mockTestRunnerFactory);
            _testRunnerProvider.ConfigureTestRunnerFor(RunnerType.NUnitResultsFileWatcher, "SomePath");
        }

        [Test, TestCaseSource("GetTransitions")]
        public bool Detects(Tuple<CodebaseState, int>[] transitions)
        {
            AppendTransitions(transitions);
            Assert.That(_stateBeacon.InBrokenTestsRut, Is.False);

            SimulateA.TestFailure(_mockIDE, _mockTestRunner);
            
            return _stateBeacon.InBrokenTestsRut;
        }

        public IEnumerable GetTransitions()
        {
            yield return new TestCaseData(WrapInObjectArray(TRANSITIONS_FOR_BROKEN_TESTS_RUT))
                .SetName("Rut : Threshold exceeded since last green build")
                .Returns(true);

            yield return new TestCaseData(WrapInObjectArray(
                                            Tuple.Create(CodebaseState.Compiling, 10),
                                            Tuple.Create(CodebaseState.CompileErrors, 30),
                                            Tuple.Create(CodebaseState.Compiling, 10),
                                            Tuple.Create(CodebaseState.NoCompileErrors, 25)))
                .SetName("No Rut : Threshold exceeded but tests have never passed")
                .Returns(false);

            yield return new TestCaseData(WrapInObjectArray(Tuple.Create(CodebaseState.Unknown, 50),
                                                            Tuple.Create(CodebaseState.Compiling, 5),
                                                            Tuple.Create(CodebaseState.NoCompileErrors, 5)))
                .SetName("No Rut : Threshold not exceeded if time in Unknown state is excluded")
                .Returns(false);

            yield return new TestCaseData(WrapInObjectArray(Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.NoCompileErrors, 30),
                                                            Tuple.Create(CodebaseState.Green, 10),
                                                            Tuple.Create(CodebaseState.Compiling, 5),
                                                            Tuple.Create(CodebaseState.NoCompileErrors, 15),
                                                            Tuple.Create(CodebaseState.Red, 10),
                                                            Tuple.Create(CodebaseState.Compiling, 5),
                                                            Tuple.Create(CodebaseState.NoCompileErrors, 10)))
                .SetName("No Rut : Time since last green build is not over threshold")
                .Returns(false);

        }

        [Test]
        public void DoesNotDetectRutOnAnythingOtherThanARedBuild()
        {
            AppendTransitions(TRANSITIONS_FOR_BROKEN_TESTS_RUT);
            Assert.That(_stateBeacon.InBrokenTestsRut, Is.False);

            SimulateA.SuccessfulBuild(_mockIDE);

            Assert.That(_stateBeacon.InBrokenTestsRut, Is.False,
                "should not trigger rut unless we have a red build / test failure");
        }

        [Test]
        public void NotifiesUserOnEnteringRut()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            
            SimulateABrokenTestsRut();

            Assert.IsTrue(listener.HasReceivedChangeNotificationFor("InBrokenTestsRut"));
            _mockNotifier.Verify(notifier => notifier.PostWarning(It.IsAny<string>()), 
                                    "user should have been notified of entering broken tests rut");
        }

        [Test]
        public void DoesNotNotifyUserOnSubsequentTestFailures()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            SimulateABrokenTestsRut();
            AppendTransitions(  Tuple.Create(CodebaseState.Red, 30),
                                Tuple.Create(CodebaseState.Compiling, 10),
                                Tuple.Create(CodebaseState.NoCompileErrors, 10));

            SimulateA.TestFailure(_mockIDE, _mockTestRunner);

            Assert.That(listener.GetCountOfChangeNotificationsFor("InBrokenTestsRut"), Is.EqualTo(1),
                            "should not notify enter rut more than once per rut");
        }
    }
}
