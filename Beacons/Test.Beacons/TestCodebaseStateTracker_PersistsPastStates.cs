// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using ManiaX.Beacons;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_PersistsPastStates : TestCodebaseStateTrackerBase
    {
        private uint SOME_DURATION = 2000;

        [Test]
        public void LogsStateChangeFromUnknownToCompiling()
        {
            ExpectLogEntry(CodebaseState.Unknown, SOME_DURATION);

            _mockIDE.Raise(m => m.SolutionOpened += null, EventArgs.Empty);
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            
            _mockTimer.VerifyAll();
            _mockStateRepository.VerifyAll();            
        }

        [Test]
        public void DoesNotLogStateChangeIfNewStateSameAsOld()
        {
            _mockIDE.Raise(m => m.SolutionOpened += null, EventArgs.Empty);
            
            _mockIDE.Raise(m => m.SolutionClosed += null, EventArgs.Empty);

            _mockTimer.Verify(t => t.GetElapsedMillisecAndRestart(), Times.Never());
            _mockStateRepository.Verify(l => l.LogTransition(It.IsAny<DateTime>(), It.IsAny<CodebaseState>(), It.IsAny<uint>()),
                Times.Never());

        }

        [Test]
        public void LogsStateChangeFromCompilingToNoCompileErrors()
        {
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            ExpectLogEntry(CodebaseState.Compiling, SOME_DURATION);

            _mockIDE.Raise(m => m.BuildSucceeded += null, EventArgs.Empty);

            _mockTimer.VerifyAll();
            _mockStateRepository.VerifyAll();
            
        }

        [Test]
        public void LogsStateChangeFromCompilingToCompileErrors()
        {
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            ExpectLogEntry(CodebaseState.Compiling, SOME_DURATION);

            _mockIDE.Raise(m => m.BuildFailed += null, EventArgs.Empty);

            _mockTimer.VerifyAll();
            _mockStateRepository.VerifyAll();

        }

        [Test]
        public void LogsLastStateOnDispose()
        {
            SetUpBuildBrokenState(_mockIDE);
            ExpectLogEntry(CodebaseState.CompileErrors, SOME_DURATION);

            _codebaseStateTracker.Dispose();

            _mockTimer.VerifyAll();
            _mockStateRepository.VerifyAll();
        }

        [Test]
        public void CanRetrieveAllPastStates()
        {
            var aStateSequence = new List<StateTimeSpan>
                                    {
                                        new StateTimeSpan(CodebaseState.Unknown,
                                                  DateTime.Parse("1900-01-01 01:01:01.000"),
                                                  1000),
                                        new StateTimeSpan(CodebaseState.Unknown,
                                                  DateTime.Parse("1900-01-01 01:01:02.000"),
                                                  4000)
                                    };
            _mockStateRepository.Setup(repository => repository.GetTransitions()).Returns(
                aStateSequence);

            Assert.That(_codebaseStateTracker.GetTransitions(), Is.EqualTo(aStateSequence));
            _mockStateRepository.VerifyAll();
        }
    }
}
