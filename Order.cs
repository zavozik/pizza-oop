using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    internal partial class Program
    {
        class Order
        {
            private static List<Order> AllOrders = new List<Order>(); 

            public int OrderNumber { get; private set; }
            public DateTime OrderTime { get; private set; }
            public DateTime? ScheduledTime { get; private set; } 
            public string Comment { get; set; }
            public decimal TotalPrice { get; private set; }

            private List<IHasPrice> Pizzas = new List<IHasPrice>();

            public Order()
            {
                OrderNumber = AllOrders.Count + 1;
                OrderTime = DateTime.Now;
                ScheduledTime = null;
                Comment = "";
                AllOrders.Add(this);
            }

            public Order(string comment)
            {
                OrderNumber = AllOrders.Count + 1;
                OrderTime = DateTime.Now;
                ScheduledTime = null;
                Comment = comment;
                AllOrders.Add(this);
            }

            public Order(DateTime scheduledTime)
            {
                OrderNumber = AllOrders.Count + 1;
                OrderTime = DateTime.Now;
                ScheduledTime = scheduledTime;
                Comment = "";
                AllOrders.Add(this);
            }

            public Order(string comment, DateTime? scheduledTime)
            {
                OrderNumber = AllOrders.Count + 1;
                OrderTime = DateTime.Now;
                ScheduledTime = scheduledTime;
                Comment = comment;
                AllOrders.Add(this);
            }

            public void AddPizza(Pizza pizza)
            {
                if (pizza == null)
                {
                    Console.WriteLine("Ошибка: пицца не может быть null");
                    return;
                }

                Pizzas.Add(pizza);
                UpdateTotalPrice();
                Console.WriteLine($"Пицца \"{pizza.Name}\" добавлена в заказ №{OrderNumber}");
            }

            public void AddPizza(ManualPizza pizza)
            {
                if (pizza == null)
                {
                    Console.WriteLine("Ошибка: пицца не может быть null");
                    return;
                }

                Pizzas.Add(pizza);
                UpdateTotalPrice();
                Console.WriteLine($"Ручная пицца \"{pizza.Name}\" добавлена в заказ №{OrderNumber}");
            }

            public void AddPizza(CombinePizza pizza)
            {
                if (pizza == null)
                {
                    Console.WriteLine("Ошибка: пицца не может быть null");
                    return;
                }

                Pizzas.Add(pizza);
                UpdateTotalPrice();
                Console.WriteLine($"Комбинированная пицца \"{pizza.Name}\" добавлена в заказ №{OrderNumber}");
            }

            public void RemovePizza(int index)
            {
                if (index >= 0 && index < Pizzas.Count)
                {
                    string pizzaName = GetPizzaName(Pizzas[index]);
                    Pizzas.RemoveAt(index);
                    UpdateTotalPrice();
                    Console.WriteLine($"Пицца \"{pizzaName}\" удалена из заказа №{OrderNumber}");
                }
                else
                {
                    Console.WriteLine("Ошибка: неверный индекс пиццы");
                }
            }

            private string GetPizzaName(IHasPrice pizza)
            {
                if (pizza is Pizza piz) return piz.Name;
                if (pizza is ManualPizza manpizza) return manpizza.Name;
                if (pizza is CombinePizza compiz) return compiz.Name;
                return "Неизвестная пицца";
            }

            private string GetPizzaType(IHasPrice pizza)
            {
                if (pizza is Pizza) return "Шаблонная";
                if (pizza is ManualPizza) return "Ручная";
                if (pizza is CombinePizza) return "Комбинированная";
                return "Неизвестный тип";
            }

            private void UpdateTotalPrice()
            {
                TotalPrice = Pizzas.Sum(p => p.Price);
            }

            public void Display()
            {
                Console.WriteLine($" --- ЗАКАЗ {OrderNumber} ---");

                Console.WriteLine($"Время заказа: {OrderTime:dd.MM.yyyy HH:mm}");

                if (ScheduledTime.HasValue)
                {
                    Console.WriteLine($"Отложен до: {ScheduledTime.Value:dd.MM.yyyy HH:mm}");
                }

                Console.WriteLine($"Комментарий: {(string.IsNullOrEmpty(Comment) ? "нет" : Comment)}");
                Console.WriteLine($"Общая стоимость: {TotalPrice} руб.");

                Console.WriteLine($"\nПиццы в заказе ({Pizzas.Count} шт.):");
                Console.WriteLine(new string('-', 50));

                for (int i = 0; i < Pizzas.Count; i++)
                {
                    var pizza = Pizzas[i];
                    string type = GetPizzaType(pizza);
                    string name = GetPizzaName(pizza);

                    Console.WriteLine($"{i + 1}. [{type}] {name} - {pizza.Price} руб.");

                    if (pizza is CombinePizza combine)
                    {
                        Console.WriteLine($"Основа: {combine.Base.Name}, Размер: {combine.Size}");
                        if (combine.Crust != null)
                            Console.WriteLine($"Борт: {combine.Crust.Name}");
                        Console.WriteLine($"  Состав:");
                        foreach (var part in combine.GetParts())
                        {
                            string partName = GetPizzaName(part.Pizza);
                            Console.WriteLine($" - {partName}: {part.Pieces} кусков");
                        }
                    }

                    if (pizza is ManualPizza manual)
                    {
                        Console.WriteLine($"   Основа: {manual.Base.Name}, Размер: {manual.Size}");
                        if (manual.Crust != null)
                            Console.WriteLine($"   Борт: {manual.Crust.Name}");
                        manual.DisplayIngredients();
                    }

                    if (pizza is Pizza template)
                    {
                        Console.WriteLine($"   Основа: {template.Basic.Name}, Размер: {template.Size}");
                    }

                }
            }

            public static void CreateOrder()
            {
                Console.WriteLine("--- СОЗДАНИЕ ЗАКАЗА ---\n");

                Console.Write("Хотите сделать отложенный заказ? (да/нет): ");
                bool isScheduled = Console.ReadLine().ToLower() == "да";

                DateTime? scheduledTime = null;
                if (isScheduled)
                {
                    Console.Write("Введите дату и время (дд.мм.гггг чч:мм): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime schedule) && schedule > DateTime.Now)
                    {
                        scheduledTime = schedule;
                    }
                    else
                    {
                        Console.WriteLine("Неверный формат даты! Заказ будет оформлен на текущее время.");
                    }
                }

                Console.Write("Введите комментарий к заказу (или оставьте пустым): ");
                string comment = Console.ReadLine();

                Order order = new Order(comment, scheduledTime);

                bool addingPizzas = true;
                while (addingPizzas)
                {
                    Console.Clear();
                    Console.WriteLine($"--- ДОБАВЛЕНИЕ ПИЦЦЫ В ЗАКАЗ {order.OrderNumber} ---\n");
                    Console.WriteLine("1. Добавить шаблонную пиццу");
                    Console.WriteLine("2. Создать ручную пиццу");
                    Console.WriteLine("3. Создать комбинированную пиццу");
                    Console.WriteLine("4. Завершить заказ");
                    Console.Write("\nВыберите действие: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            AddTemplatePizzaToOrder(order);
                            break;
                        case "2":
                            CreateManualPizzaForOrder(order);
                            break;
                        case "3":
                            CreateCombinePizzaForOrder(order);
                            break;
                        case "4":
                            addingPizzas = false;
                            break;
                        default:
                            Console.WriteLine("Неверная команда!");
                            Console.ReadKey();
                            break;
                    }
                }

                Console.Clear();
                Console.WriteLine("--- ЗАКАЗ УСПЕШНО СОЗДАН ---");
                order.Display();
                Console.ReadKey();
            }

            public static void ShowAllOrders()
            {
                if (AllOrders.Count == 0)
                {
                    Console.WriteLine("Список заказов пуст.");
                    return;
                }

                Console.WriteLine($"Всего заказов: {AllOrders.Count}");

                foreach (var order in AllOrders)
                {
                    order.Display();
                }
            }

            static void AddTemplatePizzaToOrder(Order order)
            {
                Console.Clear();
                Console.WriteLine("--- ДОБАВЛЕНИЕ ШАБЛОННОЙ ПИЦЦЫ ---\n");

                Pizza.ShowAll();

                if (Pizza.GetLengthPizza() == 0)
                {
                    Console.WriteLine("Нет доступных шаблонных пицц!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("\nВыберите пиццу: ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice > 0 && choice <= Pizza.GetLengthPizza())
                {
                    Pizza selectedPizza = Pizza.GetPizzaFromIndex(choice - 1);

                    Console.WriteLine("\nВыберите размер:");
                    Console.WriteLine("1. Маленькая");
                    Console.WriteLine("2. Средняя");
                    Console.WriteLine("3. Большая");
                    Console.Write("Ваш выбор: ");

                    PizzaSize size = PizzaSize.Medium;
                    if (int.TryParse(Console.ReadLine(), out int sizeChoice))
                    {
                        switch (sizeChoice)
                        {
                            case 1: size = PizzaSize.Small; break;
                            case 2: size = PizzaSize.Medium; break;
                            case 3: size = PizzaSize.Big; break;
                        }
                    }
                    Console.WriteLine("\nДоступные борты:");
                    Crust.ShowAll();
                    Console.Write("Выберите борт (0 - без борта): ");

                    Crust selectedCrust = null;
                    if (int.TryParse(Console.ReadLine(), out int crustChoice) && crustChoice > 0 && crustChoice <= Crust.GetLength())
                    {
                        selectedCrust = Crust.GetCrustByIndex(crustChoice - 1);
                        if (!selectedCrust.isCanUseCrust(selectedPizza.Id))
                        {
                            Console.WriteLine("Данный борт недоступен. Будет выбран стандартный.");
                            selectedCrust = null;
                        }

                    }
                    else if(crustChoice != 0)
                    {
                        Console.WriteLine("Введен неверный номер бортика. Будет выбран стандартный.");
                    }
                    
                    Console.Write("\nХотите удвоить количество ингредиентов? (да/нет): ");
                    bool doubleIngredients = Console.ReadLine().ToLower() == "да";

                    if (doubleIngredients)
                    {
                        ManualPizza doubledPizza = new ManualPizza($"{selectedPizza.Name} (удвоенная)", size, selectedPizza.Basic, selectedCrust);

                        var ingredients = selectedPizza.GetIngredientsWithPortions();
                        foreach (var item in ingredients)
                        {
                            doubledPizza.AddIngredient(item.Key, item.Value * 2);
                        }

                        order.AddPizza(doubledPizza);
                        Console.WriteLine("Ингредиенты удвоены!");
                    }
                    else
                    {
                        ManualPizza newPizza = new ManualPizza(selectedPizza.Name, size, selectedPizza.Basic, selectedCrust);
                        order.AddPizza(selectedPizza);
                    }
                    
                    Console.WriteLine("Пицца добавлена в заказ!");
                }
            }

            static void CreateManualPizzaForOrder(Order order)
            {
                Console.Clear();
                Console.WriteLine("--- СОЗДАНИЕ РУЧНОЙ ПИЦЦЫ ---\n");

                Console.Write("Введите название пиццы: ");
                string name = Console.ReadLine();

                Console.WriteLine("\nВыберите размер:");
                Console.WriteLine("1. Маленькая");
                Console.WriteLine("2. Средняя");
                Console.WriteLine("3. Большая");
                Console.Write("Ваш выбор: ");

                PizzaSize size = PizzaSize.Medium;
                if (int.TryParse(Console.ReadLine(), out int sizeChoice))
                {
                    switch (sizeChoice)
                    {
                        case 1: size = PizzaSize.Small; break;
                        case 2: size = PizzaSize.Medium; break;
                        case 3: size = PizzaSize.Big; break;
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер пиццы!");
                    return;
                }

                Console.WriteLine("\nДоступные основы:");
                PizzaBase.ShowAll();
                Console.Write("Выберите основу: ");

                if (!int.TryParse(Console.ReadLine(), out int baseChoice) ||
                    baseChoice <= 0 || baseChoice > PizzaBase.Bases.Count)
                {
                    Console.WriteLine("Неверный выбор основы!");
                    Console.ReadKey();
                    return;
                }

                PizzaBase selectedBase = PizzaBase.Bases[baseChoice - 1];

                Console.WriteLine("\nДоступные борты:");
                Crust.ShowAll();
                Console.Write("Выберите борт (0 - без борта): ");

                Crust selectedCrust = null;
                if (int.TryParse(Console.ReadLine(), out int crustChoice) && crustChoice > 0 && crustChoice <= Crust.GetLength())
                {
                    selectedCrust = Crust.GetCrustByIndex(crustChoice - 1);
                }
                else
                {
                    Console.WriteLine("Введен неверный номер бортика. Будет выбран стандартный.");
                }

                ManualPizza manualPizza = new ManualPizza(name, size, selectedBase, selectedCrust);

                bool addingIngredients = true;
                while (addingIngredients)
                {
                    Console.Clear();
                    Console.WriteLine($"--- ДОБАВЛЕНИЕ ИНГРЕДИЕНТОВ В {name} ---\n");

                    Ingredient.ShowAll();

                    if (Ingredient.GetLengthIngredients() == 0)
                    {
                        Console.WriteLine("Нет доступных ингредиентов!");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("\nВыберите ингредиент (0 - закончить): ");
                    if (!int.TryParse(Console.ReadLine(), out int ingChoice) || ingChoice == 0)
                    {
                        addingIngredients = false;
                        continue;
                    }

                    if (ingChoice > 0 && ingChoice <= Ingredient.GetLengthIngredients())
                    {
                        Ingredient selectedIng = Ingredient.GetIngredientByIndex(ingChoice - 1);

                        Console.Write("Введите количество порций: ");
                        if (int.TryParse(Console.ReadLine(), out int portions) && portions > 0)
                        {
                            manualPizza.AddIngredient(selectedIng, portions);
                            Console.WriteLine("Ингредиент добавлен!");
                            Console.ReadKey();
                        }
                    }
                }
                order.AddPizza(manualPizza);
                Console.ReadKey();
            }

            static void CreateCombinePizzaForOrder(Order order)
            {
                Console.Clear();
                Console.WriteLine("--- СОЗДАНИЕ КОМБИНИРОВАННОЙ ПИЦЦЫ ---\n");

                Console.Write("Введите название комбинированной пиццы: ");
                string name = Console.ReadLine();

                Console.WriteLine("\nВыберите размер:");
                Console.WriteLine("1. Маленькая");
                Console.WriteLine("2. Средняя");
                Console.WriteLine("3. Большая");
                Console.Write("Ваш выбор: ");

                PizzaSize size = PizzaSize.Medium;
                if (int.TryParse(Console.ReadLine(), out int sizeChoice))
                {
                    switch (sizeChoice)
                    {
                        case 1: size = PizzaSize.Small; break;
                        case 2: size = PizzaSize.Medium; break;
                        case 3: size = PizzaSize.Big; break;
                    }
                }

                Console.WriteLine("\nДоступные основы:");
                PizzaBase.ShowAll();
                Console.Write("Выберите основу: ");

                if (!int.TryParse(Console.ReadLine(), out int baseChoice) ||
                    baseChoice <= 0 || baseChoice > PizzaBase.Bases.Count)
                {
                    Console.WriteLine("Неверный выбор основы!");
                    Console.ReadKey();
                    return;
                }

                PizzaBase selectedBase = PizzaBase.Bases[baseChoice - 1];

                Console.WriteLine("\nДоступные борты:");
                Crust.ShowAll();
                Console.Write("Выберите борт (0 - без борта): ");

                Crust selectedCrust = null;
                if (int.TryParse(Console.ReadLine(), out int crustChoice) && crustChoice > 0)
                {
                    Console.WriteLine("Выбор борта временно недоступен");
                }

                CombinePizza combinePizza = new CombinePizza(name, size, selectedBase, selectedCrust);

                bool addingParts = true;
                int totalPieces = 0;
                const int maxPieces = 8;

                while (addingParts && totalPieces < maxPieces)
                {
                    Console.Clear();
                    Console.WriteLine($"--- ДОБАВЛЕНИЕ ЧАСТЕЙ В {name} ---");
                    Console.WriteLine($"Всего кусков в пицце: {maxPieces}");
                    Console.WriteLine($"Уже распределено: {totalPieces} кусков");
                    Console.WriteLine($"Осталось распределить: {maxPieces - totalPieces} кусков\n");

                    Console.WriteLine("1. Добавить шаблонную пиццу");
                    Console.WriteLine("2. Завершить (даже если не все куски распределены)");
                    Console.Write("\nВыберите действие: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.Clear();
                            Console.WriteLine("Доступные шаблонные пиццы:");
                            Pizza.ShowAll();

                            if (Pizza.GetLengthPizza() > 0)
                            {
                                Console.Write("\nВыберите пиццу: ");
                                if (int.TryParse(Console.ReadLine(), out int pizzaChoice) &&
                                    pizzaChoice > 0 && pizzaChoice <= Pizza.GetLengthPizza())
                                {
                                    Pizza selectedPizza = Pizza.GetPizzaFromIndex(pizzaChoice - 1);

                                    Console.Write($"Введите количество кусков (макс. {maxPieces - totalPieces}): ");
                                    if (int.TryParse(Console.ReadLine(), out int piece) &&
                                        piece > 0 && piece <= maxPieces - totalPieces)
                                    {
                                        combinePizza.AddPart(selectedPizza, piece);
                                        totalPieces += piece;

                                        Console.WriteLine($"\nДобавлено {piece} кусков пиццы \"{selectedPizza.Name}\"");
                                        Console.ReadKey();
                                    }
                                }
                            }
                            break;

                        case "2":
                            addingParts = false;
                            break;
                    }
                }
                if(totalPieces == 0)
                {
                    Console.WriteLine("Нельзя создать пиццу без кусков!");
                    return;
                }
                order.AddPizza(combinePizza);
                Console.ReadKey();
            }

            public static void ShowScheduledOrders()
            {
                var scheduled = AllOrders.Where(o => o.ScheduledTime.HasValue).ToList();

                if (scheduled.Count == 0)
                {
                    Console.WriteLine("Отложенных заказов нет.");
                    return;
                }

                Console.WriteLine($"Всего отложенных заказов ({scheduled.Count})");

                foreach (var order in scheduled)
                {
                    Console.WriteLine($"Заказ №{order.OrderNumber} - {order.ScheduledTime.Value:dd.MM.yyyy HH:mm} - {order.TotalPrice} руб. - {order.Comment}");
                }
            }

            public static void FilterOrdersByDate()
            {
                Console.WriteLine("Даты, на которые совершены заказы: ");

                foreach (var order in Order.AllOrders)
                {
                    if (order.ScheduledTime != null) { Console.WriteLine(order.ScheduledTime); }
                    else { Console.WriteLine(order.OrderTime); }
                }

                Console.Write("Введите дату (дд.мм.гггг): ");

                if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    Console.WriteLine("Неверный формат даты!");
                    return;
                }

                var filtered = AllOrders.Where(o => o.OrderTime.Date == date.Date).ToList();
                var filteredScheduled = AllOrders.Where(o => o.ScheduledTime != null && o.ScheduledTime.Value.Date == date.Date).ToList();


                if (filtered.Count + filteredScheduled.Count == 0)
                {
                    Console.WriteLine($"Заказов за {date:dd.MM.yyyy} не найдено.");
                    return;
                }
                Console.WriteLine($"Заказы за {date:dd.MM.yyyy}");
                foreach (var order in filtered)
                {
                    Console.WriteLine($"Заказ №{order.OrderNumber} - {order.OrderTime:HH:mm} - {order.TotalPrice} руб. - Пицц: {order.Pizzas.Count}");
                }
                foreach (var order in filteredScheduled)
                {
                    Console.WriteLine($"Заказ №{order.OrderNumber} - {order.ScheduledTime:HH:mm} - {order.TotalPrice} руб. - Пицц: {order.Pizzas.Count}");
                }
            }

            public static void FilterOrdersByIngredient()
            {
                Ingredient.ShowAll();
                
                Console.Write("\nВведите номер ингредиента: ");
                string ingredientName = Console.ReadLine();

                if (!int.TryParse(ingredientName, out int ingNum) || ingNum <= 0 || ingNum > Ingredient.GetLengthIngredients())
                {
                    Console.WriteLine("Введен неверный номер ингредиента!");
                    return;
                }
                Ingredient selectedIngredient = Ingredient.GetIngredientByIndex(--ingNum);
                Console.WriteLine($"Заказы, содержащие {selectedIngredient.Name}");

                int foundCount = 0;

                foreach (var order in AllOrders)
                {
                    bool hasIngredient = false;

                    foreach (var pizza in order.Pizzas)
                    {
                        if (pizza is Pizza templatePizza)
                        {
                            if (templatePizza.HasIngredient(selectedIngredient))
                                hasIngredient = true;
                        }
                        else if (pizza is ManualPizza manualPizza)
                        {
                            if (manualPizza.HasIngredient(selectedIngredient))
                                hasIngredient = true;
                        }
                        else if (pizza is CombinePizza combinePizza)
                        {
                            if (combinePizza.HasIngredient(selectedIngredient))
                                hasIngredient = true;
                        }
                    }

                    if (hasIngredient)
                    {
                        foundCount++;
                        Console.WriteLine($"Заказ №{order.OrderNumber} - {order.OrderTime:dd.MM.yyyy HH:mm} - {order.TotalPrice} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Заказов с ингредиентом \"{selectedIngredient.Name}\" не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено заказов: {foundCount}");
                }
            }

            public static Order GetOrderByNumber(int orderNumber)
            {
                return AllOrders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            }

            public static bool DeleteOrder(int orderNumber)
            {
                var order = GetOrderByNumber(orderNumber);
                if (order != null)
                {
                    return AllOrders.Remove(order);
                }
                return false;
            }

            public static void FilterOrdersByBase()
            {
                Console.WriteLine("Доступные основы:");
                PizzaBase.ShowAll();
                Console.Write("\nВведите название основы для фильтрации: ");
                string baseName = Console.ReadLine();
                if (!int.TryParse(baseName, out int baseNum) || baseNum <= 0 || baseNum > PizzaBase.GetLengthBasic())
                {
                    Console.WriteLine("Введен неверный номер основы!");
                    return;
                }

                PizzaBase selectedBase = PizzaBase.GetBasicByIndex(--baseNum);

                Console.WriteLine($"Заказы с основой \"{selectedBase.Name}\":");

                int foundCount = 0;

                foreach (var order in AllOrders)
                {
                    bool hasBase = false;

                    foreach (var pizza in order.Pizzas)
                    {
                        if (pizza is Pizza templatePizza && templatePizza.HasBase(selectedBase))
                        {
                            hasBase = true;
                            break;
                        }
                        else if (pizza is ManualPizza manualPizza && manualPizza.HasBase(selectedBase))
                        {
                            hasBase = true;
                            break;
                        }
                        else if (pizza is CombinePizza combinePizza && combinePizza.HasBase(selectedBase))
                        {
                            hasBase = true;
                            break;
                        }
                    }

                    if (hasBase)
                    {
                        foundCount++;
                        Console.WriteLine($"Заказ №{order.OrderNumber} - {order.OrderTime:dd.MM.yyyy HH:mm} - {order.TotalPrice} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Заказов с основой \"{baseName}\" не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено заказов: {foundCount}");
                }
            }

            public static void FilterOrdersByCrust()
            {
                Console.WriteLine("Доступные бортики:");
                Crust.ShowAll();
                Console.Write("\nВведите номер бортика для фильтрации: ");
                string crustName = Console.ReadLine();
                if (!int.TryParse(crustName, out int crustNum) || crustNum <= 0 || crustNum > Crust.GetLength())
                {
                    Console.WriteLine("Введен неверный номер основы!");
                    return;
                }
                Crust selectedCrust = Crust.GetCrustByIndex(--crustNum);

                Console.WriteLine($"Заказы с бортиком \"{selectedCrust.Name}\":");

                int foundCount = 0;

                foreach (var order in AllOrders)
                {
                    bool hasCrust = false;

                    foreach (var pizza in order.Pizzas)
                    {
                        if (pizza is Pizza)
                        {
                            continue;
                        }
                        else if (pizza is ManualPizza manualPizza && manualPizza.HasCrust(selectedCrust))
                        {
                            hasCrust = true;
                            break;
                        }
                        else if (pizza is CombinePizza combinePizza && combinePizza.HasCrust(selectedCrust))
                        {
                            hasCrust = true;
                            break;
                        }
                    }

                    if (hasCrust)
                    {
                        foundCount++;
                        Console.WriteLine($"Заказ №{order.OrderNumber} - {order.OrderTime:dd.MM.yyyy HH:mm} - {order.TotalPrice} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Заказов с бортиком \"{crustName}\" не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено заказов: {foundCount}");
                }
            }

            public static void FilterOrdersBySize()
            {
                Console.WriteLine("Выберите размер для фильтрации:");
                Console.WriteLine("1. Маленькая");
                Console.WriteLine("2. Средняя");
                Console.WriteLine("3. Большая");
                Console.Write("Ваш выбор: ");

                PizzaSize size;

                if (int.TryParse(Console.ReadLine(), out int sizeChoice) && sizeChoice > 0 && sizeChoice < 4)
                {
                    size = sizeChoice switch
                    {
                        1 => PizzaSize.Small,
                        2 => PizzaSize.Medium,
                        3 => PizzaSize.Big,
                        _ => PizzaSize.Medium
                    };
                }
                else
                {
                    Console.WriteLine("Выбран неверный номер размера");
                    return;
                }

                string sizeName = size switch
                    {
                        PizzaSize.Small => "маленькую",
                        PizzaSize.Medium => "среднюю",
                        PizzaSize.Big => "большую",
                        _ => size.ToString()
                    };

                Console.WriteLine($"Заказы, содержащие {sizeName} пиццу:");

                int foundCount = 0;

                foreach (var order in AllOrders)
                {
                    bool hasSize = false;
                    List<string> pizzasWithSize = new List<string>();

                    foreach (var pizza in order.Pizzas)
                    {
                        bool pizzaHasSize = false;

                        if (pizza is Pizza tp && tp.Size == size)
                        {
                            pizzaHasSize = true;
                            pizzasWithSize.Add($"{tp.Name} (шаблонная)");
                        }
                        else if (pizza is ManualPizza mp && mp.Size == size)
                        {
                            pizzaHasSize = true;
                            pizzasWithSize.Add($"{mp.Name} (ручная)");
                        }
                        else if (pizza is CombinePizza cp && cp.Size == size)
                        {
                            pizzaHasSize = true;
                            pizzasWithSize.Add($"{cp.Name} (комбинированная)");
                        }

                        if (pizzaHasSize) hasSize = true;
                    }

                    if (hasSize)
                    {
                        foundCount++;
                        Console.WriteLine($"Заказ №{order.OrderNumber} - {order.OrderTime:dd.MM.yyyy HH:mm} - {order.TotalPrice} руб.");
                        Console.WriteLine($"  Комментарий: {(string.IsNullOrEmpty(order.Comment) ? "нет" : order.Comment)}");
                        Console.WriteLine($"  Пиццы нужного размера:");
                        foreach (var pizzaName in pizzasWithSize)
                        {
                            Console.WriteLine($"{pizzaName}");
                        }
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"Заказов с {sizeName} пиццей не найдено.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено заказов: {foundCount}");
                }
            }

        }
    }
}
