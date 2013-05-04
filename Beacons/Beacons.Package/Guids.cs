// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿// Guids.cs
// MUST match guids.h
using System;

namespace ManiaX.Beacons
{
    static class GuidList
    {
        public const string guidBeaconsPkgString = "46e620a5-938c-43a7-bbc9-03bcb4f5c462";
        public const string guidBeaconsCmdSetString = "3a0fc4dc-3687-44b8-8167-f2308ae7ad02";
        public const string guidToolWindowPersistanceString = "80fa5586-7181-4335-9fc2-c08b5bbb06b5";

        public static readonly Guid guidBeaconsCmdSet = new Guid(guidBeaconsCmdSetString);
    };
}