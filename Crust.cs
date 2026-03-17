using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace ConsoleApp2
{
    internal partial class Program
    {
        public class Crust : PizzaComponent
        {

            private static Dictionary<Guid, Crust> AllCrusts = new Dictionary<Guid, Crust>();

            public Ingredient ingredientOfCrust { get; protected set; }

            private List<Guid> allowPizza = new List<Guid>();

            private List<Guid> forbiddenPizza = new List<Guid>();
            public Crust(string name, decimal price, Ingredient ingredient) : base(name, price)
            {
                AllCrusts.Add(Id,this);
                price = ingredient.Price;
                ingredientOfCrust = ingredient;
            }

            public static int GetLength() { return AllCrusts.Count; }

            public static Crust GetCrustByIndex(int index) { return AllCrusts.ElementAt(index).Value; }
            public override void Display()
            {
                Console.Write($"Борт: {Name}, Цена: {Price}. Состоит из {ingredientOfCrust}");
            }

            public static void CreateItem()
            {
                Console.WriteLine("--- СОЗДАНИЕ БОРТА ---");
                Console.Write("Введите название борта: ");
                string name = Console.ReadLine();

                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("\nОшибка: введено недопустимое название!");
                    return;
                }

                Console.WriteLine("\nВыберите ингредиент для борта: \n");
                Ingredient.ShowAll();
                if(!(Ingredient.GetLengthIngredients() == 0))
                {
                    Console.Write("\n Ваш выбор: ");
                    string choosenIngredient = Console.ReadLine();
                    if(int.TryParse(choosenIngredient, out int amount) && amount > 0 && amount <= Ingredient.GetLengthIngredients())
                    {
                        amount--;
                        Ingredient selectedItem = Ingredient.GetIngredientByIndex(amount);
                        Crust product = new Crust(name, selectedItem.Price, selectedItem);
                        Console.WriteLine($"\nБорт \"{name}\" стоимостью {product.Price} успешно создан! В него входит {selectedItem.Name}");

                    }
                    else
                    {
                        Console.WriteLine("Введен неверный номер ингредиента!");
                    }
                }
                else
                {
                    Console.WriteLine("Невозможно создать борт, так как отсутсвуют ингредиенты.");
                }

            }

            public void showAllForbiddenPizza()
            {
                if (forbiddenPizza.Count == 0)
                {
                    Console.WriteLine("Несовместимые пиццы отсутсвуют");
                    return;
                }

                Console.WriteLine("Несовместимые пиццы для " + this.Name);

                for (int i = 0; i < forbiddenPizza.Count; i++)
                {
                    Pizza currentPizza = Pizza.GetPizzaFromGuid(forbiddenPizza[i]);
                    Console.WriteLine($"{i+1}. {currentPizza.Name}");
                }
            }

            public void showAllAllowPizza()
            {
                if (allowPizza.Count == 0 && forbiddenPizza.Count == 0)
                {
                    Console.WriteLine("Борт доступен для всех пицц.");
                    return;
                }

                Console.WriteLine("Совместимые пиццы для " + this.Name + ":");

                for (int i = 0; i < allowPizza.Count; i++)
                {
                    Pizza currentPizza = Pizza.GetPizzaFromGuid(allowPizza[i]);
                    Console.WriteLine($"{i}. {currentPizza.Name}");
                }
            }

            public bool isCanUseCrust(Guid id)
            {
                if (forbiddenPizza.Contains(id))
                    return false;
                if (allowPizza.Count > 0 && !allowPizza.Contains(id))
                    return false;
                return true;
            }

            public static void DeleteItem()
            {
                Console.WriteLine("--- УДАЛЕНИЕ БОРТА ---\n");
                ShowAll();

                if (AllCrusts.Count > 0)
                {
                    Console.Write("\nВведите номер ингредиента для удаления: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number) && number > 0 && number <= AllCrusts.Count)
                    {
                        var selectedItem = AllCrusts.ElementAt(number - 1);
                        Guid selectedId = selectedItem.Key;
                        Crust CrustToDelete = selectedItem.Value;

                        Console.Write($"\nВы уверены, что хотите удалить ингредиент \"{CrustToDelete.Name}\"? (да/нет): ");

                        string confirmation = Console.ReadLine();

                        if (confirmation.ToLower() == "да")
                        {
                            AllCrusts.Remove(selectedItem.Key);
                            Console.WriteLine($"\nИнгредиент \"{CrustToDelete.Name}\" успешно удален!");

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
                else
                {
                    Console.WriteLine("Список бортов пуст.");
                }
            }

            public static void ShowAll()
            {
                if (AllCrusts.Count > 0)
                {
                    int counter = 1;
                    foreach (KeyValuePair<Guid, Crust> item in AllCrusts)
                    {
                        Console.WriteLine($"{counter}. Борт: {item.Value.Name}, Цена: {item.Value.Price}. Состоит из {item.Value.ingredientOfCrust.Name}");
                        if (item.Value.forbiddenPizza.Count > 0)
                        {
                            item.Value.showAllForbiddenPizza();
                        }
                        else
                        {
                            item.Value.showAllAllowPizza();
                        }
                            counter++;
                    }
                }
                else
                {
                    Console.WriteLine("Список бортов пуст.");
                }
            }

            public static void ChangeItem()
            {
                Console.WriteLine("--- РЕДАКТИРОВАНИЕ БОРТОВ ---\n");

                ShowAll();

                if (AllCrusts.Count > 0)
                {
                    Console.Write("\nВыберите борт для редактирования: ");
                    string userChoice = Console.ReadLine();

                    if (!int.TryParse(userChoice, out int choice) || choice <= 0 || choice > AllCrusts.Count)
                    {
                        Console.WriteLine("\nОшибка: неверный номер борта.");
                        return;
                    }

                    int choiceInt = choice - 1;
                    var selectedItem = AllCrusts.ElementAt(choiceInt);
                    Guid selectedId = selectedItem.Key;
                    Crust selectedCrust = selectedItem.Value;

                    Console.Clear();

                    Console.WriteLine($"--- РЕДАКТИРОВАНИЕ БОРТА {selectedCrust.Name} ---");
                    Console.WriteLine("1. Изменить название");
                    Console.WriteLine("2. Изменить ингредиент");
                    Console.WriteLine("3. Изменить список разрешенных пицц");
                    Console.WriteLine("4. Изменить список запрещенных пицц");
                    Console.WriteLine("5. Назад");
                    Console.Write("\nВыберите действие: ");

                    string operationNumber = Console.ReadLine();

                    switch (operationNumber)
                    {
                        case "1":
                            Console.Write("Введите новое название: ");
                            string newName = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(newName) && newName != selectedCrust.Name)
                            {
                                selectedCrust.UpdateName(newName);
                                Console.WriteLine("\nНазвание успешно изменено!");
                            }
                            else if (newName == selectedCrust.Name)
                            {
                                Console.WriteLine("\nВведено идентичное имя!");
                            }
                            else
                            {
                                Console.WriteLine("\nОшибка: введено некорректное название.");
                            }
                            break;

                        case "2":
                            Console.WriteLine("\nВыберите новый ингредиент для борта: ");
                            Ingredient.ShowAll();
                            Console.Write("\nВаш выбор: ");
                            string choosenIngredient = Console.ReadLine();
                            if (int.TryParse(choosenIngredient, out int amount) && amount > 0 && amount <= Ingredient.GetLengthIngredients())
                            {
                                amount--;
                                Ingredient selectedIngredient = Ingredient.GetIngredientByIndex(amount);
                                selectedCrust.ingredientOfCrust = selectedIngredient;
                                selectedCrust.UpdatePrice(selectedIngredient.Price);
                                Console.WriteLine($"\nБорт \"{selectedCrust.Name}\" стоимостью {selectedCrust.Price} успешно изменен! Теперь в него входит {selectedCrust.ingredientOfCrust.Name}");

                            }
                            else
                            {
                                Console.WriteLine("Введен неверный номер ингредиента!");
                            }
                            break;

                        case "3":
                            bool editWhiteList = true;
                            while (editWhiteList)
                            {
                                Console.Clear();

                                Console.WriteLine($"--- РЕДАКТИРОВАНИЕ БЕЛОГО СПИСКА \"{selectedCrust.Name}\" ---\n");
                                Console.WriteLine("Текущий белый список:");
                                selectedCrust.showAllAllowPizza();

                                Console.WriteLine("\n1. Добавить пиццу в список");
                                Console.WriteLine("2. Удалить пиццу из списка");
                                Console.WriteLine("3. Завершить редактирование");
                                Console.Write("\nВыберите действие: ");

                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.Clear();
                                        Console.WriteLine("Доступные пиццы для добавления");
                                        Pizza.ShowAll();
                                        if(!(Pizza.GetLengthPizza() == 0))
                                        {
                                            Console.WriteLine("\nВыберите пиццу для добавления в список: ");
                                            Console.Write("Ваш выбор: ");
                                            string userPizzaIndex = Console.ReadLine();
                                            if (int.TryParse(userPizzaIndex, out int pizzaIndex) && pizzaIndex > 0 && pizzaIndex <= Pizza.GetLengthPizza())
                                            {
                                                pizzaIndex--;
                                                Pizza selectedPizza = Pizza.GetPizzaFromIndex(pizzaIndex);
                                                if (!selectedCrust.allowPizza.Contains(selectedPizza.Id))
                                                {
                                                    selectedCrust.allowPizza.Add(selectedPizza.Id);
                                                    selectedCrust.forbiddenPizza.Clear();
                                                    Console.WriteLine($"Пицца {selectedPizza.Name} успешно добавленная в белый список!");
                                                    Console.ReadKey();
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Данная пицца уже в белом списке");
                                                    Console.ReadKey();
                                                }
                                            }
                                        }
                                        break;

                                    case "2":
                                        Console.Clear();

                                        selectedCrust.showAllAllowPizza();
                                        if (!(selectedCrust.allowPizza.Count == 0))
                                        {
                                            Console.Write("\nВыберите пиццу для удаления из списка: ");
                                            string userPizzaIndex = Console.ReadLine();
                                            if (int.TryParse(userPizzaIndex, out int pizzaIndex) && pizzaIndex > 0 && pizzaIndex <= selectedCrust.allowPizza.Count())
                                            {
                                                pizzaIndex--;
                                                Pizza selectedPizza = Pizza.GetPizzaFromIndex(pizzaIndex);
                                                selectedCrust.allowPizza.Remove(selectedPizza.Id);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Отсутсвуют элементы для удаления.");
                                        }
                                        break;
                                    case "3":
                                        editWhiteList = false;
                                        Console.WriteLine("\nРедактирование белого списка завершено.");
                                        break;

                                    default:
                                        Console.WriteLine("\nОшибка: введена неверная команда.");
                                        break;
                                }
                            }
                            break;

                        case "4":
                            bool editBlackList = true;
                            while (editBlackList)
                            {
                                Console.Clear();

                                Console.WriteLine($"--- РЕДАКТИРОВАНИЕ ЧЕРНОГО СПИСКА \"{selectedCrust.Name}\" ---\n");
                                Console.WriteLine("Текущий черный список:");
                                selectedCrust.showAllForbiddenPizza();

                                Console.WriteLine("\n1. Добавить пиццу в список");
                                Console.WriteLine("2. Удалить пиццу из списка");
                                Console.WriteLine("3. Завершить редактирование");
                                Console.Write("\nВыберите действие: ");

                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.Clear();
                                        Console.WriteLine("Доступные пиццы для добавления");
                                        Pizza.ShowAll();
                                        if (!(Pizza.GetLengthPizza() == 0))
                                        {
                                            Console.WriteLine("\nВыберите пиццу для добавления в список: ");
                                            Console.Write("Ваш выбор: ");
                                            string userPizzaIndex = Console.ReadLine();
                                            if (int.TryParse(userPizzaIndex, out int pizzaIndex) && pizzaIndex > 0 && pizzaIndex <= Pizza.GetLengthPizza())
                                            {
                                                pizzaIndex--;
                                                Pizza selectedPizza = Pizza.GetPizzaFromIndex(pizzaIndex);
                                                if (!selectedCrust.forbiddenPizza.Contains(selectedPizza.Id))
                                                {
                                                    selectedCrust.forbiddenPizza.Add(selectedPizza.Id);
                                                    selectedCrust.allowPizza.Clear();
                                                    Console.WriteLine($"Пицца {selectedPizza.Name} успешно добавленная в черном список!");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Данная пицца уже в черном списке");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Список пицц пуст.");
                                        }
                                        break;

                                    case "2":
                                        Console.Clear();

                                        selectedCrust.showAllForbiddenPizza();
                                        if (!(selectedCrust.forbiddenPizza.Count == 0))
                                        {
                                            Console.Write("\nВыберите пиццу для удаления из списка: ");
                                            string userPizzaIndex = Console.ReadLine();
                                            if (int.TryParse(userPizzaIndex, out int pizzaIndex) && pizzaIndex <= 0 && pizzaIndex <= selectedCrust.forbiddenPizza.Count())
                                            {
                                                pizzaIndex--;
                                                Pizza selectedPizza = Pizza.GetPizzaFromIndex(pizzaIndex);
                                                selectedCrust.forbiddenPizza.Remove(selectedPizza.Id);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Отсутсвуют элементы для удаления.");
                                        }
                                        break;
                                    case "3":
                                        editBlackList = false;
                                        Console.WriteLine("\nРедактирование черного списка завершено.");
                                        break;

                                    default:
                                        Console.WriteLine("\nОшибка: введена неверная команда.");
                                        break;
                                }
                            }
                            break;
                        case "5":

                            break;

                        default:
                            Console.WriteLine("\nОшибка: введена неверная команда.");
                            break;
                    }
                }
            }

            public static void FilterCrustsByIngredient()
            {
                Ingredient.ShowAll();

                Console.Write("\nВведите название ингредиента: ");
                string ingredientName = Console.ReadLine();
                if (!int.TryParse(ingredientName, out int ingNum) || ingNum <= 0 || ingNum > Ingredient.GetLengthIngredients())
                {
                    Console.WriteLine("Введен неверный номер ингредиента!");
                    return;
                }
                Ingredient selectedIngredient = Ingredient.GetIngredientByIndex(--ingNum);
                Console.WriteLine($"Бортики, содержащие ингредиент \"{ingredientName}\":");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var crust in AllCrusts.Values)
                {
                    if (crust.ingredientOfCrust == selectedIngredient)
                    {
                        {
                            foundCount++;
                            Console.WriteLine($"{foundCount}. {crust.Name} - {crust.Price} руб.");
                        }
                    }

                    if (foundCount == 0)
                    {
                        Console.WriteLine($"Бортиков с ингредиентом \"{ingredientName}\" не найдено.");
                    }
                }
            }

            public static void FilterCrustsByPrice()
            {
                Console.Write("Введите минимальную цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    Console.WriteLine("Введена неверная цена.");
                    return;
                }

                Console.Write("Введите максимальную цену: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
                {
                    Console.WriteLine("Введена неверная цена.");
                    return;
                }

                Console.WriteLine($"Бортики в ценовом диапазоне от {minPrice} до {maxPrice} руб.:");

                int foundCount = 0;

                foreach (var crust in AllCrusts.Values)
                {
                    if (crust.Price >= minPrice && crust.Price <= maxPrice)
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {crust.Name} - {crust.Price} руб.");
                        Console.WriteLine($"   Ингредиент: {crust.ingredientOfCrust.Name}");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Бортиков в указанном ценовом диапазоне не найдено.");
                }
            }

        }
    }
}
