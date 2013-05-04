// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;

namespace ManiaX.Beacons
{
    public class NUnitConsoleRunner : NUnitTestRunnerBase
    {
        private readonly ProcessStartInfo _processInfo;
        private readonly string _testResultsFilePath;

        public NUnitConsoleRunner(string commandLine)
        {
            try
            {
                //TODO: Remove this - validate commandline upstream
                var nunitconsoleIndex = commandLine.IndexOf("nunit-console");
                if (nunitconsoleIndex < 0)
                {   Logger.Log(commandLine + " : does not include nunit-console"); 
                    return;
                }

                var partitionPos = commandLine.IndexOf(' ', nunitconsoleIndex);
                var nunitPath = commandLine.Substring(0, partitionPos);
                var args = commandLine.Replace(nunitPath, "");
                _testResultsFilePath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Beacons\TestResult.xml");

                _processInfo = new ProcessStartInfo
                                   {
                                       FileName = nunitPath,
                                       Arguments = args,
                                       RedirectStandardError = true,
                                       UseShellExecute = false,
                                       CreateNoWindow = true,
                                       WorkingDirectory = Path.GetDirectoryName(_testResultsFilePath)
                                   };
            }
            catch(Exception e)
            {
                Logger.Log(e.ToString());
            }
            
        }

        public override void RunTests()
        {
            //TODO: Remove this - validate commandline upstream
            if (_processInfo == null)
                return;

            if (File.Exists(_testResultsFilePath))
                File.Delete(_testResultsFilePath);

            try
            {
                var process = Process.Start(_processInfo);
                if (!process.WaitForExit(15000))
                {
                    Logger.Log("Console Runner timed out after 15 secs");
                    return;
                }
                Logger.Log(_processInfo.Arguments + " finished with exit code = " + process.ExitCode);
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
                return;
            }

            using (var fs = new FileStream(_testResultsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                NotifyTestResults(XElement.Load(fs));
            }
        }

        public override void Dispose()
        {}
    }
}