using System;
using System.Collections;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconRutDetection
{
    [TestFixture]
    public class   NotifiesUserOnEnteringBrokenBuildsRut : TestStateBeaconRutDetectionBase
    {
        [SetUp]
        public void GivenThatTheUserIsNotInBrokenBuildsRut()
        {
            SimulateA.BuildFailure(_mockIDE);
        }

        [Test, TestCaseSource("GetTransitions")]
        public bool Detects(Tuple<CodebaseState, int>[] transitions)
        {
            AppendTransitions( transitions );
            Assert.That(_stateBeacon.InBrokenBuildsRut, Is.False);
            
            SimulateA.BuildFailure(_mockIDE);

            return _stateBeacon.InBrokenBuildsRut;
        }

        public IEnumerable GetTransitions()
        {
            yield return new TestCaseData(WrapInObjectArray(TRANSITIONS_FOR_BROKEN_BUILDS_RUT))
                .SetName("Rut if Time since last good build is over threshold")
                .Returns(true);
            yield return new TestCaseData(WrapInObjectArray(Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.NoCompileErrors, 30),
                                                            Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.CompileErrors, 20),
                                                            Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.CompileErrors, 20)))
                .SetName("No Rut if Time since last good build is not over threshold")
                .Returns(false);
            yield return new TestCaseData(WrapInObjectArray(Tuple.Create(CodebaseState.Unknown, 30),
                                                            Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.CompileErrors, 25)))
                .SetName("No Rut if Threshold not exceeded if time in Unknown state is excluded")
                .Returns(false);

            yield return new TestCaseData(WrapInObjectArray(Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.CompileErrors, 20),
                                                            Tuple.Create(CodebaseState.Compiling, 10),
                                                            Tuple.Create(CodebaseState.CompileErrors, 25)))
                .SetName("No Rut if Threshold exceeded but build has never succeeded")
                .Returns(false);
        }

        [Test]
        public void DoesNotDetectRutOnAnythingOtherThanFailedBuild()
        {
            AppendTransitions(TRANSITIONS_FOR_BROKEN_BUILDS_RUT);
            Assert.That(_stateBeacon.InBrokenBuildsRut, Is.False);

            _mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);

            Assert.That(_stateBeacon.InBrokenBuildsRut, Is.False, 
                            "Should not flag rut unless we have a broken build");
        }
        [Test]
        public void NotifiesUserOnEnteringRut()
        {
            var listener = new PropertyChangeListener(_stateBeacon);

            AppendTransitions( TRANSITIONS_FOR_BROKEN_BUILDS_RUT);
            
            SimulateA.BuildFailure(_mockIDE);

            Assert.IsTrue(listener.HasReceivedChangeNotificationFor("InBrokenBuildsRut"));
            _mockNotifier.Verify(n => n.PostWarning(It.IsAny<string>()),
                                 "should have posted an entry to notifications list");
        }

        [Test]
        public void DoesNotNotifyUserOnSubsequentBuildFailures()
        {
            var listener = new PropertyChangeListener(_stateBeacon);

            SimulateABrokenBuildsRut();
            AppendTransitions(Tuple.Create(CodebaseState.CompileErrors, 20),
                                Tuple.Create(CodebaseState.Compiling, 10));

            SimulateA.BuildFailure(_mockIDE);

            Assert.That(listener.GetCountOfChangeNotificationsFor("InBrokenBuildsRut"), Is.EqualTo(1),
                            "should raise only one entered rut notification per rut");
        }
    }
}