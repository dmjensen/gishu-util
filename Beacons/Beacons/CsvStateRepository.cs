// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public class CsvStateRepository: StateRepository
    {
        private readonly string _sessionLogFilePath;
        private static int __instancesCreated;

        public CsvStateRepository(string targetFolderForLogs)
        {
            string timestampedPrefix = DateTime.Now.ToString("yyyyMMdd_HHmmss.FFF");
          
            _sessionLogFilePath = Path.Combine(targetFolderForLogs, 
                                               String.Format("{0}.{1}_Beacons_CodebaseTransitions.csv", timestampedPrefix, ++__instancesCreated) );
        }

        public void LogTransition(DateTime startTime, CodebaseState state, long durationInMillisec)
        {
            startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Local);
            File.AppendAllText( _sessionLogFilePath,
                                String.Format("{0},{1},{2}{3}", startTime.ToString("o"), state, durationInMillisec, Environment.NewLine));
        }

        public IEnumerable<StateTimeSpan> GetTransitions()
        {
            if (!File.Exists(_sessionLogFilePath))
                return new List<StateTimeSpan>();

            return from line in File.ReadAllLines(_sessionLogFilePath)
                select ParseState(line);
        }

        private StateTimeSpan ParseState(string serializedStateInCSV)
        {
            var fields = serializedStateInCSV.Split(',');
            return new StateTimeSpan(
                (CodebaseState)Enum.Parse(typeof(CodebaseState), fields[1]),
                DateTime.Parse(fields[0], null, DateTimeStyles.RoundtripKind ),
                long.Parse(fields[2]));
        }
    }
}