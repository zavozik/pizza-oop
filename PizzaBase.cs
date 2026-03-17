using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    internal partial class Program
    {
        public class PizzaBase : PizzaComponent 
        {
            private static List<PizzaBase> ListOfbases = new List<PizzaBase>();

            public static IReadOnlyList<PizzaBase> Bases => ListOfbases.AsReadOnly();

            public PizzaBase(string name, decimal price) : base(name, price)
            {
                ListOfbases.Add(this);
            }

            public override void Display()
            {
                Console.WriteLine($"Основа: {Name}, Цена: {Price}");
            }

            public static void ShowAll()
            {
                if (ListOfbases.Count > 0)
                {
                    for (int i = 0; i < ListOfbases.Count; i++)
                    {
                        Console.WriteLine($"[ID {i + 1}] Основа: {ListOfbases[i].Name}, Цена: {ListOfbases[i].Price}");
                    }
                }
                else
                {
                    Console.WriteLine("Список основ пуст.");
                }
            }

            public static void CreateItem()
            {
                Console.WriteLine("--- СОЗДАНИЕ ОСНОВЫ ---");
                Console.Write("Введите название основы: ");
                string name = Console.ReadLine();

                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("\nОшибка: введена недопустимое название!");
                    return;
                }

                Console.Write("Введите стоимость основы: ");
                string cost = Console.ReadLine();

                if (!decimal.TryParse(cost, out decimal price) || price <= 0)
                {
                    Console.WriteLine("\nОшибка: введена недопустимая стоимость!");
                }
                else
                {
                    if (PizzaBase.Bases.Count == 0 && name != "Классическая" && name != "классическая")
                    {
                        Console.WriteLine("\nОшибка: первая основа должна быть \"Классическая\"!");
                    }
                    else if (PizzaBase.Bases.Count == 0)
                    {
                        PizzaBase product = new PizzaBase(name, price);
                        Console.WriteLine($"\nОснова \"{name}\" стоимостью {price} успешно создана!");
                    }
                    else if (price / PizzaBase.Bases[0].Price <= 6/5)
                    {
                        PizzaBase product = new PizzaBase(name, price);
                        Console.WriteLine($"\nОснова \"{name}\" стоимостью {price} успешно создана!");
                    }
                    else
                    {
                        Console.WriteLine($"\nОшибка: цена не должна превышать {PizzaBase.Bases[0].Price * 6 / 5}!");
                    }
                }
            }

            public static void ChangeItem()
            {
                Console.WriteLine("--- РЕДАКТИРОВАНИЕ ОСНОВЫ ---\n");
                PizzaBase.ShowAll();

                if (PizzaBase.Bases.Count > 0)
                {
                    Console.Write("\nВведите номер основы для редактирования: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int userChoice) && userChoice > 0 && userChoice <= PizzaBase.Bases.Count)
                    {
                        Console.Clear();
                        userChoice--;
                        Console.WriteLine("Выбранная основа:");
                        PizzaBase.Bases[userChoice].Display();

                        Console.WriteLine("\n1. Изменить название");
                        Console.WriteLine("2. Изменить стоимость");
                        Console.Write("\nВыберите действие: ");

                        switch (Console.ReadLine())
                        {
                            case "1":
                                if(userChoice == 0)
                                {
                                    Console.WriteLine("Нельзя поменять название классики!");
                                    break;
                                }
                                Console.Write("Введите новое название: ");
                                string newName = Console.ReadLine();

                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    PizzaBase.Bases[userChoice].UpdateName(newName);
                                    Console.WriteLine("\nНазвание успешно изменено!");
                                }
                                else
                                {
                                    Console.WriteLine("\nОшибка: введено некорректное название.");
                                }
                                break;

                            case "2":
                                Console.Write($"Текущая стоимость: {PizzaBase.Bases[userChoice].Price}\nВведите новую стоимость: ");
                                string newCost = Console.ReadLine();

                                if (decimal.TryParse(newCost, out decimal newPrice) && newPrice > 0 && newPrice <= Bases[0].Price * 6 / 5)
                                {
                                    PizzaBase.Bases[userChoice].UpdatePrice(newPrice);
                                    Console.WriteLine("\nСтоимость успешно изменена!");
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
                        Console.WriteLine("\nОшибка: введен неверный номер основы.");
                    }
                }
            }

            public static void DeleteItem()
            {
                Console.WriteLine("--- УДАЛЕНИЕ ОСНОВЫ ---\n");
                PizzaBase.ShowAll();

                if (PizzaBase.Bases.Count > 0)
                {
                    Console.Write("\nВведите номер основы для удаления: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number) && number > 0 && number <= PizzaBase.Bases.Count)
                    {
                        if (PizzaBase.Bases.Count == 1)
                        {
                            Console.WriteLine("\nОшибка: нельзя удалить единственную основу!");
                        }
                        else if(number == 1)
                        {
                            Console.WriteLine("\nНельзя удалить классическую основу!");
                        }
                        else
                        {
                            string deletedName = PizzaBase.Bases[number - 1].Name;
                            Console.Write($"\nВы уверены, что хотите удалить основу \"{deletedName}\"? (да/нет): ");
                            string confirmation = Console.ReadLine();

                            if (confirmation.ToLower() == "да")
                            {
                                PizzaBase.Remove(number - 1);
                                Console.WriteLine($"\nОснова \"{deletedName}\" успешно удалена!");
                            }
                            else
                            {
                                Console.WriteLine("\nУдаление отменено.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nОшибка: введен некорректный номер.");
                    }
                }
            }
            public static void Remove(int index) => ListOfbases.RemoveAt(index);

            public static void FilterBasesByPrice()
            {
                decimal maxPrice;
                Console.Write("Введите минимальную цену: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    Console.Write("Введите максимальную цену: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal maxprice) || maxprice < minPrice)
                    {
                        Console.WriteLine("Ошибка: некорректная максимальная цена!");
                        return;
                    }
                    maxPrice = maxprice;
                }
                else
                {
                    Console.WriteLine("Ошибка: некорректная минимальная цена!");
                    return;
                }

                Console.WriteLine($"Основы в ценовом диапазоне от {minPrice} до {maxPrice} руб.:");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var baseItem in ListOfbases)
                {
                    if (baseItem.Price >= minPrice && baseItem.Price <= maxPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {baseItem.Name} - {baseItem.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Основ в указанном ценовом диапазоне не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено основ: {foundCount}");
                }
            }

            public static int GetLengthBasic() { return Bases.Count; }
            public static PizzaBase GetBasicByIndex(int index) { return ListOfbases[index]; }
            public static void FilterBasesByMinPrice()
            {
                Console.Write("Введите минимальную цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    Console.WriteLine("Ошибка: некорректная цена!");
                    return;
                }

                Console.WriteLine($"\nОсновы с ценой от {minPrice} руб. и выше:");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var baseItem in ListOfbases)
                {
                    if (baseItem.Price >= minPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {baseItem.Name} - {baseItem.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"\nОснов с ценой от {minPrice} руб. не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено основ: {foundCount}");
                }
            }
            public static void FilterBasesByMaxPrice()
            {
                Console.Write("Введите максимальную цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
                {
                    Console.WriteLine("Ошибка: некорректная цена!");
                    return;
                }

                Console.WriteLine($"\nОсновы с ценой до {maxPrice} руб.:");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var baseItem in ListOfbases)
                {
                    if (baseItem.Price <= maxPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {baseItem.Name} - {baseItem.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"\nОснов с ценой до {maxPrice} руб. не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено основ: {foundCount}");
                }
            }
        }
    }
}
