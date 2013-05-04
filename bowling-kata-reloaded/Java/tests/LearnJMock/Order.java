public class Order
{
  private CakeFlavors _flavor;
  private boolean _hasIcing;
  private int _quantity;
  public Order(CakeFlavors flavor, boolean withIcing, int quantity)
  {
    _flavor = flavor;
    _hasIcing = withIcing;
    _quantity = quantity;
  }
  public CakeFlavors getFlavor()
  { return _flavor;}
  public boolean hasIcing()
  { return _hasIcing; }
  public int getQuantity()
  { return _quantity; }
  
}