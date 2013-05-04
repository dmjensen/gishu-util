// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System.Collections.Generic;
using ManiaX.Beacons.DataStructs;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.DataStructs
{
    [TestFixture]
    public class TestTestRun
    {
        [Test]
        public void NotifiesChangeIn_Note()
        {
            var run = new TestRun {TestCount = 10, Result = TestResult.RefactoringWin};
            var listener = new PropertyChangeListener(run);
            
            run.Note = "Woo hoo!";

            Assert.That(listener.HasReceivedChangeNotificationFor("Note"));
        }

    }
}
