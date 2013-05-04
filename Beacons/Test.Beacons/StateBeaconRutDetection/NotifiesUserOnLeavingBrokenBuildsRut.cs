using System;
using System.Collections.Generic;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using ManiaX.Test.Beacons.StateBeaconTests;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconRutDetection
{
    [TestFixture]
    public class NotifiesUserOnLeavingBrokenBuildsRut : TestStateBeaconRutDetectionBase
    {
        [SetUp]
        public void GivenThatUserIsInBrokenBuildsRut()
        {
            SimulateABrokenBuildsRut();
        }
        
        [Test]
        public void ExitsBrokenBuildsRut_WhenBuildSucceeds()
        {
            AppendTransitions(Tuple.Create(CodebaseState.Compiling, 25));
            Assert.That(_stateBeacon.InBrokenBuildsRut, Is.True);

            _mockIDE.Raise(ide => ide.BuildSucceeded += null, EventArgs.Empty);

            Assert.That(_stateBeacon.InBrokenBuildsRut, Is.False, "should have turned InRut off ; since build has succeeded");
        }

        [Test]
        public void NotifiesUserOnExitingRut()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            AppendTransitions(Tuple.Create(CodebaseState.Compiling, 10));

            SimulateA.SuccessfulBuild(_mockIDE);

            Assert.IsTrue(listener.HasReceivedChangeNotificationFor("InBrokenBuildsRut"));
            _mockNotifier.Verify(notifier => notifier.PostMessage(It.IsAny<string>()),
                                 "should have posted an entry to notifications list");
        }

        [Test]
        public void DoesNotNotifyAgainOnSubsequentSuccessfulBuilds()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            AppendTransitions(Tuple.Create(CodebaseState.Compiling, 10));
            SimulateA.SuccessfulBuild(_mockIDE);

            AppendTransitions(  Tuple.Create(CodebaseState.NoCompileErrors,20),
                                Tuple.Create(CodebaseState.Compiling, 10));
            SimulateA.SuccessfulBuild(_mockIDE);

            Assert.That(listener.GetCountOfChangeNotificationsFor("InBrokenBuildsRut"),
                            Is.EqualTo(1));
        }
    }
}