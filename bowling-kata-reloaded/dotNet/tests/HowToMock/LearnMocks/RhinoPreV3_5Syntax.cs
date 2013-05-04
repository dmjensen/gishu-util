//using System;
//using System.Collections.Generic;
//using DummyTestSubject;
//using NUnit.Framework;
//using Rhino.Mocks;
//using Rhino.Mocks.Constraints;
//using Rhino.Mocks.Interfaces;

//namespace LearnRhinoMocks
//{
//   [TestFixture]
//   public class Rhino101
//   {
//      private readonly string DUMMY_NAME = "IDummy";
//      private readonly int DUMMY_BALANCE = 1010;

//      private MockRepository _mocksRepo;
//      private IBank _mockBank;
//      private IPrinter _mockPrinter;
//      private ILog _mockLogger;
//      private CrashTestDummy _testSubject;

//      [SetUp]
//      public void BeforeEachTest()
//      {
//         _mocksRepo = new MockRepository();
//         _mockBank = _mocksRepo.StrictMock<IBank>();
//         //_mockPrinter = _mocksRepo.StrictMock<IPrinter>();
//         _mockPrinter = _mocksRepo.DynamicMock<IPrinter>();            // explain diff - strict mocks are discouraged - lead to fragile tests
//         _mockLogger = _mocksRepo.StrictMock<ILog>();
         
//         _mocksRepo.ReplayAll();                                                        // temporarily suspend record mode so that event subscription is not 'recorded'
//         _testSubject = new CrashTestDummy(DUMMY_NAME, _mockPrinter, _mockLogger);
//         _mocksRepo.BackToRecordAll(BackToRecordOptions.None);
//      }

//      [TearDown]
//      public void AfterEachTest()
//      {
//         _mocksRepo.ReplayAll(); // 2nd call to ReplayAll does nothing. Safeguard check
//         _mocksRepo.VerifyAll();
//      }

//      [Test]
//      public void Test_ExpectMethodCall()
//      {
//         var mocksRepo = new MockRepository();
//         var mockBank = mocksRepo.StrictMock<IBank>();
//         var mockPrinter = mocksRepo.DynamicMock<IPrinter>();
//         var mockLogger = mocksRepo.DynamicMock<ILog>();

//         // expectation on method that returns something
//         Expect.Call(mockBank.GetBalance(DUMMY_NAME)).Return(DUMMY_BALANCE);
//         // expectation on method that returns void
//         mockPrinter.Print(DUMMY_BALANCE.ToString());

//         mocksRepo.ReplayAll();
//         var testSubject = new CrashTestDummy(DUMMY_NAME, mockPrinter, mockLogger);

//         testSubject.PrintBalance(DUMMY_NAME, mockBank);

//         mocksRepo.VerifyAll();
//      }

//      [Test]
//      public void Test_ConstrainingArguments()
//      {
//         _mockPrinter.Print(null);
//         LastCall.Constraints(Text.StartsWith("The current date is : "));
//         _mocksRepo.ReplayAll();

//         _testSubject.PrintDate();
//      }

//      [Test]
//      public void Test_ComplexConstraints()
//      {
//         var expectedInitProps = new List<PropValue>{ 
//            new PropValue{ PropName="Machine", Value="bn-euro0013"},
//            new PropValue{ PropName="UserName", Value="Gishu"},
//            new PropValue{ PropName="Password", Value="pwd"}
//            };
//         _mockPrinter.Initialize(null);
//         LastCall.Constraints(Is.Matching<List<PropValue>>(x => CheckList(expectedInitProps, x)))
//            .Message("Initialization Properties do not match");

//         //_mockPrinter.Initialize(Arg<List<PropValue>>.List.ContainsAll(expectedInitProps));

//         _mocksRepo.ReplayAll();

//         _testSubject.Initialize();
//      }

//      private static bool CheckList(List<PropValue> expected, List<PropValue> actual)
//      {
//         //Console.WriteLine("Expected!");
//         //foreach (var p in expected)
//         //   Console.WriteLine(p);
//         //Console.WriteLine("Actual!");
//         //foreach (var p in actual)
//         //   Console.WriteLine(p);

//         if (expected.Count != actual.Count)
//         {
//            return false;
//         }

//         for (int iLooper = 0; iLooper < expected.Count; iLooper++)
//         {
//            var expectedPair = expected[iLooper];
//            var actualPair = actual[iLooper];

//            if ((expectedPair.PropName != actualPair.PropName)
//               || (expectedPair.Value != actualPair.Value))
//            {
//               Console.WriteLine(
//                  String.Format("Expected {0} Got {1}", expectedPair.ToString(), actualPair.ToString()));
//               return false;
//            }
//         }

//         return true;
//      }

//      [Test]
//      public void Test_MethodShouldNotBeCalled()
//      {
//         _mockPrinter.Print(null);
//         LastCall.IgnoreArguments()
//            .Repeat.Never()
//            .Message("Should not be called - Printer is on DND");
//         _mocksRepo.ReplayAll();

//         _testSubject.DoNotDisturbThePrinter();

//      }
//      [Test]
//      public void Test_ThrowExceptionFromAMock()
//      {
//         _mockPrinter.Print(null);
//         LastCall.IgnoreArguments()
//            .Throw(new ObjectDisposedException("Printer implementation"));
//         _mockLogger.LogMessage(null);
//         LastCall.Constraints(Text.Contains("ObjectDisposedException"));

//         _mocksRepo.ReplayAll();

//         _testSubject.PrintDate();

//      }

//      [Test]
//      public void Test_ExecuteCodeBlock_WhenAMethodIsCalled()
//      {
//         _mockPrinter.Print(null);
//         LastCall.IgnoreArguments()
//            .Throw(new ObjectDisposedException("Printer implementation"));
//         _mockLogger.LogMessage(null);
//         string lastLoggedMessage = null;
//         LastCall.IgnoreArguments()
//            .Do((Action<string>)delegate(string message)
//            {
//               lastLoggedMessage = message;
//               // can also do some validation
//            }
//               );

//         _mocksRepo.ReplayAll();

//         _testSubject.PrintDate();
//         Console.WriteLine(lastLoggedMessage);
//         Assert.IsTrue(lastLoggedMessage.Contains("ObjectDisposedException"));

//      }

//      //[Test]
//      //public void Test_RaisingAnEventFromAMock()
//      //{
//      //   var e = new MyEventArgs(DateTime.Now);

//      //   _mockPrinter.Torpedoed += null;
//      //   IEventRaiser raiser = LastCall.IgnoreArguments().GetEventRaiser();

//      //   _mockLogger.LogMessage(null);
//      //   LastCall.Constraints(Text.Contains(e.ToString()));
//      //   _mocksRepo.ReplayAll();

//      //   raiser.Raise(_mockPrinter, e);
//      //} 
//   }

//   // create new test fixture - do not add in setup code into previous fixture - Inappropriately shared fixture anti pattern
//   [TestFixture]
//   public class Rhino101_RaiseEvent
//   {
//      protected readonly string DUMMY_NAME = "IDummy";

//      private CrashTestDummy _testSubject;
      
//      private MockRepository _mocksRepo2;
//      private IPrinter _mockPrinter;
//      private ILog _mockLogger;
//      private IEventRaiser _torpedoedEventRaiser;

//      [SetUp]
//      public void BeforeEachTest()
//      {
//         _mocksRepo2 = new MockRepository();
//         _mockPrinter = _mocksRepo2.DynamicMock<IPrinter>();
//         _mockLogger = _mocksRepo2.StrictMock<ILog>();

//         _mockPrinter.Torpedoed += null;
//         _torpedoedEventRaiser = LastCall.IgnoreArguments().GetEventRaiser();

//         _mocksRepo2.ReplayAll();  // or else Rhino records the event subscribe as an expectation 
//         _testSubject = new CrashTestDummy(DUMMY_NAME, _mockPrinter, _mockLogger);
//         _mocksRepo2.BackToRecordAll(BackToRecordOptions.None); // remember to use .None or else!
//      }

//      [TearDown]
//      public void AfterEachTest()
//      {
//         _mocksRepo2.ReplayAll(); // 2nd call to ReplayAll does nothing. Safeguard check
//         _mocksRepo2.VerifyAll();
//      }

//      [Test]
//      public void Test_RaisingAnEventFromAMock()
//      {
//         var e = new MyEventArgs(DateTime.Now);

//         _mockLogger.LogMessage(null);
//         LastCall.Constraints(Text.Contains(e.ToString()));
//         _mocksRepo2.ReplayAll();

//         _torpedoedEventRaiser.Raise(_mockPrinter, e);
//      } 

//   }
//}
