// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Beacons.Roles
{
    public interface IDE
    {
        event EventHandler SolutionOpened;
        event EventHandler SolutionClosed;

        event EventHandler BuildInitiated;
        event EventHandler BuildSucceeded;
        event EventHandler BuildFailed;
    }
}