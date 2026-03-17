using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    internal partial class Program
    {
        public abstract class PizzaComponent : IGuid, INamed, IHasPrice
        {
            public Guid Id { get; }
            public string Name { get; private set; }
            public decimal Price { get; private set; }

            protected PizzaComponent(string name, decimal price)
            {
                Name = name;
                Price = price;
                Id = Guid.NewGuid();
            }

            public void UpdateName(string newName)
            {
                Name = newName;
            }

            public void UpdatePrice(decimal newPrice)
            {
                Price = newPrice;
            }

            public abstract void Display();
        }
    }
}
