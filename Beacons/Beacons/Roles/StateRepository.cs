// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;

namespace ManiaX.Beacons.Roles
{
    public interface StateRepository
    {
        void LogTransition(DateTime startTime, CodebaseState state, long durationInMillisec);
        IEnumerable<StateTimeSpan> GetTransitions();
    }
}