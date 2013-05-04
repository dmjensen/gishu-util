using System;
using System.Runtime.InteropServices;
using ManiaX.Beacons.Roles;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ManiaX.Beacons
{
    [Guid(GuidList.guidBeaconsPkgString)]
    internal class BeaconsTaskProvider : ErrorListProvider, UserNotifier
    {
        public BeaconsTaskProvider(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public void PostWarning(string message)
        {
            AddEntryToList(message, TaskErrorCategory.Warning);
        }

        public void PostMessage(string message)
        {
            AddEntryToList(message, TaskErrorCategory.Message);
        }

        private void AddEntryToList(string errorMessage, TaskErrorCategory entryType)
        {
            Tasks.Insert(0,
                         new ErrorTask
                         {
                             Category = TaskCategory.User,
                             ErrorCategory = entryType,
                             Text = String.Format("{0} {1}", DateTime.Now.ToString("u"), errorMessage)
                         });

            SelectLatestEntry();
        }

        private void SelectLatestEntry()
        {
            var notificationView = GetService(typeof(SVsTaskList)) as IVsTaskList2;

            var providerId = this.GetType().GUID;
            notificationView.SetActiveProvider(ref providerId);

            notificationView.SelectItems(1, new[] { Tasks[0] }, (uint)__VSTASKLISTSELECTIONTYPE.TST_REPLACESEL, 0);
        }
    }
}
