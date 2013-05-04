// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission
﻿using System;
using System.Collections.Generic;
﻿using System.Collections.ObjectModel;
﻿using System.Linq;
﻿using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons.ViewModels
{
    public class StateBeacon: ViewModelBase
    {
        public static readonly string EnteredBrokenBuildsRutAdvisory = Strings.SB_EnteredBrokenBuildsRutAdvisory;
        public static readonly string EnteredBrokenTestsRutAdvisory = Strings.SB_EnteredBrokenTestsRutAdvisory;

        private readonly CodebaseStateTracker _codebaseStateTracker;
        private bool _inBrokenBuildsRut;
        private bool _inBrokenTestsRut;
        private string _elapsedTimeInCurrentState;
        private readonly UserNotifier _notifier;

        public bool InBrokenBuildsRut
        {
            get { return _inBrokenBuildsRut; }
            private set
            {
                if (value == _inBrokenBuildsRut)
                    return;
                _inBrokenBuildsRut = value;
                NotifyPropertyChangedFor("InBrokenBuildsRut");
                if (InBrokenBuildsRut)
                    _notifier.PostWarning(Strings.SB_EnteredBrokenBuildsRutAdvisory);
                else
                    _notifier.PostMessage(Strings.SB_LeftBrokenBuildsRutAdvisory);
            }
        }

        public bool InBrokenTestsRut
        {
            get {
                return _inBrokenTestsRut;
            }
            private set
            {
                if (value == _inBrokenTestsRut)
                    return;

                _inBrokenTestsRut = value;
                NotifyPropertyChangedFor("InBrokenTestsRut");

                if (_inBrokenTestsRut)
                    _notifier.PostWarning(Strings.SB_EnteredBrokenTestsRutAdvisory);
                else
                    _notifier.PostMessage(Strings.SB_LeftBrokenTestsRutAdvisory);
            }
        }

        public CodebaseState CurrentState
        {
            get { return _codebaseStateTracker.CurrentState; }
        }

        public string ElapsedTimeInCurrentState
        {
            get { return _elapsedTimeInCurrentState; }
            set
            {
                _elapsedTimeInCurrentState = value;
                NotifyPropertyChangedFor("ElapsedTimeInCurrentState");
            }
        }

        public ObservableCollection<StateDistributionSnapshot> StateDistributionSnapshots 
        {   get; private set; }

        public TimeSpan RutThreshold
        {
            get; set;
        }

        public StateBeacon(TestRunnerProvider testRunnerProvider, CodebaseStateTracker codebaseStateTracker, UserNotifier notifier, UiUpdateDispatcher uiUpdater) : base(uiUpdater)
        {
            _codebaseStateTracker = codebaseStateTracker;
            _notifier = notifier;

            testRunnerProvider.TestRunnerCreated += SubscribeForTestRunnerUpdates;
            testRunnerProvider.TestRunnerDisposing += UnsubscribeFromTestRunnerUpdates;

            _codebaseStateTracker.StateChanged += delegate { DoOnUIThread(ProcessStateChange); };
            _codebaseStateTracker.ElapsedTimeInCurrentState += UpdateElapsedTimeField;

            StateDistributionSnapshots = new ObservableCollection<StateDistributionSnapshot>();

            RutThreshold = TimeSpan.FromMinutes(5);
        }

        private void ProcessStateChange()
        {
            RaiseCurrentStateChangedNotification();
            AddStateDistributionSnapshot();
            DetectRuts();
        }

        private void DetectRuts()
        {
            var pastStates = _codebaseStateTracker.GetTransitions()
                .Where(transition => transition.State != CodebaseState.Unknown);

            InBrokenBuildsRut = HasExceededThresholdSinceLast(
                                    CodebaseState.NoCompileErrors, CodebaseState.CompileErrors, 
                                    InBrokenBuildsRut, 
                                    pastStates);

            InBrokenTestsRut = HasExceededThresholdSinceLast(
                                    CodebaseState.Green, CodebaseState.Red, 
                                    InBrokenTestsRut, 
                                    pastStates);
        }

        private bool HasExceededThresholdSinceLast(CodebaseState goodState, CodebaseState badState, 
                                                    bool currentState, 
                                                    IEnumerable<StateTimeSpan> pastTransitions)
        {
            if ((CurrentState == goodState) ||
                !pastTransitions.Any(trans => trans.State == goodState))
                return false;

            if (CurrentState != badState)
                return currentState;

            var timeSinceLastGoodState = TimeSpan.FromMilliseconds(
                                            pastTransitions.Reverse()
                                                .TakeWhile(entry => entry.State != goodState)
                                                .Sum(entry => entry.DurationInMilliSecs));

            return (timeSinceLastGoodState > RutThreshold);
        }

        public void Dispose()
        {
            _codebaseStateTracker.Dispose();
        }

        public void Pause()
        {
            _codebaseStateTracker.Pause();
        }

        public void Resume()
        {
            _codebaseStateTracker.Resume();
        }

        private void UpdateElapsedTimeField(object sender, DurationEventArgs e)
        {
            ElapsedTimeInCurrentState = e.Duration.ToString(@"hh\:mm\:ss");
        }

        private void AddStateDistributionSnapshot()
        {
            var pastTransitions = _codebaseStateTracker.GetTransitions()
                                                    .Where(transition => transition.State != CodebaseState.Unknown);
            if (pastTransitions.Count() == 0)
                return;

            var totalMillisecs
                = pastTransitions.Aggregate(0L,
                                            (sum, transition) => sum + transition.DurationInMilliSecs);
            var mergedStates = from eachTransition in pastTransitions
                                group eachTransition by eachTransition.State into groupedStates
                                select new MergedState
                                {
                                    State = groupedStates.Key,
                                    TotalMillisecondsInState
                                        = groupedStates.Sum(state => state.DurationInMilliSecs)
                                };

            var lastTransition = pastTransitions.Last();
            var snapshot = new StateDistributionSnapshot{
                                    Timestamp = lastTransition.StartTime.AddMilliseconds(lastTransition.DurationInMilliSecs),
                                    Compiling = GetPercentTimeInState(mergedStates, CodebaseState.Compiling, totalMillisecs),
                                    NoCompileErrors = GetPercentTimeInState(mergedStates, CodebaseState.NoCompileErrors, totalMillisecs),
                                    CompileErrors = GetPercentTimeInState(mergedStates, CodebaseState.CompileErrors, totalMillisecs),
                                    Red = GetPercentTimeInState(mergedStates, CodebaseState.Red, totalMillisecs),
                                    Green = GetPercentTimeInState(mergedStates, CodebaseState.Green, totalMillisecs)
                                };

            StateDistributionSnapshots.Add(snapshot);
        }

        private struct MergedState
        {
            public CodebaseState State { get; set; }

            public long TotalMillisecondsInState { get; set; }
        }

        private static float GetPercentTimeInState(IEnumerable<MergedState> mergedPhases, CodebaseState state, long totalMillisecs)
        {
            var phaseData = mergedPhases.FirstOrDefault(phase => phase.State.Equals(state));
            if (phaseData.Equals(new MergedState()))
                return 0f;

            return (float) Math.Round(
                                100d * phaseData.TotalMillisecondsInState / totalMillisecs,
                                2);
        }

        private void RaiseCurrentStateChangedNotification()
        {
            NotifyPropertyChangedFor("CurrentState");
        }

        private void UnsubscribeFromTestRunnerUpdates(object sender, TestRunnerEventArgs e)
        {
            _codebaseStateTracker.ClearTestRunner(e.UnitTestRunner);
        }

        private void SubscribeForTestRunnerUpdates(object sender, TestRunnerEventArgs e)
        {
            _codebaseStateTracker.SetTestRunner(e.UnitTestRunner);
        }
    }
}