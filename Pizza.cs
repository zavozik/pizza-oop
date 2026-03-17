using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleApp2.Program;

namespace ConsoleApp2
{
    internal partial class Program
    {
        public class Pizza : IGuid, INamed, IHasPrice
        {
            public string Name { get; private set; }
            public PizzaBase Basic { get; private set; }
            public decimal Price { get; private set; }

            public PizzaSize Size { get; private set; }

            public Guid Id { get; }

            private Dictionary<Guid, Ingredient> IngredientsOfPizza = new Dictionary<Guid, Ingredient>();

            private Dictionary<Guid, int> PortionOfIngredient = new Dictionary<Guid, int>();

            private static Dictionary<Guid, Pizza> AllTemplatePizza = new Dictionary<Guid, Pizza>();

            public Pizza(string name, PizzaBase core, PizzaSize size = PizzaSize.Medium)
            {
                Name = name;
                Basic = core;
                Size = size;
                Price = core.Price;
                Id = Guid.NewGuid();
            }

            public void CreateTemplate()
            {
                AllTemplatePizza.Add(Id, this);
            }

            public void UpdatePrice()
            {
                decimal result = Basic.Price;

                for (int i = 0; i < IngredientsOfPizza.Count; i++)
                {
                    var selectedItem = IngredientsOfPizza.ElementAt(i);
                    Guid selectedId = selectedItem.Key;
                    result += IngredientsOfPizza[selectedId].Price * PortionOfIngredient[selectedId];
                }

                switch (Size)
                {
                    case PizzaSize.Small:
                        result *= 0.7m;
                        break;
                    case PizzaSize.Medium:
						result *= 0.85m;
						break;
                    case PizzaSize.Big:
                        break;
                }

                Price = result;
            }

            public void Addingredient()
            {

                Console.WriteLine("--- ДОБАВЛЕНИЕ ИНГРЕДИЕНТА ---\n");
                Console.WriteLine("Доступные ингредиенты:");
                Ingredient.ShowAll();

                if (IngredientsOfPizza.Count > 0)
                {
                    Console.Write("\nВыберите ингредиент для добавления: ");
                    string ingredientChoice = Console.ReadLine();

                    if (int.TryParse(ingredientChoice, out int ingChoice) &&
                        ingChoice > 0 && ingChoice <= IngredientsOfPizza.Count)
                    {
                        ingChoice--;
                        Ingredient selectedItem = Ingredient.GetIngredientByIndex(ingChoice);
                        Guid selectedId = selectedItem.Id;

                        if (!IngredientsOfPizza.ContainsKey(selectedId))
                        {
                            Console.Write("Введите количество порций: ");
                            string portionInput = Console.ReadLine();

                            if (int.TryParse(portionInput, out int portions) && portions > 0)
                            {
                                IngredientsOfPizza.Add(selectedId, selectedItem);
                                PortionOfIngredient.Add(selectedId, portions);
                                this.UpdatePrice();
                                Console.WriteLine($"\nИнгредиент \"{selectedItem.Name}\" добавлен в количестве {portions} порций.");
                            }
                            else
                            {
                                Console.WriteLine("\nОшибка: неверное количество порций.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\nИнгредиент \"{selectedItem.Name}\" уже есть в пицце.");
                            Console.Write("Сколько порций добавить к существующим? ");

                            string addPortions = Console.ReadLine();
                            if (int.TryParse(addPortions, out int additionalPortions) && additionalPortions > 0)
                            {
                                this.AddToIngredientPortion(selectedItem, additionalPortions);
                                this.UpdatePrice();
                                Console.WriteLine($"\nДобавлено {additionalPortions} порций. Теперь всего: {PortionOfIngredient[selectedId]} порций.");
                            }
                            else
                            {
                                Console.WriteLine("\nОшибка: неверное количество порций.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nОшибка: неверный номер ингредиента.");
                    }
                }
                else
                {
                    Console.WriteLine("\nОшибка: нет доступных ингредиентов.");
                }
            }

            public static void DeletePizza()
            {
                Console.WriteLine("--- УДАЛЕНИЕ ПИЦЦЫ ---\n");

                ShowAll();

                if (AllTemplatePizza.Count > 0)
                {
                    Console.Write("\nВведите номер пиццы для удаления: ");
                    string deleteChoice = Console.ReadLine();

                    if (int.TryParse(deleteChoice, out int deleteId) && deleteId > 0 && deleteId <= AllTemplatePizza.Count)
                    {
                        deleteId--;

                        var selectedItem = AllTemplatePizza.ElementAt(deleteId);
                        Guid selectedId = selectedItem.Key;
                        string deletedPizzaName = AllTemplatePizza[selectedId].Name;


                        Console.Write($"\nВы уверены, что хотите удалить пиццу \"{deletedPizzaName}\"? (да/нет): ");
                        string confirmation = Console.ReadLine();

                        if (confirmation.ToLower() == "да")
                        {
                            AllTemplatePizza.Remove(selectedId);

                            Console.WriteLine($"\nПицца \"{deletedPizzaName}\" успешно удалена!");
                        }
                        else
                        {
                            Console.WriteLine("\nУдаление отменено.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nОшибка: неверный номер пиццы.");
                    }
                }
            }

            public void AddEveryIngredient(Ingredient ingredient, int portions)
            {
                if (!IngredientsOfPizza.ContainsKey(ingredient.Id))
                {
                    IngredientsOfPizza.Add(ingredient.Id, ingredient);
                    PortionOfIngredient.Add(ingredient.Id, portions);
                }
                else
                {
                    PortionOfIngredient[ingredient.Id] += portions;
                }
                UpdatePrice();
            }

            public void Removeingredient(int id)
            {
                id--;
                Guid selectedKey = IngredientsOfPizza.ElementAt(id).Key;
                IngredientsOfPizza.Remove(selectedKey);
                PortionOfIngredient.Remove(selectedKey);
            }

            public static void Removeingredient(Ingredient ing)
            {
                foreach(var pizza in AllTemplatePizza)
                {
                    if (pizza.Value.HasIngredient(ing)) {
                        pizza.Value.IngredientsOfPizza.Remove(ing.Id);
                        pizza.Value.PortionOfIngredient.Remove(ing.Id);
                    }
                }
            }

            public void UpdatePortion(int id, int newPortion)
            {
                id--;
                Guid selectedKey = PortionOfIngredient.ElementAt(id).Key;
                PortionOfIngredient[selectedKey] = newPortion;
            }

            public void UpdateBase()
            {
                Console.WriteLine("\nДоступные основы:");
                PizzaBase.ShowAll();
                Console.Write($"\nВыберите новую основу (текущая: {this.Basic.Name}): ");

                string newBase = Console.ReadLine();

                if (int.TryParse(newBase, out int newbase) && newbase > 0 && newbase <= PizzaBase.Bases.Count)
                {
                    newbase--;
                    this.Basic = PizzaBase.Bases[newbase];
                    this.UpdatePrice();
                    Console.WriteLine("\nОснова успешно заменена!");
                }
                else
                {
                    Console.WriteLine("\nОшибка: неверный номер основы.");
                }
            }

            public void UpdateName()
            {
                Console.Write("\nВведите новое название: ");
                string newName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    this.Name = newName;
                    Console.WriteLine("\nНазвание успешно изменено!");
                }
                else
                {
                    Console.WriteLine("\nОшибка: введено некорректное название.");
                }
            }

            public void ShowAllStructure()
            {
                Console.WriteLine($"Основа: {Basic.Name}");
                Console.WriteLine("Ингредиенты:");

                for (int i = 0; i < IngredientsOfPizza.Count; i++)
                {
                    var selectedItem = IngredientsOfPizza.ElementAt(i);
                    Guid selectedId = selectedItem.Key;

                    Console.WriteLine($"{IngredientsOfPizza[selectedId].Name} в количестве {PortionOfIngredient[selectedId]} порций");
                }

            }

            public void AddToIngredientPortion(Ingredient ingredient, int additionalPortions)
            {
                if (IngredientsOfPizza.TryGetValue(ingredient.Id, out var existingIngredient))
                {
                    PortionOfIngredient[ingredient.Id] += additionalPortions;
                    UpdatePrice();
                }
            }


            public static void ShowAll()
            {
                if (AllTemplatePizza.Count > 0)
                {
                    for (int i = 0; i < AllTemplatePizza.Count; i++)
                    {
                        var selectedItem = AllTemplatePizza.ElementAt(i);
                        Guid selectedId = selectedItem.Key;

                        Console.WriteLine($"\n[ID {i + 1}] Пицца: {AllTemplatePizza[selectedId].Name}, Цена: {AllTemplatePizza[selectedId].Price}");
                        Console.WriteLine($"Основа: {AllTemplatePizza[selectedId].Basic.Name}");
                        if (AllTemplatePizza[selectedId].IngredientsOfPizza.Count > 0)
                        {
                            Console.WriteLine("Ингредиенты:");
                            for (int j = 0; j < AllTemplatePizza[selectedId].IngredientsOfPizza.Count; j++)
                            {
                                var selectedIngredient = AllTemplatePizza[selectedId].IngredientsOfPizza.ElementAt(j);
                                Guid selectedIngredientId = selectedIngredient.Key;

                                Console.WriteLine($"{j + 1}. {AllTemplatePizza[selectedId].IngredientsOfPizza[selectedIngredientId].Name} - " +
                                    $"{AllTemplatePizza[selectedId].PortionOfIngredient[selectedIngredientId]} порций");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Список пицц пуст.");
                }
            }

            public static void CreatePizza(bool isTemplate)
            {
                Console.WriteLine("--- СОЗДАНИЕ ПИЦЦЫ ---\n");
                Console.Write("Введите название пиццы: ");

                string userName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    Console.WriteLine("\nОшибка: введено некорректное название.");
                    return;
                }

                Console.WriteLine("\nДоступные основы:");

                PizzaBase.ShowAll();

                if (PizzaBase.Bases.Count > 0)
                {
                    Console.Write("\nВыберите основу для пиццы: ");

                    string userChoice = Console.ReadLine();

                    if (!int.TryParse(userChoice, out int choice) || choice <= 0 || choice > PizzaBase.Bases.Count)
                    {
                        Console.WriteLine("\nОшибка: неверный номер основы.");
                        return;
                    }

                    choice--;
                    Pizza createdPizza = new Pizza(userName, PizzaBase.Bases[choice]);

                    Console.WriteLine("\nДоступные ингредиенты:");

                    Ingredient.ShowAll();

                    int ingredientsCount = Ingredient.GetLengthIngredients();

                    if (ingredientsCount > 0)
                    {
                        bool isIngredient = true;

                        while (isIngredient)
                        {
                            Console.Write("\nВыберите ингредиент для пиццы: ");

                            string ingChoice = Console.ReadLine();

                            if (int.TryParse(ingChoice, out int ingNum) && ingNum > 0 && ingNum <= ingredientsCount)
                            {
                                ingNum--;

                                Ingredient selectedIngredient = Ingredient.GetIngredientByIndex(ingNum);

                                Console.Write("Введите количество порций: ");

                                string portionInput = Console.ReadLine();

                                if (!int.TryParse(portionInput, out int portions) || portions <= 0)
                                {
                                    Console.WriteLine("Ошибка: неверное количество порций.");
                                    continue;
                                }

                                if (!createdPizza.IngredientsOfPizza.ContainsValue(selectedIngredient))
                                {
                                    createdPizza.IngredientsOfPizza.Add(selectedIngredient.Id, selectedIngredient);
                                    createdPizza.PortionOfIngredient.Add(selectedIngredient.Id, portions);
                                }
                                else
                                {
                                    createdPizza.AddToIngredientPortion(selectedIngredient, portions);
                                }

                                Console.Write("\nХотите добавить еще ингредиенты? (да/нет): ");
                                string userAnswer = Console.ReadLine();

                                if (userAnswer.ToLower() != "да")
                                {
                                    isIngredient = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ошибка: неверный номер ингредиента.");
                            }
                        }
                        createdPizza.UpdatePrice();

                        Console.Clear();
                        Console.WriteLine("--- ПИЦЦА УСПЕШНО СОЗДАНА ---\n");
                        Console.WriteLine($"Название: {createdPizza.Name}");
                        Console.WriteLine($"Стоимость: {createdPizza.Price}");
                        Console.WriteLine("\n \t Состав");
                        if(isTemplate)
                        {
                            createdPizza.CreateTemplate();
                        }

                        createdPizza.ShowAllStructure();
                    }
                }
            }
            public static void editPizza()
            {
                Console.WriteLine("--- РЕДАКТИРОВАНИЕ ПИЦЦЫ ---\n");

                Pizza.ShowAll();

                if (Pizza.AllTemplatePizza.Count > 0)
                {
                    Console.Write("\nВыберите пиццу для редактирования: ");
                    string userChoice = Console.ReadLine();

                    if (!int.TryParse(userChoice, out int choice) || choice <= 0 || choice > AllTemplatePizza.Count)
                    {
                        Console.WriteLine("\nОшибка: неверный номер пиццы.");
                        return ;
                    }

                    int choiceInt = choice - 1;
                    var selectedItem = AllTemplatePizza.ElementAt(choiceInt);
                    Guid selectedId = selectedItem.Key;
                    Pizza selectedPizza = selectedItem.Value;

                    Console.Clear();

                    Console.WriteLine($"--- РЕДАКТИРОВАНИЕ ПИЦЦЫ {selectedPizza.Name} ---");
                    Console.WriteLine("1. Изменить название");
                    Console.WriteLine("2. Изменить основу");
                    Console.WriteLine("3. Изменить состав ингредиентов");
                    Console.WriteLine("4. Назад");
                    Console.Write("\nВыберите действие: ");

                    string operationNumber = Console.ReadLine();

                    switch (operationNumber)
                    {
                        case "1":
                            selectedPizza.UpdateName();
                            break;

                        case "2":
                            selectedPizza.UpdateBase();
                            break;

                        case "3":
                            bool editIngredients = true;
                            while (editIngredients)
                            {
                                Console.Clear();

                                Console.WriteLine($"--- РЕДАКТИРОВАНИЕ СОСТАВА ПИЦЦЫ \"{selectedPizza.Name}\" ---\n");
                                Console.WriteLine("Текущий состав:");
                                selectedPizza.ShowAllStructure();
                                Console.WriteLine($"\nОбщая стоимость: {selectedPizza.Price}");

                                Console.WriteLine("\n1. Добавить ингредиент");
                                Console.WriteLine("2. Удалить ингредиент");
                                Console.WriteLine("3. Изменить количество порций");
                                Console.WriteLine("4. Завершить редактирование");
                                Console.Write("\nВыберите действие: ");

                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.Clear();

                                        selectedPizza.Addingredient();
                                        break;

                                    case "2":
                                        Console.Clear();

                                        Console.WriteLine("--- УДАЛЕНИЕ ИНГРЕДИЕНТА ---\n");

                                        if (selectedPizza.IngredientsOfPizza.Count > 0)
                                        {
                                            Console.WriteLine("Текущие ингредиенты:");
                                            for (int i = 0; i < selectedPizza.IngredientsOfPizza.Count; i++)
                                            {
                                                var selectedIngredient = selectedPizza.IngredientsOfPizza.ElementAt(i);
                                                Guid selectedIngredientId = selectedIngredient.Key;
                                                Console.WriteLine($"[ID {i + 1}] {selectedPizza.IngredientsOfPizza[selectedIngredientId].Name} - " +
                                                    $"{selectedPizza.PortionOfIngredient[selectedIngredientId]} порций");
                                            }

                                            Console.Write("\nВведите номер ингредиента для удаления: ");
                                            string removeChoice = Console.ReadLine();

                                            if (int.TryParse(removeChoice, out int removeId) &&
                                                removeId > 0 && removeId <= selectedPizza.IngredientsOfPizza.Count)
                                            {
                                                Console.Write($"\nВы уверены, что хотите удалить ингредиент " +
                                                    $"\"{selectedPizza.IngredientsOfPizza.ElementAt(removeId - 1).Value.Name}\"? (да/нет): ");
                                                string confirmRemove = Console.ReadLine();

                                                if (confirmRemove.ToLower() == "да")
                                                {
                                                    string removedName = selectedPizza.IngredientsOfPizza.ElementAt(removeId - 1).Value.Name;
                                                    selectedPizza.Removeingredient(removeId);
                                                    selectedPizza.UpdatePrice();
                                                    Console.WriteLine($"\nИнгредиент \"{removedName}\" удален из пиццы.");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\nУдаление отменено.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nОшибка: неверный номер ингредиента.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("\nВ пицце нет ингредиентов для удаления.");
                                        }
                                        break;

                                    case "3":
                                        Console.Clear();

                                        Console.WriteLine("--- ИЗМЕНЕНИЕ КОЛИЧЕСТВА ПОРЦИЙ ---\n");

                                        if (selectedPizza.IngredientsOfPizza.Count > 0)
                                        {
                                            Console.WriteLine("Текущие ингредиенты:");
                                            for (int i = 0; i < selectedPizza.IngredientsOfPizza.Count; i++)
                                            {
                                                Console.WriteLine($"[ID {i + 1}] {selectedPizza.IngredientsOfPizza.ElementAt(i).Value.Name} - " +
                                                    $"{selectedPizza.PortionOfIngredient.ElementAt(i).Value} порций");
                                            }

                                            Console.Write("\nВведите номер ингредиента для изменения количества: ");
                                            string portionChoice = Console.ReadLine();

                                            if (int.TryParse(portionChoice, out int portionId) &&
                                                portionId > 0 && portionId <= selectedPizza.IngredientsOfPizza.Count)
                                            {
                                                Console.Write($"Введите новое количество порций для " +
                                                    $"\"{selectedPizza.IngredientsOfPizza.ElementAt(portionId - 1).Value.Name}\": ");
                                                string newPortionInput = Console.ReadLine();

                                                if (int.TryParse(newPortionInput, out int newPortions) && newPortions > 0)
                                                {
                                                    selectedPizza.UpdatePortion(portionId, newPortions);
                                                    selectedPizza.UpdatePrice();
                                                    Console.WriteLine("\nКоличество порций успешно обновлено.");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\nОшибка: неверное количество порций.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nОшибка: неверный номер ингредиента.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("\nВ пицце нет ингредиентов для изменения.");
                                        }
                                        break;

                                    case "4":
                                        editIngredients = false;
                                        Console.WriteLine("\nРедактирование состава завершено.");
                                        Console.WriteLine($"Новая стоимость пиццы: {selectedPizza.Price}");
                                        break;

                                    default:
                                        Console.WriteLine("\nОшибка: введена неверная команда.");
                                        break;
                                }
                            }
                            break;

                        case "4":
                            break;

                        default:
                            Console.WriteLine("\nОшибка: введена неверная команда.");
                            break;
                    }
                }
            }

            public static Pizza GetPizzaFromGuid( Guid guid )
            {
                return AllTemplatePizza[guid];
            }

            public static int GetLengthPizza() { return AllTemplatePizza.Count; }

            public static Pizza GetPizzaFromIndex(int index) { return AllTemplatePizza.ElementAt(index).Value; }
            
            public Dictionary<Ingredient, int> GetIngredientsWithPortions()
            {
                var result = new Dictionary<Ingredient, int>();

                foreach (var item in IngredientsOfPizza)
                {
                    result.Add(item.Value, PortionOfIngredient[item.Key]);
                }

                return result;
            }

            public bool HasIngredient(Ingredient ingredient)
            {
                return IngredientsOfPizza.ContainsKey(ingredient.Id);
            }

            public bool HasBase(PizzaBase Base)
            {
                return Basic == Base;
            }

            public bool HasCrust(string crustName)
            {
                return false;
            }

            public static void FilterPizzasByIngredient()
            {
                Console.WriteLine("Список ингредиентов:");
                Ingredient.ShowAll();

                Console.Write("\nВведите номер ингредиента: ");
                string ingredientName = Console.ReadLine();
                if(!int.TryParse(ingredientName, out int ingNum) || ingNum <= 0 || ingNum > Ingredient.GetLengthIngredients()) {
                    Console.WriteLine("Введен неверный номер ингредиента!");
                    return;
                }
                Ingredient selectedIng = Ingredient.GetIngredientByIndex(--ingNum);
                Console.WriteLine($"Шаблонные пиццы, содержащие ингредиент \"{selectedIng.Name}\":");
                Console.WriteLine(new string('-', 50));

                int foundCount = 0;

                foreach (var pizza in AllTemplatePizza.Values)
                {
                    if (pizza.HasIngredient(selectedIng))
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {pizza.Name} - {pizza.Price} руб.");
                    }
                }

                if (foundCount == 0)
                {
                    Console.WriteLine($"\nПицц с ингредиентом \"{ingredientName}\" не найдено.");
                }
            }

            public static void FilterPizzasByBase()
            {
                Console.WriteLine("Список основ:");
                PizzaBase.ShowAll();
                Console.Write("\nВведите номер основы: ");
                string baseName = Console.ReadLine();
                if (!int.TryParse(baseName, out int baseNum) || baseNum <= 0 || baseNum > PizzaBase.GetLengthBasic())
                {
                    Console.WriteLine("Введен неверный номер основы!");
                    return;
                }
                PizzaBase selectedBase = PizzaBase.GetBasicByIndex(--baseNum);
                Console.WriteLine($"\nШаблонные пиццы на основе \"{selectedBase.Name}\":");

                int foundCount = 0;

                foreach (var pizza in AllTemplatePizza.Values)
                {
                    if (pizza.HasBase(selectedBase))
                    {
                        foundCount++;
                        Console.WriteLine($"{foundCount}. {pizza.Name} - {pizza.Price} руб.");
                    }
                }


                if (foundCount == 0)
                {
                    Console.WriteLine($"\nПицц на основе \"{selectedBase.Name}\" не найдено.");
                }
            }
        }
    }
}
