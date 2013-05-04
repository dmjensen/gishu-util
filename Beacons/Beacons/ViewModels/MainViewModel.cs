// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Windows.Input;
﻿using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TDDRhythmBeacon _tddRhythmBeacon;
        private readonly StateBeacon _stateBeacon;
        private readonly TestRunnerConfiguration _testRunnerConfig;
        private bool _paused;

        public MainViewModel(StateBeacon stateBeacon, TDDRhythmBeacon tddRhythmBeacon, TestRunnerConfiguration testRunnerConfiguration, UiUpdateDispatcher uiUpdater) : base(uiUpdater)
        {
            _tddRhythmBeacon = tddRhythmBeacon;
            _testRunnerConfig = testRunnerConfiguration;
            _stateBeacon = stateBeacon;

            TogglePauseCommand = new DelegatingCommand(TogglePause);
            TogglePauseTooltip = Strings.Main_PauseAllBeacons; 
        }

        public StateBeacon StateBeaconVM
        {
            get { return _stateBeacon; }
        }

        public TDDRhythmBeacon TDDRhythmBeaconVM
        {
            get { return _tddRhythmBeacon; }
        }

        public TestRunnerConfiguration TestRunnerConfigVM
        {
            get { return _testRunnerConfig; }
        }

        public ICommand TogglePauseCommand { get; set; }

        public bool IsPaused
        {
            get { return _paused; }
            private set
            {
                _paused = value;
                NotifyPropertyChangedFor("IsPaused");
            }
        }

        private string _togglePauseTooltip;
        public string TogglePauseTooltip
        {
            get { return _togglePauseTooltip; }
            set
            {
                _togglePauseTooltip = value;
                NotifyPropertyChangedFor("TogglePauseTooltip");
            }
        }

        public void Dispose()
        {
            StateBeaconVM.Dispose();
            TestRunnerConfigVM.ConfigArguments = String.Empty;
            
        }

        private void TogglePause(object parameter)
        {
            IsPaused = !IsPaused;
            TogglePauseTooltip = IsPaused ? Strings.Main_ResumeAllBeacons : Strings.Main_PauseAllBeacons;

            if (IsPaused)
            {   _stateBeacon.Pause(); 
                _tddRhythmBeacon.Pause();
            }
            else
            {
                _stateBeacon.Resume();
                _tddRhythmBeacon.Resume();
            }
        }
    }
}