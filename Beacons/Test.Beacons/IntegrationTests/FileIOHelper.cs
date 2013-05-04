// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    public class FileIOHelper
    {
        static FileIOHelper()
        {
            __origin = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
        }
        private static string _tempFolderPath;
        private static string __origin;

        public static string TempFolderPath
        {
            get { return _tempFolderPath; }
        }

        public static void CreateEmptyTempFolder()
        {
            _tempFolderPath = Path.Combine(__origin, "Temp");
            DeleteTempFolderIfItExists();
            Directory.CreateDirectory(_tempFolderPath);
            Assert.True(Directory.Exists(_tempFolderPath));
        }
        public static void DeleteTempFolderIfItExists()
        {
            if (Directory.Exists(_tempFolderPath))
                Directory.Delete(_tempFolderPath, true);
        }
        public static string GetResourceFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(__origin, @"../../TestResources", fileName));
        }
    }
}