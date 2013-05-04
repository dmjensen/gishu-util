using System;
using ManiaX.Beacons.Roles;

namespace ManiaX.Test.Beacons
{
    public class MockUiUpdater : UiUpdateDispatcher
    {
        public void UpdateOnOwnerThread(Action updateAction)
        {
            updateAction();
        }
    }
}