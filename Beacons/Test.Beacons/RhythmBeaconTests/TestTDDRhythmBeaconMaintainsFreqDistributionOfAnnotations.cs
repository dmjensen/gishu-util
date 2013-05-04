// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Linq;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.Roles;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.RhythmBeaconTests
{
    [TestFixture]
    public class TestTDDRhythmBeaconMaintainsFreqDistributionOfAnnotations : TestTDDRhythmBeaconBase
    {
        private Mock<TestRunner> _mockTestRunner;

        [SetUp]
        public void GivenThatSomeTestRunResultsAreAvailable()
        {
            _mockTestRunner = GivenThat.TestRunnerIsConfigured(_testRunnerProvider, _mockTestRunnerFactory);
            
            PopulateSomeAnnotatedTestRuns();
        }

        private void PopulateSomeAnnotatedTestRuns()
        {
            SimulateA.TestFailure(_mockTestRunner);
            AnnotateTestRun(_beacon.TestResultsStack[0], "Bad Test!");
            SimulateA.TestFailure(_mockTestRunner);
            AnnotateTestRun(_beacon.TestResultsStack[0], "Distractions");
            SimulateA.SuccessfulTestRun(_mockTestRunner, 102);
            AnnotateTestRun(_beacon.TestResultsStack[0], "Woo hoo!");
            SimulateA.TestFailure(_mockTestRunner);
            AnnotateTestRun(_beacon.TestResultsStack[0], "Distractions");
        }

        [Test]
        public void MaintainsFreqDistributionOfAnnotationsOnRedTestRuns()
        {
            Assert.That(_beacon.GroupedAnnotations,
                        Is.EquivalentTo(new[]
                                            {   new GroupedAnnotation{Text="Bad Test!", Count = 1},
                                                new GroupedAnnotation{Text="Distractions", Count = 2}
                                            }));
        }

        [Test]
        public void SkipsRedTestRunsWithoutAnnotationsForFreqDistribution()
        {
            SimulateA.TestFailure(_mockTestRunner);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_beacon.GroupedAnnotations.Where(item => String.IsNullOrEmpty(item.Text)).Count(),
                            Is.EqualTo(0));
        }

        [Test]
        public void MaintainsFreqDistributionAsAnObservableCollection()
        {
            var listener = new PropertyChangeListener(_beacon);
            
            SimulateA.TestFailure(_mockTestRunner);
            AnnotateTestRun(_beacon.TestResultsStack[0], "Bad Test!");

            Assert.That(listener.HasReceivedChangeNotificationFor("GroupedAnnotations"), "should have updated annotation frequency distribution and notified observers");
        }
    }
}