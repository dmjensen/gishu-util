
import org.jmock.Expectations;
import org.jmock.Mockery;
import org.jmock.integration.junit4.JMock;
import org.jmock.integration.junit4.JUnit4Mockery;
import org.jmock.Sequence;

import org.junit.*;
import org.junit.runner.*;
import static org.junit.Assert.* ;

import org.hamcrest.*; // for Matchers
import org.jmock.api.*; // for Actions

@RunWith(JMock.class)
public class TestBakery 
{
  @Test
  public void test_simple_math()
  {
    assertTrue( 2+2 == 4 );
  }
  
  // convention: name it as context
  Mockery context = new JUnit4Mockery();
  
  Chef _mockChef;
  Inventory _mockInventory;
  Bakery _bakery;

  @Before
  public void setUp()
  {
    _mockChef = context.mock(Chef.class);
    _mockInventory = context.mock(Inventory.class);
    _bakery = new Bakery(_mockChef, _mockInventory);
  }

  @Test
  public void test_expectCall()
  {
    context.checking( new Expectations() { {
      oneOf(_mockChef).bake(CakeFlavors.Pineapple, false );
    } } );
    
    _bakery.placeOrder( new Order(CakeFlavors.Pineapple, false, 1) );
    // _mockChef.Verify() - autoverified : framework :)
  }
  
  
  @Test
  public void  test_expectMultipleCalls() 
  {
    context.checking( new Expectations() {{
      exactly(3).of(_mockChef).bake(CakeFlavors.Pineapple, false);
    }} );
    
    _bakery.placeOrder( new Order(CakeFlavors.Pineapple, false, 3) );
  }
  
  @Test
  public void test_expectCallAndReturnValue()
  {
    context.checking( new Expectations() {{
      oneOf(_mockInventory).isEmpty();
        will(returnValue(false));
      oneOf(_mockChef).bake(CakeFlavors.Pineapple, false);
    }} );      
    
    _bakery.pleaseDonate( OrderForOnePineappleCakeNoIcing() );
  }
  
  private Order OrderForOnePineappleCakeNoIcing()
  { return new Order(CakeFlavors.Pineapple, false, 1);  }
  
  //http://www.jmock.org/custom-matchers.html
  @Test
  public void test_expectAndCheckArgs()
  {
    context.checking( new Expectations() { {
      oneOf(_mockInventory).isEmpty();
        will(returnValue(false));
      oneOf(_mockChef).bake( 
        with( FlavorMatcher.aPineappleFlavoredCake() ), 
        with( any(boolean.class)));
    }} );
    _bakery.pleaseDonate( OrderForOnePineappleCakeNoIcing() );
  }
  
  
  @Test
  public void test_expectNoCall()
  {
    context.checking( new Expectations() {{
      oneOf(_mockInventory).isEmpty();
        will(returnValue(true));
      never(_mockChef).bake( CakeFlavors.Pineapple, false );

    }});
    
    _bakery.pleaseDonate( OrderForOnePineappleCakeNoIcing() );  
  }
  
  @Test
  public void test_expectAndThrowException() throws OutOfIngredientsException
  {
    final Sequence aSequence = context.sequence("retryBake");
    context.checking( new Expectations() {{
      oneOf(_mockChef).bakeRealWorld( CakeFlavors.Pineapple, false );
        will(throwException(new OutOfIngredientsException() ));
        inSequence(aSequence);
      oneOf(_mockInventory).replenishStocks();
        inSequence(aSequence);
      oneOf(_mockChef).bakeRealWorld( CakeFlavors.Pineapple, false );
        inSequence(aSequence);
    }});
    
    _bakery.placeOrderRealWorld( OrderForOnePineappleCakeNoIcing() );  
  }
  // http://www.jmock.org/custom-actions.html
  @Test
  public void test_expectAndInvokeCustomCallback()
  {
    context.checking( new Expectations() { {
      oneOf(_mockChef).bake(CakeFlavors.Pineapple, false );
        will( MyCustomCallback.tracingCallback() );
    } } );
    
    _bakery.placeOrder( new Order(CakeFlavors.Pineapple, false, 1) );
  }

  @Test
  public void test_expectCallsInSpecificOrder()
  {
    final Sequence pleaseDonateSequence = context.sequence("pleaseDonate");
    context.checking( new Expectations() {{
      oneOf(_mockInventory).isEmpty();
        inSequence(pleaseDonateSequence);
        will(returnValue(false));
      oneOf(_mockChef).bake(CakeFlavors.Pineapple, false);
        inSequence(pleaseDonateSequence);
    }});
    
    _bakery.pleaseDonate( OrderForOnePineappleCakeNoIcing() );  
  }
}

class FlavorMatcher extends TypeSafeMatcher<CakeFlavors>
{
  public boolean matchesSafely(CakeFlavors flavor)
  {  return flavor == CakeFlavors.Pineapple;}
  public void describeTo(org.hamcrest.Description description) {
        description.appendText("a cake with pineapple flavor ");
  }

  
  @Factory
  public static Matcher<CakeFlavors> aPineappleFlavoredCake()
 {
    return new FlavorMatcher();
  }
}

class MyCustomCallback implements Action 
{
    public void describeTo(org.hamcrest.Description description) 
    {
        description.appendText("a custom callback to do what I deem fit! ");
    }
    
    public Object invoke(Invocation invocation) throws Throwable 
    {
      System.out.println("Callback invoked with " 
        + ((CakeFlavors)invocation.getParameter(0)) 
        + " and " 
        + (((Boolean) invocation.getParameter(1)).booleanValue() ? "" : "no")
        + " icing!");
        return null;
    }
    public static Action tracingCallback() 
    {
      return new MyCustomCallback();
    }
}