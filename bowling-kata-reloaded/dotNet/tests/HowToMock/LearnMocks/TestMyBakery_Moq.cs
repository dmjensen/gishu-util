using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using MyBakery;

namespace LearnMoq
{
    [TestFixture]
    public class TestMyBakery_Moq
    {
        Mock<Chef> _mockChef;
        Mock<Inventory> _mockInventory;
        Bakery _bakery;

        [Test]
        public void Test_ExpectMethodCall()
        {
            //arrange
            var mockChef = new Mock<Chef>();
            var mockInventory = new Mock<Inventory>();
            //arrange: inject dep
            var bakery = new Bakery(mockChef.Object, mockInventory.Object);
            // arrange: expect
            _mockChef.Setup(chef => chef.Bake(CakeFlavors.Vanilla, true));

            //act
            _bakery.PlaceOrder(new Order { Flavor = CakeFlavors.Vanilla, WithIcing = true, Quantity = 1 });
            //assert
            _mockChef.VerifyAll();
        }
        


        private Order OrderForOnePineappleCakeNoIcing
        {
            get
            {
                return new Order { Flavor = CakeFlavors.Pineapple, WithIcing = false, Quantity = 1 };
            }
        }
        

        [SetUp]
        public void BeforeEachTest()
        {
            // arrange
            _mockChef = new Mock<Chef>();
            _mockInventory = new Mock<Inventory>();
            _bakery = new Bakery(_mockChef.Object, _mockInventory.Object);
        }
        [Test]
        public void Test_ExpectPropertyGet()
        {
            _mockChef.Setup(chef => chef.IsAvailable).Returns(true);
            Assert.IsTrue(_bakery.IsOpen);

            _mockChef.Setup(chef => chef.IsAvailable).Returns(false);
            Assert.IsFalse(_bakery.IsOpen);

            _mockChef.VerifyAll();
        }

        
        [Test]
        public void Test_ExpectMultipleMethodCalls()
        {
            _bakery.PlaceOrder(new Order { Flavor = CakeFlavors.Pineapple, WithIcing = false, Quantity = 3 });
            _mockChef.Verify( chef => chef.Bake(CakeFlavors.Pineapple, false), Times.Exactly(3), "Chef should have been called thrice!");
        }

        [Test]
        public void Test_ExpectAndReturnValue()
        {
            _mockInventory.Setup(inventory => inventory.IsEmpty).Returns(false);
            _mockChef.Setup(chef => chef.Bake(CakeFlavors.Pineapple, false));

            _bakery.PleaseDonate(OrderForOnePineappleCakeNoIcing);

            _mockChef.VerifyAll();
            _mockInventory.VerifyAll();
        }

        [Test]
        public void Test_ExpectNoCall()
        {
            _mockInventory.Setup(inventory => inventory.IsEmpty).Returns(true);
            _bakery.PleaseDonate(OrderForOnePineappleCakeNoIcing);

            _mockChef.Verify(chef => chef.Bake(It.IsAny<CakeFlavors>(), It.IsAny<bool>()), Times.Never());
            _mockInventory.VerifyAll();
        }

        [ExpectedException(ExceptionType=typeof(UnableToServeException), 
            MatchType=MessageMatch.Contains, 
            ExpectedMessage="Sorry for the inconvenience.")]
        [Test]
        public void Test_ExpectExceptionToBeThrown()
        {
            _mockChef.Setup( chef => chef.Bake(CakeFlavors.Pineapple, false)).Throws<Exception>();
            _bakery.PlaceOrder( OrderForOnePineappleCakeNoIcing );
            
        }
      

        [Test]
        public void Test_ExpectWithCustomCallback()
        {
            var callbacks = 0;
            _mockChef.Setup(chef => chef.Bake(CakeFlavors.Pineapple, false))
                .Callback( delegate(CakeFlavors flavor, bool withIcing)
                            {   // any custom code
                                Console.WriteLine("Callback Params: {0}, {1}", flavor, withIcing);
                                callbacks += 1;
                                if (callbacks == 1)
                                    throw new OutOfIngredientsException();

                                return;
                            }
                );
            
            _mockInventory.Setup(inv => inv.ReplenishStocks()).Returns(true);
            _bakery.PlaceOrder( OrderForOnePineappleCakeNoIcing );
            
            _mockChef.VerifyAll();
            _mockInventory.VerifyAll();
        }

        
      

        

        [Test]
        public void Test_RaiseEventFromMock()
        {
            _mockChef.Setup(chef => chef.GoEasyOn("Sugar"));

            _mockInventory.Raise(inventory => inventory.RunningLowOnIngredient += null, new StockEventArgs("Sugar"));
            _mockChef.VerifyAll();
        }


        [Test]
        public void Test_ConstrainingArguments()
        {
            var itemAboutToGoOutOfStock = "Eggs";
            _mockChef.Setup(chef => chef.GoEasyOn(
                It.Is<string>(ingredient => ComplexCheck(itemAboutToGoOutOfStock, ingredient))));

            _mockInventory.Raise(inventory => inventory.RunningLowOnIngredient += null, 
                new StockEventArgs(itemAboutToGoOutOfStock));
            _mockChef.VerifyAll();
        }
        private bool ComplexCheck(string expected, string actual)
        {
            return expected.Equals(actual);
        }


        [Test]
        public void Test_ExpectCallsInOrder()
        {
            var sequence = String.Empty;
            _mockInventory.Setup(inv => inv.IsEmpty).Returns(false)
                .Callback(delegate { sequence += "1"; });
            _mockChef.Setup(chef => chef.Bake(CakeFlavors.Pineapple, false))
                .Callback(delegate { sequence += "2"; });

            _bakery.PleaseDonate(OrderForOnePineappleCakeNoIcing);

            Assert.AreEqual("12", sequence, "Calls not in expected order!");
            _mockChef.VerifyAll();
            _mockInventory.VerifyAll();
        }
    }
        
}
