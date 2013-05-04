// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Beacons.Roles
{
    public interface Stopwatch : IDisposable
    {
        long GetElapsedMillisecAndRestart();
        void Start();
        DateTime StartedAt();
        void Pause();
        void Resume();
    }

    public interface TransitionTimer : Stopwatch
    {
        event EventHandler<DurationEventArgs> ElapsedTimeInCurrentState;
        int ElapsedTimeNotificationRateInSeconds { get; set; }
    }

    public class DurationEventArgs : EventArgs
    {
        private readonly TimeSpan _duration;

        public DurationEventArgs(TimeSpan duration)
        {
            _duration = duration;
        }

        public TimeSpan Duration
        {
            get { return _duration; }
        }
    }
}