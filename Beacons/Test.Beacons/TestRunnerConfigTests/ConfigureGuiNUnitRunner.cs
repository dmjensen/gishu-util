// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using System.Linq;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TestRunnerConfigTests
{
    [TestFixture]
    public class ConfigureGuiNUnitRunner : TestSpecificRunnerConfigBase
    {
        [SetUp]
        public void BeforeEachTest()
        {
            _mockFileSystem.Setup(fs => fs.FolderExists(TestConstants.A_TESTRESULTS_FOLDERPATH)).Returns(true);
        }

        [Test]
        public void ShowsValidationErrorIfSpecifiedResultsFilePathDoesNotExist()
        {
            _mockFileSystem.Setup(fs => fs.FolderExists(TestConstants.NON_EXISTENT_FILEPATH)).Returns(false);

            _configurationVM.SelectedRunnerType = RunnerType.NUnitResultsFileWatcher;
            _configurationVM.ConfigArguments = TestConstants.NON_EXISTENT_FILEPATH;

            Assert.That(_configurationVM["ConfigArguments"], Is.EqualTo("This folder does not exist."), 
                        "should warn user that he might have made a mistake");
        }

        protected override RunnerType GetRunnerType()
        {
            return RunnerType.NUnitResultsFileWatcher;
        }

        protected override string GetConfigArguments()
        {
            return TestConstants.A_TESTRESULTS_FOLDERPATH;
        }

        protected override string GetAnotherSetOfConfigArguments()
        {
            return @"c:\project2";
        }


        protected override void SetupAdditionalExpectations(string arguments)
        {
            _mockFileSystem.Setup(fs => fs.FolderExists(arguments)).Returns(true);
        }

        protected override string GetValidationErrorIfRunnerIsNotConfigured()
        {
            return "Enter path to folder containing NUnit test results";
        }

        protected override IEnumerable<TestCaseData> GetInvalidArguments()
        {
            return new List<TestCaseData>(base.GetInvalidArguments())
                       {new TestCaseData(null).SetName("Non Existent Results Folder path set")};
        }

        protected override IEnumerable<string> GetFiveValidConfigurations()
        {
            const string FOLDER_PATH_FORMATSTRING = @"h:\code\beacons\Beacons{0}";
            var FOLDER1 = string.Format(FOLDER_PATH_FORMATSTRING, 1);
            var FOLDER2 = string.Format(FOLDER_PATH_FORMATSTRING, 2);
            var FOLDER3 = string.Format(FOLDER_PATH_FORMATSTRING, 3);
            var FOLDER4 = string.Format(FOLDER_PATH_FORMATSTRING, 4);
            var FOLDER5 = string.Format(FOLDER_PATH_FORMATSTRING, 5);

            return new[] { FOLDER1, FOLDER2, FOLDER3, FOLDER4, FOLDER5 };
        }
    }
}
