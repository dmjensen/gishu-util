// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.IO;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestDefaultTestRunnerFactory
    {
        [SetUp]
        public void BeforeEachTest()
        {
            FileIOHelper.CreateEmptyTempFolder();
        }
        [TearDown]
        public void AfterEachTest()
        {
            FileIOHelper.DeleteTempFolderIfItExists();
        }

        [Test]
        public void CanCreateNUnitResultsFileWatcher()
        {
            TestRunnerFactory factory = new DefaultTestRunnerFactory();

            using (var testRunner = factory.CreateTestRunner(
                                        RunnerType.NUnitResultsFileWatcher, FileIOHelper.TempFolderPath))
            {
                Assert.That(testRunner,  Is.InstanceOf(typeof(NUnitResultsWatcher)));
            }
        }

        [Test]
        public void CanCreateNUnitConsoleRunner()
        {
            TestRunnerFactory factory = new DefaultTestRunnerFactory();

            using (var testRunner = factory.CreateTestRunner(
                                        RunnerType.NUnitConsole, ""))
            {
                Assert.That(testRunner, Is.InstanceOf(typeof(NUnitConsoleRunner)));
            }
        }
    }
}
