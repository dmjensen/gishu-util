// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

namespace ManiaX.Beacons.Roles
{
    public interface UserNotifier
    {
        void PostWarning(string message);
        void PostMessage(string message);
    }
}