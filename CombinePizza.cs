using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    internal partial class Program
    {
        class CombinePizza : IHasPrice
        {
            public string Name { get; private set; }
            public decimal Price { get; private set; }
            public PizzaSize Size { get; private set; }
            public PizzaBase Base { get; private set; }
            public Crust Crust { get; private set; }

            private List<PartOfCombine> Parts = new List<PartOfCombine>();
            private const int TotalPieces = 8; 

            public CombinePizza(string name, PizzaSize size, PizzaBase pizzaBase, Crust crust = null)
            {
                Name = name;
                Size = size;
                Base = pizzaBase;
                Crust = crust;
                UpdatePrice();
            }

            public void AddPart(Pizza pizza, int pieces)
            {
                if (pieces <= 0 || pieces > TotalPieces)
                {
                    Console.WriteLine($"Ошибка: количество кусков должно быть от 1 до {TotalPieces}");
                    return;
                }

                int totalPieces = Parts.Sum(p => p.Pieces) + pieces;
                if (totalPieces > TotalPieces)
                {
                    Console.WriteLine($"Ошибка: сумма кусков не может превышать {TotalPieces}. Текущая сумма: {totalPieces - pieces}, осталось: {TotalPieces - (totalPieces - pieces)} кусков");
                    return;
                }

                Parts.Add(new PartOfCombine(pizza, pieces));
                UpdatePrice();

                if (Parts.Sum(p => p.Pieces) == TotalPieces)
                {
                    Console.WriteLine($"Комбинация пиццы завершена (все {TotalPieces} кусков распределены)");
                }
            }

            public void AddPart(ManualPizza pizza, int pieces)
            {
                if (pieces <= 0 || pieces > TotalPieces)
                {
                    Console.WriteLine($"Ошибка: количество кусков должно быть от 1 до {TotalPieces}");
                    return;
                }

                int totalPieces = Parts.Sum(p => p.Pieces) + pieces;
                if (totalPieces > TotalPieces)
                {
                    Console.WriteLine($"Ошибка: сумма кусков не может превышать {TotalPieces}. Текущая сумма: {totalPieces - pieces}, осталось: {TotalPieces - (totalPieces - pieces)} кусков");
                    return;
                }

                Parts.Add(new PartOfCombine(pizza, pieces));
                UpdatePrice();
            }

            private string GetPizzaName(IHasPrice pizza)
            {
                if (pizza is Pizza p) return p.Name;
                if (pizza is ManualPizza mp) return mp.Name;
                if (pizza is CombinePizza cp) return cp.Name;
                return "Неизвестная пицца";
            }

            public List<PartOfCombine> GetParts()
            {
                return new List<PartOfCombine>(Parts);
            }


            public void UpdatePrice()
            {
                decimal totalPrice = Base.Price;

                foreach (var part in Parts)
                {
                    totalPrice += part.Pizza.Price * ((decimal)part.Pieces / TotalPieces);
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
                    case PizzaSize.Big:
                        break;
                }

                Price = totalPrice;
            }
            public bool HasIngredient(Ingredient ingredient)
            {
                foreach (var part in Parts)
                {
                    if (part.Pizza is Pizza p && p.HasIngredient(ingredient))
                        return true;
                    if (part.Pizza is ManualPizza mp && mp.HasIngredient(ingredient))
                        return true;
                    if (part.Pizza is CombinePizza cp && cp.HasIngredient(ingredient))
                        return true;
                }
                return false;
            }

            public void Display()
            {
                Console.WriteLine($"Комбинированная пицца: {Name}");
                Console.WriteLine($"Размер: {Size}, Основа: {Base.Name}, Цена: {Price}");
                if (Crust != null)
                {
                    Console.WriteLine($"Борт: {Crust.Name}");
                }
                Console.WriteLine($"Состав (всего {TotalPieces} кусков):");
                foreach (var part in Parts)
                {
                    string pizzaName = GetPizzaName(part.Pizza);
                    Console.WriteLine($"  - {pizzaName}: {part.Pieces} кусков");
                }
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