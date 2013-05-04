using System;
using System.Collections.Generic;
using System.Linq;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Test.Beacons.Infrastructure;
using ManiaX.Test.Beacons.StateBeaconTests;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconRutDetection
{
    public class TestStateBeaconRutDetectionBase : TestStateBeaconBase
    {
        private List<StateTimeSpan> _pastTransitions;

        protected readonly Tuple<CodebaseState, int>[] TRANSITIONS_FOR_BROKEN_BUILDS_RUT = new[]{
                                Tuple.Create(CodebaseState.NoCompileErrors,5),
                                Tuple.Create( CodebaseState.Compiling, 10),
                                Tuple.Create( CodebaseState.CompileErrors, 51)};

        protected Mock<TestRunner> _mockTestRunner;

        protected readonly Tuple<CodebaseState, int>[] TRANSITIONS_FOR_BROKEN_TESTS_RUT = new[]{
                                                                                                 Tuple.Create(CodebaseState.Compiling, 10),
                                                                                                 Tuple.Create(CodebaseState.NoCompileErrors, 30),
                                                                                                 Tuple.Create(CodebaseState.Green, 10),
                                                                                                 Tuple.Create(CodebaseState.Compiling, 10),
                                                                                                 Tuple.Create(CodebaseState.NoCompileErrors, 10),
                                                                                                 Tuple.Create(CodebaseState.Red, 10),
                                                                                                 Tuple.Create(CodebaseState.Compiling, 31)};

        [SetUp]
        public void SetThresholdToOneMinute()
        {
            _stateBeacon.RutThreshold = TimeSpan.FromMinutes(1);

            _pastTransitions = new List<StateTimeSpan>();
            _mockStateRepository.Setup(repo => repo.GetTransitions())
                                    .Returns(_pastTransitions);
        }

        protected void AppendTransitions(params Tuple<CodebaseState, int>[] pastStates)
        {
            
            var timestamp = _pastTransitions.Count == 0 
                                ? DateTime.Parse("2010-08-24 15:00:00") 
                                : _pastTransitions.Last().StartTime.AddMilliseconds(_pastTransitions.Last().DurationInMilliSecs);

            foreach(var state in pastStates)
            {    
                _pastTransitions.Add(new StateTimeSpan(state.Item1, timestamp, state.Item2 * 1000));
                timestamp = timestamp.AddSeconds(state.Item2);
            }
        }

        protected static object[] WrapInObjectArray(params Tuple<CodebaseState, int>[] transitions)
        {
            return new object[]{transitions};
        }

        protected void SimulateABrokenBuildsRut()
        {
            AppendTransitions(TRANSITIONS_FOR_BROKEN_BUILDS_RUT);
            SimulateA.BuildFailure(_mockIDE);
        }

        protected void SimulateABrokenTestsRut()
        {
            AppendTransitions( TRANSITIONS_FOR_BROKEN_TESTS_RUT);
            SimulateA.TestFailure(_mockIDE, _mockTestRunner);
        }
    }
}