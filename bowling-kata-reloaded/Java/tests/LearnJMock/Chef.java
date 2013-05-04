public interface Chef
{
  void bake(CakeFlavors flavor, boolean withIcing);
  void bakeRealWorld(CakeFlavors flavor, boolean withIcing) throws OutOfIngredientsException;
  
}

