// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TDDRhythmBeaconTests
{
    [TestFixture]
    public class TestTDDRhythmBeaconAnnotatesTestRuns : TestTDDRhythmBeaconBase
    {
        private Mock<SettingsRepository> _settingsStore;
        private Mock<TestRunner> _mockTestRunner;

        private static readonly string NOTE_1 = "This is Note 1";
        private TestRun THIRD_TEST_RUN;
        private TestRun SECOND_TEST_RUN;

        [SetUp]
        public void GivenThatSomeTestRunResultsAreAvailable()
        {
            _settingsStore = new Mock<SettingsRepository>();
            _mockTestRunner = GivenThat.TestRunnerIsConfigured(_testRunnerProvider, _mockTestRunnerFactory);

            SimulateA.SuccessfulTestRun(_mockTestRunner, 100);
            SimulateA.TestFailure(_mockTestRunner);
            SimulateA.SuccessfulTestRun(_mockTestRunner, 101);

            THIRD_TEST_RUN = _beacon.TestResultsStack[0];
            SECOND_TEST_RUN = _beacon.TestResultsStack[1];
        }

        [Test]
        public void SupportsAnnotatingSelectedTestResult()
        {
            _beacon.SelectedTestRun = THIRD_TEST_RUN;
            _beacon.NoteForSelectedTestRun = NOTE_1;

            Assert.That(THIRD_TEST_RUN.Note, Is.EqualTo(NOTE_1), "The selected test run should have been tagged");
        }

        [Test]
        public void ShowsNoteForSelectedTestRun()
        {
            AnnotateTestRun(THIRD_TEST_RUN, NOTE_1);
            AnnotateTestRun(SECOND_TEST_RUN, "This is Note 2");

            _beacon.SelectedTestRun = THIRD_TEST_RUN;

            Assert.That(_beacon.NoteForSelectedTestRun, Is.EqualTo(NOTE_1), "should show the note for the selected test result");
        }


        [Test]
        public void NotifiesChangeIn_SelectedTestRun()
        {
            var listener = new PropertyChangeListener(_beacon);
            
            _beacon.SelectedTestRun = SECOND_TEST_RUN;

            Assert.That(listener.HasReceivedChangeNotificationFor("SelectedTestRun"), "should have notified change in SelectedTestRun");
        }

        [Test]
        public void NotifiesChangeIn_NoteForSelectedTestRun_WhenSelectedNodeChanges()
        {
            var listener = new PropertyChangeListener(_beacon);
            
            _beacon.SelectedTestRun = _beacon.TestResultsStack[1];

            Assert.That(listener.HasReceivedChangeNotificationFor("NoteForSelectedTestRun"));
        }

        [Test]
        public void MaintainsListOfRecentlyUsedNotesForEasyTagging()
        {
            AnnotateTestRun(THIRD_TEST_RUN, NOTE_1);
            AnnotateTestRun(SECOND_TEST_RUN, NOTE_1);

            Assert.That(_beacon.MruNotes.Count, Is.EqualTo(1));
            Assert.That(_beacon.MruNotes, Contains.Item(NOTE_1));
        }

        [Test]
        public void MaintainsListOfRecentlyUsedNotesAsStack()
        {
            AnnotateTestRun(THIRD_TEST_RUN, NOTE_1);
            AnnotateTestRun(SECOND_TEST_RUN, NOTE_1);
            string NOTE_2 = "This is the last note I made";
            AnnotateTestRun(_beacon.TestResultsStack[2], NOTE_2);
            
            Assert.That(_beacon.MruNotes.Count, Is.EqualTo(2), "should be 2 items in Mru notes dropdown");
            Assert.That(_beacon.MruNotes[0], Is.EqualTo(NOTE_2), "last used note should be top of the dropdown");
        }

        [Test]
        public void TrimsNotesToAvoidDuplicatesInListOfRecentlyUsedNotes()
        {
            AnnotateTestRun(THIRD_TEST_RUN, NOTE_1);
            AnnotateTestRun(SECOND_TEST_RUN, "  " + NOTE_1 + "  ");

            Assert.That(_beacon.MruNotes.Count, Is.EqualTo(1), "dropdown should not contain duplicates coz of leading/trailing spaces");
        }

        [Test]
        public void ListOfRecentlyUsedNotesNotifiesChangeInCollection()
        {
            var anObserver = new NotificationListener();
            _beacon.MruNotes.CollectionChanged += anObserver.Handler;

            AnnotateTestRun(THIRD_TEST_RUN, NOTE_1);

            Assert.That(anObserver.NotificationReceived, "Mru notes should notify observers of change in collection so that the GUI can resync");
        }

        
        [Test]
        public void CanLoadMruNotesFromSettingsStore()
        {
            _settingsStore.Setup(store => store[It.IsAny<string>()]).Returns(String.Empty);
            _settingsStore.Setup(store => store["MruNotes1"]).Returns("slime");
            _settingsStore.Setup(store => store["MruNotes2"]).Returns("emacs");
            _settingsStore.Setup(store => store["MruNotes3"]).Returns("sbcl");

            _beacon = new TDDRhythmBeacon(_testRunnerProvider, _mockStopwatch.Object, new MockUiUpdater());
            _beacon.LoadSettingsFrom(_settingsStore.Object);
            Assert.That(_beacon.MruNotes, Is.EqualTo(new[] { "slime", "emacs", "sbcl" }));
        }

        [Test]
        public void CanSaveMruNotesToSettingStore()
        {
            _settingsStore.SetupSet(store => store["MruNotes1"] = "slime");
            _settingsStore.SetupSet(store => store["MruNotes2"] = "emacs");
            _settingsStore.SetupSet(store => store["MruNotes3"] = "sbcl");
            for (int looper = 4; looper <= 10; looper++ )
            {
                var key = "MruNotes" + looper;
                _settingsStore.SetupSet(store => store[key] = string.Empty);
            }

            AnnotateTestRun(THIRD_TEST_RUN, "sbcl");
            AnnotateTestRun(THIRD_TEST_RUN, "emacs");
            AnnotateTestRun(THIRD_TEST_RUN, "slime");
            _beacon.SaveSettingsTo(_settingsStore.Object);

            _settingsStore.VerifyAll();
        }
    }
}