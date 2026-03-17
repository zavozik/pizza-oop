using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    internal partial class Program
    {
        class ManualPizza : IHasPrice
        {
            public string Name { get; private set; }
            public decimal Price { get; private set; }
            public PizzaSize Size { get; private set; }
            public PizzaBase Base { get; private set; }
            public Crust Crust { get; private set; }

            private Dictionary<Ingredient, int> Ingredients = new Dictionary<Ingredient, int>();

            public ManualPizza(string name, PizzaSize size, PizzaBase pizzaBase, Crust crust = null)
            {
                Name = name;
                Size = size;
                Base = pizzaBase;
                Crust = crust;
                UpdatePrice();
            }

            public void RemoveIngredient(Ingredient ingredient)
            {
                if (Ingredients.ContainsKey(ingredient))
                {
                    Ingredients.Remove(ingredient);
                    UpdatePrice();
                }
            }

            public void UpdatePrice()
            {
                decimal totalPrice = Base.Price;

                foreach (var item in Ingredients)
                {
                    totalPrice += item.Key.Price * item.Value;
                }

                if (Crust != null)
                {
                    totalPrice += Crust.Price;
                }

                switch (Size)
                {
                    case PizzaSize.Small:
                        totalPrice *= 0.7m;
                        break;
                    case PizzaSize.Medium:
                        totalPrice *= 0.85m;
                        break;
                }

                Price = totalPrice;
            }
            public void DisplayIngredients()
            {
                foreach (var item in Ingredients)
                {
                    Console.WriteLine($"{item.Key.Name}: {item.Value} порций");
                }
            }

            public void AddIngredient(Ingredient ingredient, int portions)
            {
                if (Ingredients.ContainsKey(ingredient))
                {
                    Ingredients[ingredient] += portions;
                }
                else
                {
                    Ingredients.Add(ingredient, portions);
                }
                UpdatePrice();
            }

            public bool HasIngredient(Ingredient ingredient)
            {
                foreach (var Ingr in Ingredients.Keys)
                {
                    if (Ingr == ingredient)
                        return true;
                }
                return false;
            }

            public bool HasBase(PizzaBase pizzaBase)
            {
                return Base == pizzaBase;
            }

            public bool HasCrust(Crust crust)
            {
                return Crust == crust;
            }
        }
    }
}