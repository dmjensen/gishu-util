using System;

namespace ManiaX.Beacons.Roles
{
    /// <summary>
    /// Abstracts the Wpf UI Thread affinity deal 
    /// i.e. Wpf UI can only be updated on the thread they are created on
    /// </summary>
    public interface UiUpdateDispatcher
    {
        void UpdateOnOwnerThread(Action updateAction);
    }
}