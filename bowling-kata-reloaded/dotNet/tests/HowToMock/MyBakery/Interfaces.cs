using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyBakery
{
    public interface Chef
    {
        bool IsAvailable { get; }

        void Bake(CakeFlavors flavor, bool withIcing);
        void GoEasyOn(string ingredient);
    }
    public interface Inventory
    {
        bool ReplenishStocks();
        bool IsEmpty { get; }
        event EventHandler<StockEventArgs> RunningLowOnIngredient;
    }
    
}
