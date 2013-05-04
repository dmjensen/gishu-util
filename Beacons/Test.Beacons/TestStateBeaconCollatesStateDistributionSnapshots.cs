// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ManiaX.Beacons;
using ManiaX.Beacons.DataStructs;
using ManiaX.Test.Beacons.StateBeaconRutDetection;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconTests
{
    [TestFixture]
    public class TestStateBeaconCollatesStateDistributionSnapshots : TestStateBeaconRutDetectionBase
    {
        [SetUp]
        public void BeforeEachTest()
        {
            //U5-C10-CE25
            AppendTransitions(new Tuple<CodebaseState, int>(CodebaseState.Unknown, 5),
                              new Tuple<CodebaseState, int>(CodebaseState.Compiling, 10),
                              new Tuple<CodebaseState, int>(CodebaseState.CompileErrors, 25));
        }

        [Test]
        public void AppendsSnapshotsOfStateDistribution_WhenCurrentStateChanges()
        {
            _mockIDE.Raise(ide => ide.SolutionOpened += null, EventArgs.Empty);
            _mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);

            var expectedSnapshots = new List<StateDistributionSnapshot>
                                        {
                                            new StateDistributionSnapshot{ Timestamp = DateTime.Parse("2010-08-24 15:00:40"),
                                                                            Compiling = 28.57f, CompileErrors = 71.43f }
                                        };
            Assert.That(_stateBeacon.StateDistributionSnapshots, Is.EqualTo(expectedSnapshots));

            //U5-C10-CE25-C10-NCE5-G10
            AppendTransitions( new Tuple<CodebaseState, int>(CodebaseState.Compiling, 10),
                                new Tuple<CodebaseState, int>(CodebaseState.NoCompileErrors, 5),
                                new Tuple<CodebaseState, int>(CodebaseState.Green, 10));
            expectedSnapshots.Add(
                new StateDistributionSnapshot { Timestamp = DateTime.Parse("2010-08-24 15:01:05"),
                                                Compiling = 33.33f, CompileErrors = 41.67f, NoCompileErrors = 8.33f, 
                                                Green = 16.67f });

            _mockIDE.Raise(ide => ide.BuildSucceeded += null, EventArgs.Empty);

            Assert.That(_stateBeacon.StateDistributionSnapshots, Is.EqualTo(expectedSnapshots));
        }

        [Test]
        public void NotifiesChangeInStateDistributionSnapshots()
        {
            Assert.That(_stateBeacon.StateDistributionSnapshots, 
                            Is.InstanceOf(typeof(ObservableCollection<StateDistributionSnapshot>)));
        }
    }
}