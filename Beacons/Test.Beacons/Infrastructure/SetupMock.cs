// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using Moq;

namespace ManiaX.Test.Beacons.Infrastructure
{
    public static class SetupMock
    {
        public static void ClockToReturnCurrentTimesAs(Mock<SystemClock> mockClock, params DateTime[] times)
        {
            mockClock.Setup(clock => clock.GetCurrentTime()).ReturnsNextValueFrom(times);
        }

        public static void ClockToReturnCurrentTimeAs(Mock<SystemClock> mockClock, DateTime time)
        {
            mockClock.Setup(clock => clock.GetCurrentTime()).Returns(time);
        }

        public static Mock<TestRunner> TestRunnerFactoryToCreateRunnerFor(RunnerType runnerType, string commandLine,        
                                                                            Mock<TestRunnerFactory> mockFactory)
        {
            var mockTestRunner = new Mock<TestRunner>();
            mockFactory.Setup(factory => factory.CreateTestRunner(runnerType, commandLine))
                .Returns(mockTestRunner.Object);
            return mockTestRunner;
        }
    }
}