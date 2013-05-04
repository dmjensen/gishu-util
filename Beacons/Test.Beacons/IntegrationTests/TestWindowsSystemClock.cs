// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestWindowsSystemClock
    {
        [Test]
        public void ReturnsCurrentTime()
        {
            Assert.That(DateTime.Now.Subtract(new WindowsSystemClock().GetCurrentTime()),
                          Is.LessThan(TimeSpan.FromSeconds(2)),
                          "WindowsSystemClock returned a time which is off from the current time by 2 secs or more");
        }
    }
}