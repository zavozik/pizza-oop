using System;
using System.ComponentModel;
using System.Security.Policy;
using static ConsoleApp2.Program;

namespace ConsoleApp2
{
    internal partial class Program
    {
        public interface IGuid
        {
            public Guid Id { get;  }
        }

        public interface INamed
        {
            public string Name { get; }
        }

        public interface IHasPrice
        {
            public decimal Price { get; }
        }

        public enum PizzaSize
        {
            Big,
            Medium,
            Small
        }

        static void Main()
        {
            mainMenu();

            Console.WriteLine("\nСпасибо, что выбрали нас!");
        }
        static void mainMenu()
        {
            bool isWorking = true;
            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("----------");
                Console.WriteLine("PIZZA TIME");
                Console.WriteLine("----------");
                Console.WriteLine("\n1. Меню ингредиентов");
                Console.WriteLine("2. Меню основ");
                Console.WriteLine("3. Меню пицц");
                Console.WriteLine("4. Меню бортов");
                Console.WriteLine("5. Меню заказов");
                Console.WriteLine("6. Выход из программы");
                Console.Write("\nВведите номер команды: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ingredientsMenu();
                        break;

                    case "2":
                        basicsMenu();
                        break;
                    case "3":
                        pizzaMenu();
                        break;
                     
                    case "4":
                        crustMenu();
                        break;

                    case "5":
                        ordersMenu();
                        break;

                    case "6":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("\nОшибка: введена неверная команда.");
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ordersMenu()
        {
            bool isWorking = true;
            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("--- МЕНЮ ЗАКАЗОВ ---");
                Console.WriteLine("\n1. Создать заказ");
                Console.WriteLine("2. Показать все заказы");
                Console.WriteLine("3. Показать отложенные заказы");
                Console.WriteLine("4. Фильтр заказов по дате");
                Console.WriteLine("5. Фильтр заказов по ингредиенту");
                Console.WriteLine("6. Фильтр заказов по основе");
                Console.WriteLine("7. Фильтр заказов по бортику");
                Console.WriteLine("8. Фильтр заказов по размеру");
                Console.WriteLine("9. Назад");
                Console.Write("\nВведите номер команды: ");

                string choiceUser = Console.ReadLine();

                switch (choiceUser)
                {
                    case "1":
                        Console.Clear();
                        Order.CreateOrder();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("--- ВСЕ ЗАКАЗЫ ---\n");
                        Order.ShowAllOrders();
                        break;

                    case "3":
                        Console.Clear();
                        Order.ShowScheduledOrders();
                        break;

                    case "4":
                        Console.Clear();

                        Order.FilterOrdersByDate();
                        break;

                    case "5":
                        Console.Clear();

                        Order.FilterOrdersByIngredient();
                        break;

                    case "6":
                        Console.Clear();

                        Order.FilterOrdersByBase();
                        break;

                    case "7":
                        Console.Clear();
                        Order.FilterOrdersByCrust();
                        break;

                    case "8":
                        Console.Clear();
                        Order.FilterOrdersBySize();
                        break;

                    case "9":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("\nОшибка: введена неверная команда.");
                        break;
                }

                if (choiceUser != "9")
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void ingredientsMenu()
        {
            bool isWorking = true;
            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("--- МЕНЮ ИНГРЕДИЕНТОВ ---");
                Console.WriteLine("\n1. Создать ингредиент");
                Console.WriteLine("2. Редактировать ингредиент");
                Console.WriteLine("3. Удалить ингредиент");
                Console.WriteLine("4. Показать все ингредиенты");
                Console.WriteLine("5. Фильтр по диапазону цен");
                Console.WriteLine("6. Фильтр по минимальной цене");
                Console.WriteLine("7. Фильтр по максимальной цене");
                Console.WriteLine("8. Назад");
                Console.Write("\nВведите номер команды: ");

                string choiceUser = Console.ReadLine();

                switch (choiceUser)
                {
                    case "1":
                        Console.Clear();

                        Ingredient.CreateItem();
                        break;

                    case "2":
                        Console.Clear();

                        Ingredient.ChangeItem();
                        break;

                    case "3":
                        Console.Clear();

                        Ingredient.DeleteItem();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("--- СПИСОК ИНГРЕДИЕНТОВ ---\n");
                        Ingredient.ShowAll();
                        break;

                    case "5":
                        Console.Clear();
                        Ingredient.FilterIngredientsByPrice();
                        break;

                    case "6":
                        Console.Clear();
                        Ingredient.FilterIngredientsByMinPrice();
                        break;

                    case "7":
                        Console.Clear();
                        Ingredient.FilterIngredientsByMaxPrice();
                        break;


                    case "8":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("\nОшибка: введена неверная команда.");
                        break;
                }
                
                if (choiceUser != "8")
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void crustMenu()
        {
            bool isWorking = true;
            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("--- МЕНЮ БОРТОВ ---");
                Console.WriteLine("\n1. Создать борт");
                Console.WriteLine("2. Редактировать борт");
                Console.WriteLine("3. Удалить борт");
                Console.WriteLine("4. Показать все борты");
                Console.WriteLine("5. Фильтр бортов по ингредиенту");
                Console.WriteLine("6. Фильтр бортов по цене");
                Console.WriteLine("7. Назад");
                Console.Write("\nВведите номер команды: ");

                string choiceUser = Console.ReadLine();

                switch (choiceUser)
                {
                    case "1":
                        Console.Clear();

                        Crust.CreateItem();
                        break;

                    case "2":
                        Console.Clear();

                        Crust.ChangeItem();
                        break;

                    case "3":
                        Console.Clear();

                        Crust.DeleteItem();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("--- СПИСОК БОРТОВ ---\n");
                        Crust.ShowAll();
                        break;

                    case "5":
                        Console.Clear();

                        Crust.FilterCrustsByIngredient();
                        break;

                    case "6":
                        Console.Clear();

                        Crust.FilterCrustsByPrice();
                        break;

                    case "7":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("\nОшибка: введена неверная команда.");
                        break;
                }

                if (choiceUser != "7")
                {
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void basicsMenu()
        {
            bool isWorking = true;
            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("--- МЕНЮ ОСНОВ ---");
                Console.WriteLine("\n1. Создать основу");
                Console.WriteLine("2. Редактировать основу");
                Console.WriteLine("3. Удалить основу");
                Console.WriteLine("4. Показать все основы");
                Console.WriteLine("5. Фильтр по диапазону цен");
                Console.WriteLine("6. Фильтр по минимальной цене");
                Console.WriteLine("7. Фильтр по максимальной цене");
                Console.WriteLine("8. Назад");
                Console.Write("\nВведите номер команды: ");

                string choiceUser = Console.ReadLine();

                switch (choiceUser)
                {
                    case "1":
                        Console.Clear();

                        PizzaBase.CreateItem();
                        break;

                    case "2":
                        Console.Clear();

                        PizzaBase.ChangeItem();
                        break;

                    case "3":
                        Console.Clear();
                        PizzaBase.DeleteItem();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("--- СПИСОК ОСНОВ ---\n");
                        PizzaBase.ShowAll();
                        break;

                    case "5":
                        Console.Clear();
                        PizzaBase.FilterBasesByPrice();
                        break;

                    case "6":
                        Console.Clear();
                        PizzaBase.FilterBasesByMinPrice();
                        break;

                    case "7":
                        Console.Clear();
                        PizzaBase.FilterBasesByMaxPrice();
                        break;

                    case "8":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("\nОшибка: введена неверная команда.");
                        break;
                }
                
                if(choiceUser != "8")
                {
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void pizzaMenu()
        {
            bool isWorking = true;
            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("--- МЕНЮ ПИЦЦ ---");
                Console.WriteLine("\n1. Создать пиццу");
                Console.WriteLine("2. Редактировать пиццу");
                Console.WriteLine("3. Удалить пиццу");
                Console.WriteLine("4. Показать все пиццы");
                Console.WriteLine("5. Фильтр пицц по ингредиенту");
                Console.WriteLine("6. Фильтр пицц по основе");
                Console.WriteLine("7. Назад");
                Console.Write("\nВведите номер команды: ");

                string choiceUser = Console.ReadLine();

                switch (choiceUser)
                {
                    case "1":
                        Console.Clear();

                        Pizza.CreatePizza(true);
                        break;

                    case "2":
                        Console.Clear();

                        Pizza.editPizza();

                        break;

                    case "3":
                        Console.Clear();

                        Pizza.DeletePizza();
                        break;

                    case "4":
                        Console.Clear();

                        Console.WriteLine("--- СПИСОК ПИЦЦ ---\n");

                        Pizza.ShowAll();
                        break;

                    case "5":
                        Console.Clear();
                        Pizza.FilterPizzasByIngredient();
                        break;

                    case "6":
                        Console.Clear();
                        Pizza.FilterPizzasByBase();
                        break;

                    case "7":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("\nОшибка: введена неверная команда.");
                        break;

                }
                
                if (choiceUser != "7")
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }
    }
}
