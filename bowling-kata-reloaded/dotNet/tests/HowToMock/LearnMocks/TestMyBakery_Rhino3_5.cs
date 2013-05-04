using NUnit.Framework;
using Rhino.Mocks;
using MyBakery;
using System;

namespace LearnRhinoMocks
{
    [TestFixture]
    public class TestMyBakery_Rhino3_5
    {
        [Test]
        public void Test_ExpectMethodCall()
        {
            var mockChef = MockRepository.GenerateMock<Chef>();
            var mockInventory = MockRepository.GenerateMock<Inventory>();

            Bakery bakery = new Bakery(mockChef, mockInventory);

            mockChef.Expect(chef => chef.Bake(CakeFlavors.Vanilla, false));

            bakery.PlaceOrder(new Order { Flavor = CakeFlavors.Vanilla, WithIcing = false, Quantity = 1 });

            mockChef.VerifyAllExpectations();
        }

        

        Bakery _bakery;
        Chef _mockChef;
        Inventory _mockInventory;
        [SetUp]
        public void BeforeEachTest()
        {
            _mockChef = MockRepository.GenerateMock<Chef>();
            _mockInventory = MockRepository.GenerateMock<Inventory>();
            _bakery = new Bakery(_mockChef, _mockInventory);
        }

        [Test]
        public void Test_ExpectPropertyGet()
        {
            _mockChef.Expect(chef => chef.IsAvailable).Return(true);
            Assert.IsTrue(_bakery.IsOpen);

            _mockChef.Expect(chef => chef.IsAvailable).Return(false);
            Assert.IsFalse(_bakery.IsOpen);

            _mockChef.VerifyAllExpectations();
        }

        [Test]
        public void Test_ExpectMultipleCalls()
        {
            _mockChef.Expect(chef => chef.Bake(CakeFlavors.Pineapple, true)).Repeat.Times(3);

            _bakery.PlaceOrder(new Order { Flavor = CakeFlavors.Pineapple, WithIcing = true, Quantity = 3 });
            
            _mockChef.VerifyAllExpectations();
        }

        private static Order OrderForOnePineappleCakeNoIcing
        {
            get
            {
                return new Order { Flavor = CakeFlavors.Pineapple, WithIcing = false, Quantity = 1 };
            }
        }


        [Test]
        public void Test_ExpectAndReturnValue()
        {
            _mockInventory.Expect(inv => inv.IsEmpty).Return(false);
            _mockChef.Expect(chef => chef.Bake(CakeFlavors.Pineapple, false));

            _bakery.PleaseDonate(OrderForOnePineappleCakeNoIcing);
            
            _mockChef.VerifyAllExpectations();
            _mockInventory.VerifyAllExpectations();
        }


        [Test]
        public void Test_ExpectNoCall()
        {
            _mockInventory.Expect(inv => inv.IsEmpty).Return(true);
            _mockChef.Expect(chef => chef.Bake(Arg<CakeFlavors>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Never();

            _bakery.PleaseDonate(OrderForOnePineappleCakeNoIcing);

            _mockInventory.VerifyAllExpectations();
            _mockChef.VerifyAllExpectations();
        }

        [ExpectedException(ExceptionType=typeof(UnableToServeException), MatchType=MessageMatch.Contains, ExpectedMessage="Sorry for the inconvenience.")]
        [Test]
        public void Test_ExpectExceptionToBeThrown()
        {
            _mockChef.Expect(chef => chef.Bake(CakeFlavors.Pineapple, false)).Throw(new Exception());

            _bakery.PlaceOrder(OrderForOnePineappleCakeNoIcing);
        }

        [Test]
        public void Test_ExpectWithCustomCallback()
        {
            int callbacks = 0;
            _mockChef.Expect(chef => chef.Bake(CakeFlavors.Pineapple, false))
                .Repeat.Times(2)
                .WhenCalled(delegate(MethodInvocation mi) {
                    Console.WriteLine("Callback Params: {0}, {1}", mi.Arguments[0], mi.Arguments[1]);
                    callbacks += 1;
                    if (callbacks == 1)
                        throw new OutOfIngredientsException();

                    return;
                });
            _mockInventory.Expect(inv => inv.ReplenishStocks()).Return(true);
            
            _bakery.PlaceOrder(OrderForOnePineappleCakeNoIcing);
            
            _mockChef.VerifyAllExpectations();
            _mockInventory.VerifyAllExpectations();
        }


        [Test]
        public void Test_RaiseEventFromMock()
        {
            _mockChef.Expect(chef => chef.GoEasyOn("Sugar"));

            _mockInventory.Raise(inv => inv.RunningLowOnIngredient += null,
                this, new StockEventArgs("Sugar"));

            _mockChef.VerifyAllExpectations();
        }


        [Test]
        public void Test_ConstrainingArguments()
        {
            var itemAboutToGoOutOfStock = "Eggs";
            _mockChef.Expect(chef => chef.GoEasyOn(
                Arg<string>.Matches (ingredient => ComplexCheck(itemAboutToGoOutOfStock, ingredient))));

            _mockInventory.Raise(inventory => inventory.RunningLowOnIngredient += null,
                this, new StockEventArgs(itemAboutToGoOutOfStock));
            _mockChef.VerifyAllExpectations();
        }
        private bool ComplexCheck(string expected, string actual)
        {
            return expected.Equals(actual);
        }

        [Test]
        public void Test_ExpectCallsInOrder()
        {
            var mockCreator = new MockRepository();
            _mockChef = mockCreator.DynamicMock<Chef>();
            _mockInventory = mockCreator.DynamicMock<Inventory>();
            mockCreator.ReplayAll();

            _bakery = new Bakery(_mockChef, _mockInventory);

            using (mockCreator.Ordered())
            {
                _mockInventory.Expect(inv => inv.IsEmpty).Return(false);
                _mockChef.Expect(chef => chef.Bake(CakeFlavors.Pineapple, false));
            }


            _bakery.PleaseDonate(OrderForOnePineappleCakeNoIcing);

            _mockChef.VerifyAllExpectations();
            _mockInventory.VerifyAllExpectations();
        }
      
    }
}
