using System;
using System.Collections.Generic;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using Moq;

namespace ManiaX.Test.Beacons.Infrastructure
{
    public static class ObjectMother
    {
        public class MockDependenciesCollection
        {
            private Dictionary<Type, Mock> _listOfDependencies;

            public MockDependenciesCollection(Dictionary<Type, Mock> listOfDependencies)
            {
                _listOfDependencies = listOfDependencies;
            }
            
            public Mock<T> Get<T>() where T : class
            {
                return _listOfDependencies[typeof(T)] as Mock<T>;
            }
        }

        public static CodebaseStateTracker GetCodebaseStateTracker(out MockDependenciesCollection mockDependencies)
        {
            Dictionary<Type, Mock> listOfDependencies;
            var mockIDE = new Mock<IDE>();
            var mockStateRepository = new Mock<StateRepository>();
            var mockTimer = new Mock<TransitionTimer>();

            listOfDependencies = new Dictionary<Type, Mock>
                                                  {
                                                      {typeof(IDE), mockIDE},
                                                      {typeof(StateRepository), mockStateRepository},
                                                      {typeof(TransitionTimer), mockTimer},
                                                  };

            var codebaseStateTracker = new CodebaseStateTracker( mockIDE.Object, mockStateRepository.Object, mockTimer.Object);
            var stateTracker = codebaseStateTracker;
            mockDependencies = new MockDependenciesCollection(listOfDependencies);
            return stateTracker;
        }
    }
}