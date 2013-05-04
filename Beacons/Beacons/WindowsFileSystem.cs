// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System.IO;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public class WindowsFileSystem : FileSystem
    {
        public bool FolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }
    }
}