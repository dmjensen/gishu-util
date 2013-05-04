// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.IO;
using System.Reflection;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestWindowsFileSystem
    {
        [Test]
        public void ChecksIfFolderInPathExists()
        {
            var validFilePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var validFolderPath = Path.GetDirectoryName(validFilePath);
            var nonExistentFolderPath = Path.Combine(validFolderPath, @"Wackow2000");

            FileSystem fs = new WindowsFileSystem();

            Assert.IsTrue(fs.FolderExists(validFolderPath), "should return true if parent folder exists");
            Assert.IsFalse(fs.FolderExists(nonExistentFolderPath),
                            "should return false if folder does not exist");
            Assert.IsFalse(fs.FolderExists(validFilePath), "should return false if file path is passed");
        }

    }
}
