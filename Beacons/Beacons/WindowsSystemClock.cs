// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public class WindowsSystemClock : SystemClock
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}