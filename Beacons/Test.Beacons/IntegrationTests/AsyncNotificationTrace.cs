// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    public interface Matcher<in T>
    {
        bool Matches(T actual);
        string Description { get; }
    }
    public class AsyncNotificationTrace 
    {
        private readonly string _description;
        private readonly List<EventArgs> _receivedNotifications = new List<EventArgs>();

        public AsyncNotificationTrace(string description)
        {
            _description = description;
        }

        public void Handler(object sender, EventArgs e)
        {
            lock(_receivedNotifications){
                _receivedNotifications.Add(e);
            }
        }

        public int ReceivedNotificationCount
        {
            get
            {
                lock (_receivedNotifications)
                {
                    return _receivedNotifications.Count;
                }
            }
        }

        internal class NotificationCountMatcher : Matcher<AsyncNotificationTrace>
        {
            private readonly int _expectedCount;

            public NotificationCountMatcher(int expectedCount)
            {
                _expectedCount = expectedCount;
            }

            public bool Matches(AsyncNotificationTrace actual)
            {

                int notificationsReceived = actual._receivedNotifications.Count;
                var result = (notificationsReceived == _expectedCount);
                if (!result)
                {
                    this.Description = String.Format("{0}{1} Expected {2} but was {3}", 
                                                     actual._description, Environment.NewLine,
                                                     _expectedCount, notificationsReceived);
                }
                return result;
            }

            public string Description { get; private set; }

            public Matcher<AsyncNotificationTrace> Notification
            {
                get { return this; }
            }
            public Matcher<AsyncNotificationTrace> Notifications
            {
                get { return this; }
            }
        }

        public void VerifyAfter(TimeSpan delay, Matcher<AsyncNotificationTrace> matcher)
        {
            Thread.Sleep(delay);
            if (!matcher.Matches(this))
                Assert.Fail(matcher.Description);
        }

        internal NotificationCountMatcher Received(int expectedNotifications)
        {
            return new NotificationCountMatcher(expectedNotifications);
        }

        public EventArgs VerifyEventArgsAt(int index)
        {
            return _receivedNotifications[index];
        }
    }
}