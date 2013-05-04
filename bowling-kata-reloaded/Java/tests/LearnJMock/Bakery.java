public class Bakery
{
  private Chef _chef;
  private Inventory _inventory;
  
  public Bakery(Chef chef, Inventory inventory)
  {
    _chef = chef;
    _inventory = inventory;
  }
  
  public void placeOrder(Order anOrder) 
  {
    for(int i = 0; i< anOrder.getQuantity(); i++)
    {
      _chef.bake(anOrder.getFlavor(), anOrder.hasIcing());
    }
  }
  
  public void placeOrderRealWorld(Order anOrder) 
  {
    for(int i = 0; i< anOrder.getQuantity(); i++)
    {
      if (!tryBake(anOrder))
      {
        _inventory.replenishStocks();
        tryBake(anOrder);
      }
    }
  }
  private boolean tryBake(Order anOrder)
  {
      try
      {
        _chef.bakeRealWorld(anOrder.getFlavor(), anOrder.hasIcing());
        return true;
      }
      catch(OutOfIngredientsException ex)
      {
        return false;
      }
  }
  
  public void pleaseDonate( Order anOrder ) 
  {
      if (!_inventory.isEmpty())
        _chef.bake( anOrder.getFlavor(), anOrder.hasIcing() );
      //System.out.println(_inventory.isEmpty());
  }
    
}