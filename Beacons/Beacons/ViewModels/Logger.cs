using System;
using System.IO;
using System.Threading;

namespace ManiaX.Beacons.ViewModels
{
    public static class Logger
    {
        public static void Log(string message)
        {
            File.AppendAllText(Path.Combine(Beacons.OutputFolderPath, "Beacons.log"),
                               String.Format("T={0,-3} @ {1} {2} {3}",
                                             Thread.CurrentThread.ManagedThreadId,
                                             DateTime.Now.ToString("o"),
                                             message ,
                                             Environment.NewLine));
        }
    }
}