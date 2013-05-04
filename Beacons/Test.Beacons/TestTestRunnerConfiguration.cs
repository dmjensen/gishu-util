// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.Collections.Generic;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace ManiaX.Test.Beacons
{
    [TestFixture]
    public class TestTestRunnerConfiguration
    {
        private Mock<TestRunnerFactory> _mockRunnerFactory;
        private TestRunnerProvider _testRunnerProvider;
        private Mock<FileSystem> _mockFileSystem;
        private TestRunnerConfiguration _configurationVM;
        
        private const string FILE_PATH_FORMATSTRING = @"h:\code\beacons\Beacons\TestResults{0}.xml";
        private static readonly string FILE_PATH1 = string.Format(FILE_PATH_FORMATSTRING, 1);
        private static readonly string FILE_PATH2 = string.Format(FILE_PATH_FORMATSTRING, 2);
        private static readonly string FILE_PATH3 = string.Format(FILE_PATH_FORMATSTRING, 3);
        private static readonly string FILE_PATH4 = string.Format(FILE_PATH_FORMATSTRING, 4);
        private static readonly string FILE_PATH5 = string.Format(FILE_PATH_FORMATSTRING, 5);

        [SetUp]
        public void BeforeEachTest()
        {
            _mockRunnerFactory = new Mock<TestRunnerFactory>();
            _testRunnerProvider = new TestRunnerProvider(_mockRunnerFactory.Object);
            _mockFileSystem = new Mock<FileSystem>();
            _configurationVM = new TestRunnerConfiguration(_testRunnerProvider, _mockFileSystem.Object);

            _mockFileSystem.Setup(fs => fs.FolderExists(TestConstants.A_TESTRESULTS_FILEPATH)).Returns(true);
        }

        [Test]
        public void ConfiguresTestRunner()
        {
            var testRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(TestConstants.A_TESTRESULTS_FILEPATH, _mockRunnerFactory);

            _configurationVM.TestResultsFilePath = TestConstants.A_TESTRESULTS_FILEPATH;

            Assert.That(_testRunnerProvider.UnitTestRunner, Is.SameAs(testRunner.Object), "provider should now be exposing a valid test runner");
            Assert.That(_configurationVM["TestResultsFilePath"], Is.Null, "should not be showing any validation errors");
        }

        [Test]
        public void DisposesExistingRunnerIfReconfigured()
        {
            const string ANOTHER_RESULTS_FILEPATH = @"c:\project2\results.xml";
            var testRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(TestConstants.A_TESTRESULTS_FILEPATH, _mockRunnerFactory);
            _configurationVM.TestResultsFilePath = TestConstants.A_TESTRESULTS_FILEPATH;

            SetupMock.TestRunnerFactoryToCreateRunnerFor(ANOTHER_RESULTS_FILEPATH, _mockRunnerFactory);
            _mockFileSystem.Setup(fs => fs.FolderExists(ANOTHER_RESULTS_FILEPATH)).Returns(true);
            
            _configurationVM.TestResultsFilePath = ANOTHER_RESULTS_FILEPATH;

            testRunner.Verify(runner => runner.Dispose(), Times.Once(), "existing runner should have been disposed before switching to the new one");
        }

        [Test]
        public void ShowsValidationErrorIfSpecifiedResultsFilePathDoesNotExist()
        {
            _mockFileSystem.Setup(fs => fs.FolderExists(TestConstants.NON_EXISTENT_FILEPATH)).Returns(false);

            _configurationVM.TestResultsFilePath = TestConstants.NON_EXISTENT_FILEPATH;

            Assert.That(_configurationVM["TestResultsFilePath"], Is.EqualTo("This folder does not exist."), 
                "should warn user that he might have made a mistake");
        }

        [TestCase(null, TestName = "Null file path should be rejected")]
        [TestCase("   ", TestName = "Blank file path should be rejected")]
        public void IgnoresNullOrEmptyFilePaths(string resultsFilepath)
        {
            _configurationVM.TestResultsFilePath = resultsFilepath;

            Assert.That(_configurationVM["TestResultsFilePath"], Is.EqualTo("Enter the path of file containing test results."),
                "prompt user to enter a results file path");
            Assert.That(_testRunnerProvider.UnitTestRunner, Is.Null);
        }

        [TestCase(null, TestName = "Invalid - null file path set")]
        [TestCase("   ", TestName = "Invalid - blank file path set")]
        [TestCase("ThisPathDoesNotExist", TestName = "Invalid - specified file path does not exist")]
        public void ClearsExistingRunnerIfInvalidFilepathIsSet(string resultsFilepath)
        {
            SetupMock.TestRunnerFactoryToCreateRunnerFor(TestConstants.A_TESTRESULTS_FILEPATH,
                                                                              _mockRunnerFactory);
            _configurationVM.TestResultsFilePath = TestConstants.A_TESTRESULTS_FILEPATH;

            _configurationVM.TestResultsFilePath = resultsFilepath;

            Assert.That(_testRunnerProvider.UnitTestRunner, Is.Null, "existing runner should have been cleared off!");
        }

        [Test]
        public void ConfiguringTestRunnerAddsItToMRUStack()
        {
            foreach(var path in new[] {TestConstants.A_TESTRESULTS_FILEPATH, FILE_PATH1, FILE_PATH2, FILE_PATH3, FILE_PATH4, FILE_PATH5})
            {
                _mockFileSystem.Setup(fs => fs.FolderExists(path)).Returns(true);
                _configurationVM.TestResultsFilePath = path;
            }

            Assert.That(_configurationVM.MRUPaths.Count, Is.EqualTo(5), "MRU list should contain max 5 elements");
            Assert.That(_configurationVM.MRUPaths[0], Is.EqualTo(FILE_PATH5), "Top of MRU should be recently used path");
            Assert.That(_configurationVM.MRUPaths[4], Is.EqualTo(FILE_PATH1), "End of MRU should be oldest used path");
        }

        [Test]
        public void NotifiesChangeInCollectionMRUPaths()
        {
            var addedPathsCount = 0;
            _configurationVM.MRUPaths.CollectionChanged += (sender, args) => addedPathsCount = args.NewItems.Count;

            _configurationVM.TestResultsFilePath = TestConstants.A_TESTRESULTS_FILEPATH;

            Assert.That(addedPathsCount, Is.EqualTo(1), "Should have notified observers of change in MRUPaths");
        }

        [Test]
        public void PromoteFilepathToTopOfMRUStack_IfExistingMRUItemIsUsedAgain()
        {
            foreach (var path in new[] { FILE_PATH1, FILE_PATH2, FILE_PATH3, FILE_PATH4, FILE_PATH5, FILE_PATH3 })
            {
                _mockFileSystem.Setup(fs => fs.FolderExists(path)).Returns(true);
                _configurationVM.TestResultsFilePath = path;
            }
            Assert.That(_configurationVM.MRUPaths.Count, Is.EqualTo(5), "MRU should contain 5 unique entries");
            Assert.That(_configurationVM.MRUPaths[0], Is.EqualTo(FILE_PATH3), "");
            Assert.That(_configurationVM.MRUPaths.Where(path => path.Equals(FILE_PATH3)).Count(), 
                Is.EqualTo(1), "Mru must not contain duplicates");
        }
    }
}
