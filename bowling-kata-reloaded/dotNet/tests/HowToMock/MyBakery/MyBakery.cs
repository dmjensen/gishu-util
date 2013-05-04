using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using MyBakery;

namespace MyBakery
{
    
    public class Bakery
    {
        Chef _chef;
        Inventory _inventory;
        public Bakery(Chef chef, Inventory inventory)
        {
            _chef = chef;
            _inventory = inventory;

            _inventory.RunningLowOnIngredient += delegate(object sender, StockEventArgs e) { _chef.GoEasyOn(e.Ingredient); };
        }

        public bool IsOpen
        {
            get { return _chef.IsAvailable; }
        }
    
        public void PlaceOrder(Order order)
        {
            order.Quantity.Times(
                delegate 
                {
                    try
                    {
                        _chef.Bake(order.Flavor, order.WithIcing);
                    }
                    catch (OutOfIngredientsException)
                    {
                        if (_inventory.ReplenishStocks())
                            _chef.Bake(order.Flavor, order.WithIcing);
                    }
                    catch (Exception)
                    {
                        throw new UnableToServeException(@"We regret to inform you that we can't complete your order today. 
Sorry for the inconvenience. Your cake would be delivered to you tomorrow without any charges.");
                    }
                }
            );
        }

        public void PleaseDonate(Order order)
        {
            if (!_inventory.IsEmpty)
                _chef.Bake(order.Flavor, order.WithIcing);
            //    var isEmpty = _inventory.IsEmpty;
        }

        public void SomeMethod()
        {
            Console.WriteLine(_chef.IsAvailable);
            _chef.Bake(CakeFlavors.Pineapple, false);
            
            _chef.Bake(CakeFlavors.Vanilla, true);
        }
    }
    static class MyExtensions
    {
        //TODO: 2010 01 22 Add test for this
        public static void Times(this int count, Action action)
        {
            for (int i = 0; i < count; i++)
                action.Invoke();
        }
    }
    
}
