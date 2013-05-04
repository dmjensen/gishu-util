using System;
namespace MyBakery
{

    public class StockEventArgs : EventArgs
    {
        public string Ingredient { get; private set; }
        public StockEventArgs(string ingredient)
            : base()
        {
            this.Ingredient = ingredient;
        }
    }

    public class Order
    {
        public CakeFlavors Flavor { get; set; }
        public bool WithIcing { get; set; }
        public int Quantity { get; set; }
    }
    public class OutOfIngredientsException : ApplicationException
    {
    }
    public class UnableToServeException : ApplicationException
    {
        public UnableToServeException(string messageText) : base(messageText) { }
    }

    public enum CakeFlavors
    {
        Pineapple,
        Vanilla
    }
}
