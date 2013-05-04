// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Threading;
using ManiaX.Beacons.Roles;
using Stopwatch = System.Diagnostics.Stopwatch;
using StopwatchAdapter = ManiaX.Beacons.Roles.Stopwatch;

namespace ManiaX.Beacons
{
    public class DotNetStopwatch : StopwatchAdapter
    {
        protected Stopwatch _stopwatch;
        private DateTime _timerStartedAt;

        public DotNetStopwatch()
        {
            _stopwatch = new Stopwatch();
        }

        public long GetElapsedMillisecAndRestart()
        {
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Restart();
            _timerStartedAt = DateTime.Now;
            PostStart();
            return elapsedMilliseconds;
        }
        virtual protected void PostStart() { }

        public void Start()
        {
            _stopwatch.Start();
            _timerStartedAt = DateTime.Now;
            PostStart();
        }

        public DateTime StartedAt()
        {
            return _timerStartedAt;
        }

        public void Pause()
        {
            _stopwatch.Stop();
        }

        public void Resume()
        {
            _stopwatch.Start();
        }

        public virtual void Dispose()
        {
        }

        
    }

    public class DotNetStopwatchWithUpdates : DotNetStopwatch, TransitionTimer
    {
        private readonly Timer _timer;
        
        public DotNetStopwatchWithUpdates()
        {
            _timer = new Timer(UpdateClientsWithElapsedTimeInCurrentState);
            ElapsedTimeNotificationRateInSeconds = 60;
        }

        private void UpdateClientsWithElapsedTimeInCurrentState(object state)
        {
            if(_stopwatch.IsRunning && this.ElapsedTimeInCurrentState != null)
            {
                this.ElapsedTimeInCurrentState(this, new DurationEventArgs(_stopwatch.Elapsed));
            }
        }

        public event EventHandler<DurationEventArgs> ElapsedTimeInCurrentState;

        private int _elapsedTimeNotificationRateInSeconds;
        public int ElapsedTimeNotificationRateInSeconds
        {
            get { return _elapsedTimeNotificationRateInSeconds; }
            set { 
                _elapsedTimeNotificationRateInSeconds = value;
                var updateRate = TimeSpan.FromSeconds(value);
                _timer.Change(updateRate, updateRate);
            }
        }

        public override void Dispose()
        {
            _timer.Dispose();
        }
        protected override void PostStart()
        {
            UpdateClientsWithElapsedTimeInCurrentState(null);
        }
    }
}