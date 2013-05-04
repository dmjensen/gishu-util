// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;

namespace ManiaX.Test.Beacons.Infrastructure
{
    class NotificationListener
    {
        private bool _notificationReceived;

        public bool NotificationReceived
        {
            get { return _notificationReceived; }
        }

        public void Handler(object sender, EventArgs e)
        {
            _notificationReceived = true;
        }
    }
}