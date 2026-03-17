using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ConsoleApp2.Program;

namespace ConsoleApp2
{
    internal partial class Program
    {
        public class Ingredient : PizzaComponent
        {
            private static Dictionary<Guid, Ingredient> AllIngredients = new Dictionary<Guid,Ingredient>();

            public Ingredient(string name, decimal price) : base(name, price)
            {
                AllIngredients.Add(Id, this);
            }

            public override void Display() => Console.WriteLine($"Ингредиент: {Name}, Цена: {Price}");

            public static void ShowAll()
            {
                if (AllIngredients.Count > 0)
                {
                    int counter = 1;
                    foreach (KeyValuePair<Guid, Ingredient> item in AllIngredients)
                    {
                        Console.WriteLine($" {counter}. Ингредиент: {item.Value.Name}, Цена: {item.Value.Price}");
                        counter++;
                    }
                }
                else
                {
                    Console.WriteLine("Список ингредиентов пуст.");
                }
            }

            public static void CreateItem()
            {
                Console.WriteLine("--- СОЗДАНИЕ ИНГРЕДИЕНТА ---");
                Console.Write("Введите название ингредиента: ");
                string name = Console.ReadLine();

                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("\nОшибка: введено недопустимое название!");
                    return;
                }

                Console.Write("Введите стоимость ингредиента: ");
                string cost = Console.ReadLine();

                if (!decimal.TryParse(cost, out decimal price) || price <= 0)
                {
                    Console.WriteLine("\nОшибка: введена недопустимая стоимость!");
                }
                else
                {
                    Ingredient product = new Ingredient(name, price);
                    Console.WriteLine($"\nИнгредиент \"{name}\" стоимостью {price} успешно создан!");
                }
            }

            public static void ChangeItem()
            {
                Console.WriteLine("--- РЕДАКТИРОВАНИЕ ИНГРЕДИЕНТА ---\n");
                ShowAll();

                if (AllIngredients.Count > 0)
                {
                    Console.Write("\nВведите номер ингредиента для редактирования: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int userChoice) && userChoice > 0 && userChoice <= AllIngredients.Count)
                    {
                        Console.Clear();

                        var selectedItem = AllIngredients.ElementAt(userChoice - 1);
                        Guid selectedId = selectedItem.Key;
                        Ingredient ingredientToEdit = selectedItem.Value;
                        
                        Console.WriteLine("--- РЕДАКТИРОВАНИЕ ИНГРЕДИЕНТА ---\n");
                        ingredientToEdit.Display();

                        Console.WriteLine("\n1. Изменить название");
                        Console.WriteLine("2. Изменить стоимость");
                        Console.Write("\nВыберите действие: ");

                        switch (Console.ReadLine())
                        {
                            case "1":
                                Console.Write("Введите новое название: ");
                                string newName = Console.ReadLine();

                                if (!string.IsNullOrWhiteSpace(newName) && newName != ingredientToEdit.Name)
                                {
                                    ingredientToEdit.UpdateName(newName);
                                    Console.WriteLine("Название успешно изменено!");
                                }
                                else if (newName == ingredientToEdit.Name)
                                {
                                    Console.WriteLine("\nВведено идентичное имя!");
                                }
                                else
                                {
                                    Console.WriteLine("\nОшибка: введено некорректное название.");
                                }
                                break;

                            case "2":
                                Console.Write($"\nТекущая стоимость: {ingredientToEdit.Price}\nВведите новую стоимость: ");
                                string newCost = Console.ReadLine();

                                if (decimal.TryParse(newCost, out decimal newPrice) && newPrice > 0)
                                {
                                    ingredientToEdit.UpdatePrice(newPrice);
                                    Console.WriteLine("Стоимость успешно изменена!");
                                }
                                else
                                {
                                    Console.WriteLine("\nОшибка: введена некорректная стоимость.");
                                }
                                break;

                            default:
                                Console.WriteLine("\nОшибка: введена неверная команда.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nОшибка: введен неверный номер ингредиента.");
                    }
                }
            }

            public static void DeleteItem()
            {
                Console.WriteLine("--- УДАЛЕНИЕ ИНГРЕДИЕНТА ---\n");
                Ingredient.ShowAll();

                if (AllIngredients.Count > 0)
                {
                    Console.Write("\nВведите номер ингредиента для удаления: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number) && number > 0 && number <= AllIngredients.Count)
                    {
                        var selectedItem = AllIngredients.ElementAt(number - 1);
                        Guid selectedId = selectedItem.Key;
                        Ingredient ingredientToDelete = selectedItem.Value;

                        Console.Write($"\nВы уверены, что хотите удалить ингредиент \"{ingredientToDelete.Name}\"? (да/нет): ");

                        string confirmation = Console.ReadLine();

                        if (confirmation.ToLower() == "да")
                        {
                            Pizza.Removeingredient( ingredientToDelete );
                            AllIngredients.Remove(selectedItem.Key);
                            Console.WriteLine($"\nИнгредиент \"{ingredientToDelete.Name}\" успешно удален!");
                            
                        }
                        else
                        {
                            Console.WriteLine("\nУдаление отменено.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nОшибка: введен некорректный номер.");
                    }
                }
            }
            public static Ingredient GetIngredientByIndex(int index) { return AllIngredients.ElementAt(index).Value; }

            public static int GetLengthIngredients() { return AllIngredients.Count;}

            public static void FilterIngredientsByPrice()
            {
                decimal maxPrice;
                Console.Write("Введите минимальную цену: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    Console.Write("Введите максимальную цену: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal maxprice) || !(maxprice >= minPrice))
                    {
                        Console.WriteLine("Ошибка: некорректная максимальная цена!");
                        return;
                    }
                    else
                    {
                        maxPrice = maxprice;
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка: некорректная минимальная цена!");
                    return;
                }

                Console.WriteLine($"\nИнгредиенты в ценовом диапазоне от {minPrice} до {maxPrice} руб.:");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var ingredient in AllIngredients.Values)
                {
                    if (ingredient.Price >= minPrice && ingredient.Price <= maxPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {ingredient.Name} - {ingredient.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Ингредиентов в указанном ценовом диапазоне не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено ингредиентов: {foundCount}");
                }
            }

            public static void FilterIngredientsByMinPrice()
            {
                Console.Write("Введите минимальную цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    Console.WriteLine("Ошибка: некорректная цена!");
                    return;
                }
                else

                Console.WriteLine($"\nИнгредиенты с ценой от {minPrice} руб. и выше:");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var ingredient in AllIngredients.Values)
                {
                    if (ingredient.Price >= minPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {ingredient.Name} - {ingredient.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"\nИнгредиентов с ценой от {minPrice} руб. не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено ингредиентов: {foundCount}");
                }
            }

            public static void FilterIngredientsByMaxPrice()
            {
                Console.Write("Введите максимальную цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
                {
                    Console.WriteLine("Ошибка: некорректная цена!");
                }

                Console.WriteLine($"\nИнгредиенты с ценой до {maxPrice} руб.:");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var ingredient in AllIngredients.Values)
                {
                    if (ingredient.Price <= maxPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {ingredient.Name} - {ingredient.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Ингредиентов с ценой до {maxPrice} руб. не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено ингредиентов: {foundCount}");
                }
            }

        
        }
    }
}
