// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
﻿using System.IO;
﻿using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace ManiaX.Beacons
{
    public class NUnitResultsWatcher : NUnitTestRunnerBase
    {
        private readonly string _testResultsFilePath;
        private FileSystemWatcher _watcher;
        private bool _isDisposed;
        private string _timeOfLastTestRun;

        public NUnitResultsWatcher(string testResultsFilePath)
        {
            const string DEFAULT_NUNIT_RESULTS_FILENAME = "TestResult.xml";
            _testResultsFilePath = Path.Combine(testResultsFilePath, DEFAULT_NUNIT_RESULTS_FILENAME);
            _watcher = new FileSystemWatcher(testResultsFilePath)
                           {
                               Filter = DEFAULT_NUNIT_RESULTS_FILENAME,
                               NotifyFilter = NotifyFilters.LastWrite
                           };

            _watcher.Changed += ParseResultsFileAndNotify;
            _watcher.EnableRaisingEvents = true;
        }

        override public void RunTests()
        {}

        override public void Dispose()
        {
            Dispose(true);
        }

        private void ParseResultsFileAndNotify(object sender, FileSystemEventArgs e)
        {
            int tries = 0;
            while (tries < 5)
            {
                try
                {
                    using (var fileStream = new FileStream(_testResultsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var root = XElement.Load(fileStream);
                        if (IsDuplicateCallbackForATestRun(root))
                            return;

                        NotifyTestResults(root);
                        _timeOfLastTestRun = GetTimeOfLastTestRun(root);
                        break;
                    }

                }
                catch (XmlException) { /* filewatcher raises callbacks even if the file write hasn't completed*/ }
                catch (IOException){ /*nunit might have a lock on the file at the time of the callback*/ }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(Beacons.OutputFolderPath, "Beacons.log"),
                                        String.Format("{2} - Error! : {0}{1}", ex, Environment.NewLine, DateTime.Now.ToString("u")));
                    Console.WriteLine(ex);
                }
                tries++;
                Thread.Sleep(500);
            }
        }

        private bool IsDuplicateCallbackForATestRun(XElement root)
        {
            return _timeOfLastTestRun == GetTimeOfLastTestRun(root);
        }

        ~NUnitResultsWatcher()
        {
            Dispose(false);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            _watcher.Dispose();
            _watcher = null;

            _isDisposed = true;

            if (isDisposing)
                GC.SuppressFinalize(this);
        }
    }

    
}