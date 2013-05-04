// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.IO;

namespace ManiaX.Beacons
{
    public class Beacons
    {
        public static string OutputFolderPath
        {
            get
            {
                var outputFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Beacons");
                if (!Directory.Exists(outputFolderPath))
                    Directory.CreateDirectory(outputFolderPath);
                return outputFolderPath;
            }
            
        }
    }
}