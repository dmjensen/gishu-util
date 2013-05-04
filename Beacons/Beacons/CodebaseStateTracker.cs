// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public class CodebaseStateTracker
    {
        private readonly IDE _ide;
        private readonly StateRepository _stateRepository;
        private readonly TransitionTimer _transitionTimer;

        private CodebaseState _currentState;
        public CodebaseState CurrentState
        {
            get { return _currentState; }
            
            private set 
            {
                if (Paused || (value == _currentState))
                    return;

                LogTransitionAndRestartTimer();
                _currentState = value;

                if (this.StateChanged != null)
                    this.StateChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler StateChanged;

        public event EventHandler<DurationEventArgs> ElapsedTimeInCurrentState
        {
            add { _transitionTimer.ElapsedTimeInCurrentState += value; }
            remove { _transitionTimer.ElapsedTimeInCurrentState -= value; }
        }

        public CodebaseStateTracker(IDE ide, StateRepository stateRepository, TransitionTimer transitionTimer)
        {
            _ide = ide;
            _stateRepository = stateRepository;
            _transitionTimer = transitionTimer;
            _transitionTimer.Start();

            _ide.BuildInitiated += SetStateToCompiling;
            _ide.BuildSucceeded += SetStateToNoCompileErrors;
            _ide.BuildFailed += SetStateToCompileErrors;

            _ide.SolutionClosed += SetStateToUnknown;
        }

        public void Dispose()
        {
            LogTransitionAndRestartTimer();

            _ide.BuildInitiated -= SetStateToCompiling;
            _ide.BuildSucceeded -= SetStateToNoCompileErrors;
            _ide.BuildFailed -= SetStateToCompileErrors;

            _ide.SolutionClosed -= SetStateToUnknown;
        }

        public IEnumerable<StateTimeSpan> GetTransitions()
        {
            return _stateRepository.GetTransitions();
        }

        public void SetTestRunner(TestRunner testRunner)
        {
            testRunner.TestsPassed += ChangeStateIfWeHaveAGreenBuild;
            testRunner.TestsFailed += ChangeStateIfWeHaveARedBuild;
        }

        public void ClearTestRunner(TestRunner testRunner)
        {
            testRunner.TestsPassed -= ChangeStateIfWeHaveAGreenBuild;
            testRunner.TestsFailed -= ChangeStateIfWeHaveARedBuild;
        }

        public void Pause()
        {
            Paused = true;
            _transitionTimer.Pause();
        }

        public void Resume()
        {
            Paused = false;
            _transitionTimer.Resume();
        }

        private bool Paused { get; set; }

        #region State Transition helpers

        private void SetStateToUnknown(object sender, EventArgs e)
        {
            this.CurrentState = CodebaseState.Unknown;
        }

        private void SetStateToCompileErrors(object sender, EventArgs e)
        {
            this.CurrentState = CodebaseState.CompileErrors;
        }

        private void SetStateToNoCompileErrors(object sender, EventArgs e)
        {
            this.CurrentState = CodebaseState.NoCompileErrors;
        }

        private void SetStateToCompiling(object sender, EventArgs e)
        {
            this.CurrentState = CodebaseState.Compiling;
        }

        private void ChangeStateIfWeHaveARedBuild(object sender, TestResultEventArgs e)
        {
            if ((this.CurrentState == CodebaseState.NoCompileErrors) || (this.CurrentState == CodebaseState.Green))
                this.CurrentState = CodebaseState.Red;
        }

        private void ChangeStateIfWeHaveAGreenBuild(object sender, TestResultEventArgs e)
        {
            if ((this.CurrentState == CodebaseState.NoCompileErrors) || (this.CurrentState == CodebaseState.Red))
                this.CurrentState = CodebaseState.Green;
        }

        #endregion

        private void LogTransitionAndRestartTimer()
        {
            _stateRepository.LogTransition(_transitionTimer.StartedAt(), this.CurrentState, _transitionTimer.GetElapsedMillisecAndRestart());
        }

    }
}