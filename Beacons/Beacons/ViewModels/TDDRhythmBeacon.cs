// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons.ViewModels
{
    public class TDDRhythmBeacon : ViewModelBase, HasSettings
    {
        public static readonly string ChangeHatsTooltip = Strings.RB_ChangeHats;

        private readonly Stopwatch _stopwatch;
        private TimeSpan _accumulator = TimeSpan.Zero;
        private int _runsPerHour;
        private MruItemsStack _mruNotesStack;

        private const int RPH_SCALE_MAX_VALUE = 80;

        public TDDRhythmBeacon(TestRunnerProvider testRunnerProvider, Stopwatch stopwatch, UiUpdateDispatcher uiUpdater) 
            : base(uiUpdater) 
        {
            _stopwatch = stopwatch;
            TestResultsStack = new ObservableCollection<TestRun>();
            
            _stopwatch.Start();
            testRunnerProvider.TestRunnerCreated += SubscribeToNewTestRunner;
            testRunnerProvider.TestRunnerDisposing += UnsubscribeFromOldTestRunner;

            this.ToggleRefactoringHat = new DelegatingCommand(ChangeHats);
            
            _mruNotesStack = new MruItemsStack("MruNotes", 10);
            this.RphGaugeMaxScale = RPH_SCALE_MAX_VALUE;
        }

        public ObservableCollection<TestRun> TestResultsStack { get; private set; }

        public void Pause()
        {
            Paused = true;
            _stopwatch.Pause();
        }

        public void Resume()
        {
            Paused = false;
            _stopwatch.Resume();    
        }

        #region Runs Per Hour Gauge
        public int RunsPerHour
        {
            get { return _runsPerHour; }
            private set
            {
                _runsPerHour = value;
                NotifyPropertyChangedFor("RunsPerHour");
            }
        }

        private int _rphGaugeMaxScale;
        public int RphGaugeMaxScale
        {
            get { return _rphGaugeMaxScale; }
            set
            {
                if (value == _rphGaugeMaxScale)
                    return;

                _rphGaugeMaxScale = value;
                NotifyPropertyChangedFor("RphGaugeMaxScale");
            }
        } 
        #endregion

        private bool _isRefactoringHatOn;
        public bool IsRefactoringHatOn
        {
            get { return _isRefactoringHatOn; }
            private set 
            {   
                _isRefactoringHatOn = value; 
                NotifyPropertyChangedFor("IsRefactoringHatOn");
            }
        }

        public ICommand ToggleRefactoringHat { get; set; }

        #region Annotations
        private TestRun _selectedTestRun;

        public TestRun SelectedTestRun
        {
            get { return _selectedTestRun; }
            set
            {
                _selectedTestRun = value;

                NotifyPropertyChangedFor("SelectedTestRun");
                NotifyPropertyChangedFor("NoteForSelectedTestRun");
            }
        }

        public string NoteForSelectedTestRun
        {
            get
            {
                if (SelectedTestRun == null)
                    return string.Empty;

                return SelectedTestRun.Note;
            }
            set
            {
                if (SelectedTestRun == null)
                    return;

                value = value.Trim();
                SelectedTestRun.Note = value;
                NotifyPropertyChangedFor("GroupedAnnotations");

                _mruNotesStack.Push(value);
                if (MruNotes.Contains(value))
                    return;
                MruNotes.Insert(0, value);
            }
        }

        public ObservableCollection<string> MruNotes
        {
            get { return _mruNotesStack.Items; }
        }

        public IEnumerable<GroupedAnnotation> GroupedAnnotations
        {
            get
            {
                return TestResultsStack
                    .Where(result => (result.Result == TestResult.Red) && (!String.IsNullOrEmpty(result.Note)))
                    .GroupBy(result => result.Note)
                    .Select(group => new GroupedAnnotation { Text = group.Key, Count = group.Count() });
            }
        }

        #endregion

        private bool Paused { get; set; }

        private void ChangeHats(object obj)
        {
            IsRefactoringHatOn = !IsRefactoringHatOn;
        }

        private void SubscribeToNewTestRunner(object sender, TestRunnerEventArgs e)
        {
            e.UnitTestRunner.TestsPassed += AddGreenResultToStack;
            e.UnitTestRunner.TestsFailed += AddRedResultToStack;
        }

        private void UnsubscribeFromOldTestRunner(object sender, TestRunnerEventArgs e)
        {
            e.UnitTestRunner.TestsPassed -= AddGreenResultToStack;
            e.UnitTestRunner.TestsFailed -= AddRedResultToStack;
        }

        private void AddRedResultToStack(object sender, FailedTestRunEventArgs e)
        {
            DoOnUIThread(delegate
                             {
                                 if (Paused)
                                     return;


                                 var shouldExpandStackTraces = e.Failures.Count() == 1;

                                 var failureVMs = e.Failures.Select(
                                     failure =>
                                     new FailureVM(failure.TestName, failure.Message, shouldExpandStackTraces, failure.StackTrace));
                                 TestResultsStack.Insert(0,
                                                         new TestRun { Result = TestResult.Red, TestCount = e.TestCount, Failures = failureVMs});
                                 SelectedTestRun = TestResultsStack[0];
                                 UpdateFrequencyOfTestRuns();
                             });
        }

        private void AddGreenResultToStack(object sender, TestResultEventArgs e)
        {
            DoOnUIThread(delegate
                             {
                                 if (Paused)
                                     return;

                                 TestResultsStack.Insert(0,
                                                         new TestRun { 
                                                                       Result = IsRefactoringHatOn ? TestResult.RefactoringWin : TestResult.Green, 
                                                                       TestCount = e.TestCount });
                                 SelectedTestRun = TestResultsStack[0];
                                 UpdateFrequencyOfTestRuns();
                             });
        }

        private void UpdateFrequencyOfTestRuns()
        {
            _accumulator = _accumulator.Add( TimeSpan.FromMilliseconds(_stopwatch.GetElapsedMillisecAndRestart()) );
            this.RunsPerHour = (int) (TestResultsStack.Count/_accumulator.TotalHours);

            var modTen = RunsPerHour % 10;
            var roundedRphValue = (modTen == 0 ? RunsPerHour : RunsPerHour + 10 - modTen);
            this.RphGaugeMaxScale = Math.Max(roundedRphValue, RPH_SCALE_MAX_VALUE);
        }

        #region Persist MRU Annotations
        public void LoadSettingsFrom(SettingsRepository settingsStore)
        {
            _mruNotesStack.LoadFrom(settingsStore);
        }

        public void SaveSettingsTo(SettingsRepository settingsStore)
        {
            _mruNotesStack.SaveTo(settingsStore);
        } 
        #endregion
    }
}