// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Test.Beacons
{
    internal class TestConstants
    {
        /// <summary>
        /// RutDetector: Time when we last had a good build 
        /// </summary>
        public static readonly DateTime GOOD_BUILD_TIME = DateTime.Parse("2010-09-23 08:00:00.000");

        /// <summary>
        /// RutDetector : Time of first build failure
        /// </summary>
        public static DateTime FIRST_FAILURE_AT = GOOD_BUILD_TIME.AddSeconds(15);

        public static string NON_EXISTENT_FILEPATH = @"Z:\I no exist\sneezo\a.xml";

        public const string A_TESTRESULTS_FOLDERPATH = "SomeFilePath";
        public static TimeSpan SOME_DURATION = TimeSpan.FromMilliseconds(12500);
    }
}