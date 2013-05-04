// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.IO;
using ManiaX.Beacons;
using NUnit.Framework;
using System.Collections.Generic;


namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestCSVStateRepository
    {
        private DateTime SOME_TIMESTAMP = DateTime.Parse("2010-08-23 08:54:00.000");
        private CsvStateRepository _repository;

        [SetUp]
        public void BeforeEachTest()
        {
            FileIOHelper.CreateEmptyTempFolder();
            
            _repository = new CsvStateRepository(FileIOHelper.TempFolderPath);
        }

        [TestFixtureTearDown]
        public void AfterAllTests()
        {
            FileIOHelper.DeleteTempFolderIfItExists();
        }

        [Test]
        public void LogsCodebaseTransitionsToCSVFile()
        {
            _repository.LogTransition(SOME_TIMESTAMP, CodebaseState.Unknown, 2000);
            _repository.LogTransition(SOME_TIMESTAMP.AddSeconds(2), CodebaseState.Compiling, 500);
            _repository.LogTransition(SOME_TIMESTAMP.AddSeconds(2.5), CodebaseState.CompileErrors, 4500);
            _repository.LogTransition(SOME_TIMESTAMP.AddSeconds(7), CodebaseState.Compiling, 1000);
            _repository.LogTransition(SOME_TIMESTAMP.AddSeconds(8), CodebaseState.NoCompileErrors, 1000);

            string[] sessionFiles = Directory.GetFiles(FileIOHelper.TempFolderPath);
            Assert.That(sessionFiles.Length, Is.EqualTo(1));

            string[] expectedFileContents = new string[]
                                                {
                                                    "2010-08-23T08:54:00.0000000+05:30,Unknown,2000",
                                                    "2010-08-23T08:54:02.0000000+05:30,Compiling,500",
                                                    "2010-08-23T08:54:02.5000000+05:30,CompileErrors,4500",
                                                    "2010-08-23T08:54:07.0000000+05:30,Compiling,1000",
                                                    "2010-08-23T08:54:08.0000000+05:30,NoCompileErrors,1000",
        };
            Assert.That(File.ReadAllLines(sessionFiles[0]), Is.EqualTo(expectedFileContents) );
        }

        [Test]
        public void CreatesNewLogFilesPerInstance()
        {
            FileIOHelper.DeleteTempFolderIfItExists();
            FileIOHelper.CreateEmptyTempFolder();

            new CsvStateRepository(FileIOHelper.TempFolderPath).LogTransition(SOME_TIMESTAMP, CodebaseState.Unknown, 1000);
            new CsvStateRepository(FileIOHelper.TempFolderPath).LogTransition(SOME_TIMESTAMP, CodebaseState.Unknown, 1000);
            new CsvStateRepository(FileIOHelper.TempFolderPath).LogTransition(SOME_TIMESTAMP, CodebaseState.Unknown, 1000);

            Assert.That(Directory.GetFiles(FileIOHelper.TempFolderPath).Length, Is.EqualTo(3));
        }

        
        [Test]
        public void CanRetrieveAllStatesOnDemand()
        {
            _repository.LogTransition(SOME_TIMESTAMP, CodebaseState.Unknown, 2000);
            _repository.LogTransition(SOME_TIMESTAMP.AddSeconds(2), CodebaseState.Compiling, 500);

            Assert.That(_repository.GetTransitions(), Is.EqualTo(
                new List<StateTimeSpan>
                    {
                        new StateTimeSpan(CodebaseState.Unknown, SOME_TIMESTAMP, 2000),
                        new StateTimeSpan(CodebaseState.Compiling, SOME_TIMESTAMP.AddSeconds(2), 500)
                    }));
        }

        [Test]
        public void WillRetrieveEmptyListIfNoStateHasBeenPersistedYet()
        {
            Assert.That(_repository.GetTransitions(), Is.Empty);
        }
    }
}
