using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ToDoTaskManager
{
    public class Task
    {
        string name;
        string description;
        int priority;
        int hour;
        int minute;
        bool NameWasCorrect = false;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        public int Hour
        {
            get
            {
                return hour;
            }
            set
            {
                if(value >= 0 && value < 24)
                    hour = value;
            }
        }
        public int Minute
        {
            get
            {
                return minute;
            }
            set
            {
                if(value >= 0 && value < 60)
                    minute = value;
            }
        }
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    name = value;
                    NameWasCorrect = true;
                }
                else
                {
                    Console.WriteLine("Ім'я введено не правильно. Спробуйте ще раз.");
                }
            }
        }
        public Task()
        {
            if(ToDoTaskManager.isLoading == false) Editting();
        }
        public void Editting()
        {
            Console.Clear();
            ToDoTaskManager.Header();

            NameWasCorrect = false;
            while (NameWasCorrect == false)
            {
                Console.WriteLine("Введіть ім'я завдання(Коротко і зрозуміло. Ім'я не може бути пустим):");
                Name = Console.ReadLine();
            }
            Console.WriteLine("Введіть опис вашого завдання (поле може бути пустим):");
            Description = Console.ReadLine();

            Console.WriteLine("Введіть час до коли потрібно виконати завдання:");
            Console.WriteLine("Години:");
            Hour = int.Parse(Console.ReadLine());
            Console.WriteLine("Хвилини:");
            Minute = int.Parse(Console.ReadLine());

            while (true)
            {
                Console.WriteLine("Оберіть важливість завдання(Цифри: 1,2,3):");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.D1) { Priority = 1; break; }
                if (key.Key == ConsoleKey.D2) { Priority = 2; break; }
                if (key.Key == ConsoleKey.D3) { Priority = 3; break; }
            }
            Console.Clear();
            ToDoTaskManager.Header();
        }
        public void GetRemind()
        {
            Console.WriteLine($"Завдання: {Name}, лишилось {Math.Abs(Hour - DateTime.Now.Hour)} годин та {Math.Abs(Minute - DateTime.Now.Minute)}");
        }
        public void GetTaskInfo(int index)
        {
            Console.WriteLine($"\n{index}. {Name}");
            Console.WriteLine($"Опис: {Description}");
            Console.WriteLine($"Пріоритет: {Priority}");
            Console.WriteLine($"Дедлайн: {Hour}:{Minute}");
        }
    }
    class TaskManager
    {
        List<Task> tasks = new List<Task>();
        public void CreateTask()
        {
            tasks.Add(new Task());
        }
        public void EditTask()
        {
            Console.Clear();
            ToDoTaskManager.Header();

            WatchAllTasks();

            Console.WriteLine("Введіть назву завдання, щоб відредагувати-видалити його:");
            string name = Console.ReadLine();

            var collection = tasks.Where(task => task.Name.Contains(name));
            Task tempTask = null;
            bool isdeletting = false;
            foreach (var task in collection)
            {
                Console.WriteLine("Оберіть режим редагування(Видалити=<d>, Змінити=<c>):");
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.D) { tempTask = task; isdeletting = true; break; }
                    if (key.Key == ConsoleKey.C) { task.Editting(); break; }
                }
                
            }
            if (isdeletting)
            {
                DeleteTask(tempTask);
            }
            
        }
        public void DeleteTask(Task _task)
        {
            foreach(Task task in tasks.ToArray())
            {
                if(_task != null)
                    if (task.Equals(_task)) tasks.Remove(task);
            }
            Console.WriteLine("Елемент успішно видалено!");
        }

        public void WatchAllTasks()
        {
            Console.Clear();
            ToDoTaskManager.Header();

            Console.WriteLine("\t\tВаші завдання:");
            var taskColl = tasks.OrderBy(x => x.Priority);
            int i = 1;
            foreach(var task in taskColl)
            {
                if (task == null) continue;

                task.GetTaskInfo(i);
                i++;
            }
        }

        public void Remind()
        {
            foreach(Task task in tasks)
            {
                task.GetRemind();
            }
        }

        public void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Task>));
            string path = "save.txt";
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            ser.Serialize(file, tasks);
            file.Close();
        }

        public void Load()
        {
            ToDoTaskManager.isLoading = true;
            XmlSerializer ser = new XmlSerializer(typeof(List<Task>));
            string path = "save.txt";
            FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            if (file.Length == 0)
            {
                file.Close();
                return;
            }
            tasks = (List<Task>)ser.Deserialize(file);
            
            file.Close();
        }
    }

    class ToDoTaskManager
    {
        public static bool isLoading = false;
        public static void Header()
        {
            Console.WriteLine("\t\tTask Manager v0.1");
            Console.WriteLine("Маніпуляції:\n C - додати завдання\n Е - редагувати завдання\n W - подивитись всі завдання\n S - зберегти зміни\n Esc - вихід");
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            TaskManager taskManager = new TaskManager();

            taskManager.Load();
            ToDoTaskManager.isLoading = false;

            ToDoTaskManager.Header();
            taskManager.Remind();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.C) taskManager.CreateTask();
                if (key.Key == ConsoleKey.E) taskManager.EditTask();
                if (key.Key == ConsoleKey.W) taskManager.WatchAllTasks();
                if (key.Key == ConsoleKey.S) taskManager.Save();
                if (key.Key == ConsoleKey.Escape) return;
            }
        }
    }
}
