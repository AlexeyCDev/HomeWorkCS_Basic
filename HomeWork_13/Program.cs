using System;
using System.Collections.Generic;
using System.Globalization;

namespace TelegramBotMenu
{
    
    public enum ToDoItemState
    {
        Active,
        Completed
    }

    
    public class ToDoUser
    {
        public Guid UserId { get; }
        public string TelegramUserName { get; }
        public DateTime RegisteredAt { get; }

        public ToDoUser(string telegramUserName)
        {
            UserId = Guid.NewGuid();
            TelegramUserName = telegramUserName;
            RegisteredAt = DateTime.UtcNow;
        }
    }

    
    public class ToDoItem
    {
        public Guid Id { get; }
        public ToDoUser User { get; }
        public string Name { get; }
        public DateTime CreatedAt { get; }
        public ToDoItemState State { get; private set; }
        public DateTime? StateChangedAt { get; private set; }

        public ToDoItem(ToDoUser user, string name)
        {
            Id = Guid.NewGuid();
            User = user;
            Name = name;
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
            StateChangedAt = null;
        }

        public void MarkAsCompleted()
        {
            State = ToDoItemState.Completed;
            StateChangedAt = DateTime.UtcNow;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ToDoUser currentUser = null;
            string appVersion = "1.0";
            string creationDate = "19.08.2025";
            List<ToDoItem> tasks = new List<ToDoItem>();

            Console.WriteLine("Добро пожаловать! Доступные команды:");
            Console.WriteLine("/start - начать работу");
            Console.WriteLine("/help - помощь");
            Console.WriteLine("/info - информация о боте");
            Console.WriteLine("/exit - выход\n");

            while (true)
            {
                Console.Write(currentUser != null ? $"{currentUser.TelegramUserName}> " : "> ");
                string input = Console.ReadLine().Trim();
                string[] inputParts = input.Split(' ', 2);
                string command = inputParts[0].ToLower();

                switch (command)
                {
                    case "/start":
                        if (currentUser != null)
                        {
                            Console.WriteLine($"Вы уже зарегистрированы как {currentUser.TelegramUserName}.");
                        }
                        else
                        {
                            Console.Write("Введите ваше имя: ");
                            string userName = Console.ReadLine().Trim();
                            if (!string.IsNullOrEmpty(userName))
                            {
                                currentUser = new ToDoUser(userName);
                                Console.WriteLine($"Привет, {currentUser.TelegramUserName}! Теперь вам доступна команда /echo.");
                                Console.WriteLine($"Ваш ID: {currentUser.UserId}");
                                Console.WriteLine($"Дата регистрации: {currentUser.RegisteredAt.ToString("dd.MM.yyyy HH:mm:ss")}");
                            }
                            else
                            {
                                Console.WriteLine("Имя не может быть пустым.");
                            }
                        }
                        break;

                    case "/help":
                        Console.WriteLine("\nДоступные команды:");
                        Console.WriteLine("/start - регистрация в боте");
                        Console.WriteLine("/help - показать эту справку");
                        Console.WriteLine("/info - информация о боте");
                        Console.WriteLine("/echo [текст] - эхо-сообщение");
                        Console.WriteLine("/addtask [описание] - добавить новую задачу");
                        Console.WriteLine("/showtasks - показать активные задачи");
                        Console.WriteLine("/showalltasks - показать все задачи (активные и завершенные)");
                        Console.WriteLine("/completetask [ID] - завершить задачу по ID");
                        Console.WriteLine("/removetask - удалить задачу по номеру в списке");
                        Console.WriteLine("/exit - выход из программы\n");
                        break;

                    case "/info":
                        Console.WriteLine("\n=== Информация о боте ===");
                        Console.WriteLine($"Версия: {appVersion}");
                        Console.WriteLine($"Дата создания: {creationDate}");
                        Console.WriteLine("Бот для управления учебными курсами\n");
                        break;

                    case "/echo":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Сначала выполните команду /start");
                        }
                        else if (inputParts.Length < 2 || string.IsNullOrWhiteSpace(inputParts[1]))
                        {
                            Console.WriteLine("Введите текст после команды: /echo [ваш текст]");
                        }
                        else
                        {
                            Console.WriteLine($"{currentUser.TelegramUserName}, вы сказали: {inputParts[1]}");
                        }
                        break;

                    case "/addtask":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Сначала выполните команду /start");
                            break;
                        }

                        if (inputParts.Length < 2 || string.IsNullOrWhiteSpace(inputParts[1]))
                        {
                            Console.WriteLine("Введите описание задачи после команды: /addtask [описание]");
                        }
                        else
                        {
                            ToDoItem newTask = new ToDoItem(currentUser, inputParts[1]);
                            tasks.Add(newTask);
                            Console.WriteLine($"Задача добавлена: {inputParts[1]}");
                            Console.WriteLine($"ID задачи: {newTask.Id}");
                        }
                        break;

                    case "/showtasks":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Сначала выполните команду /start");
                            break;
                        }

                        var activeTasks = tasks.FindAll(t => t.State == ToDoItemState.Active);
                        if (activeTasks.Count == 0)
                        {
                            Console.WriteLine("Список активных задач пуст.");
                        }
                        else
                        {
                            Console.WriteLine("\nАктивные задачи:");
                            for (int i = 0; i < activeTasks.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {activeTasks[i].Name} - {activeTasks[i].CreatedAt.ToString("dd.MM.yyyy HH:mm:ss")} - {activeTasks[i].Id}");
                            }
                            Console.WriteLine();
                        }
                        break;

                    case "/showalltasks":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Сначала выполните команду /start");
                            break;
                        }

                        if (tasks.Count == 0)
                        {
                            Console.WriteLine("Список задач пуст.");
                        }
                        else
                        {
                            Console.WriteLine("\nВсе задачи:");
                            for (int i = 0; i < tasks.Count; i++)
                            {
                                string state = tasks[i].State == ToDoItemState.Active ? "Active" : "Completed";
                                Console.WriteLine($"{i + 1}. ({state}) {tasks[i].Name} - {tasks[i].CreatedAt.ToString("dd.MM.yyyy HH:mm:ss")} - {tasks[i].Id}");
                            }
                            Console.WriteLine();
                        }
                        break;

                    case "/completetask":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Сначала выполните команду /start");
                            break;
                        }

                        if (inputParts.Length < 2 || string.IsNullOrWhiteSpace(inputParts[1]))
                        {
                            Console.WriteLine("Введите ID задачи после команды: /completetask [ID]");
                            break;
                        }

                        if (!Guid.TryParse(inputParts[1], out Guid taskId))
                        {
                            Console.WriteLine("Неверный формат ID. ID должен быть в формате GUID.");
                            break;
                        }

                        ToDoItem taskToComplete = tasks.Find(t => t.Id == taskId);
                        if (taskToComplete == null)
                        {
                            Console.WriteLine("Задача с указанным ID не найдена.");
                        }
                        else
                        {
                            taskToComplete.MarkAsCompleted();
                            Console.WriteLine($"Задача завершена: {taskToComplete.Name}");
                            Console.WriteLine($"Дата изменения состояния: {taskToComplete.StateChangedAt?.ToString("dd.MM.yyyy HH:mm:ss")}");
                        }
                        break;

                    case "/removetask":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Сначала выполните команду /start");
                            break;
                        }

                        if (tasks.Count == 0)
                        {
                            Console.WriteLine("Список задач пуст. Нечего удалять.");
                            break;
                        }

                        // Показываем список всех задач
                        Console.WriteLine("\nВсе задачи:");
                        for (int i = 0; i < tasks.Count; i++)
                        {
                            string state = tasks[i].State == ToDoItemState.Active ? "Active" : "Completed";
                            Console.WriteLine($"{i + 1}. ({state}) {tasks[i].Name}");
                        }

                        // Запрашиваем номер задачи для удаления
                        Console.Write("Введите номер задачи для удаления: ");
                        string inputNumber = Console.ReadLine().Trim();

                        if (int.TryParse(inputNumber, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                        {
                            ToDoItem removedTask = tasks[taskNumber - 1];
                            tasks.RemoveAt(taskNumber - 1);
                            Console.WriteLine($"Задача удалена: {removedTask.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Неверный номер задачи. Пожалуйста, введите корректный номер.");
                        }
                        break;

                    case "/exit":
                        Console.WriteLine($"\nДо свидания{(currentUser != null ? $", {currentUser.TelegramUserName}" : "")}! Работа бота завершена.");
                        return;

                    default:
                        Console.WriteLine($"Неизвестная команда: {command}. Введите /help для помощи.");
                        break;
                }
            }
        }
    }
}