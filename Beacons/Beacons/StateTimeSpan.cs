// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Beacons
{
    public struct StateTimeSpan{
        public StateTimeSpan(CodebaseState type, DateTime startTime, long durationMilliSecs):this()
        {
            State = type;
            StartTime = startTime;
            DurationInMilliSecs = durationMilliSecs;
        }

        public CodebaseState State { get; private set; }

        public DateTime StartTime { get; private set; }

        public long DurationInMilliSecs { get; private set; }
    }
}