using System;
using System.Windows.Threading;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public class WpfDispatcher : UiUpdateDispatcher
    {
        private readonly Dispatcher _dispatcher;
        public WpfDispatcher()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void UpdateOnOwnerThread(Action updateAction)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.Invoke(updateAction);
                return;
            }
            updateAction();
        }
    }

}