
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace ToDoList
{

    class GradientPrinter
    {
        private (int, int, int) startColor;
        private (int, int, int) endColor;
        private int steps;
        private (int, int, int)[] colors;

        public GradientPrinter((int, int, int)? startColor = null, (int, int, int)? endColor = null, int steps = 10)
        {
            this.startColor = startColor ?? (128, 0, 255);
            this.endColor = endColor ?? (0, 0, 255);
            this.steps = steps;
            this.colors = GenerateGradient();
        }

        private (int, int, int)[] GenerateGradient()
        {
            (int, int, int)[] gradient = new (int, int, int)[steps];
            double rStep = (endColor.Item1 - startColor.Item1) / (double)steps;
            double gStep = (endColor.Item2 - startColor.Item2) / (double)steps;
            double bStep = (endColor.Item3 - startColor.Item3) / (double)steps;

            for (int i = 0; i < steps; i++)
            {
                int r = (int)(startColor.Item1 + rStep * i);
                int g = (int)(startColor.Item2 + gStep * i);
                int b = (int)(startColor.Item3 + bStep * i);
                gradient[i] = (r, g, b);
            }

            return gradient;
        }
        public void Print(string text)
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == lines.Length - 1)
                {
                    (int, int, int) color = colors[Math.Min(i, colors.Length - 1)];
                    Console.Write($"\u001b[38;2;{color.Item1};{color.Item2};{color.Item3}m{lines[i]}\u001b[0m");
                }
                else if (i < lines.Length)
                {
                    (int, int, int) color = colors[Math.Min(i, colors.Length - 1)];
                    Console.WriteLine($"\u001b[38;2;{color.Item1};{color.Item2};{color.Item3}m{lines[i]}\u001b[0m");
                }
            }
        }

        public void PrintCenter(string text)
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                (int, int, int) color = colors[Math.Min(i, colors.Length - 1)];
                Console.WriteLine($"\u001b[38;2;{color.Item1};{color.Item2};{color.Item3}m{CenterText(lines[i])}\u001b[0m");
            }
        }

        private string CenterText(string text)
        {
            int windowWidth = Console.WindowWidth;
            int padding = (windowWidth - text.Length) / 2;
            return new string(' ', Math.Max(0, padding)) + text;
        }
    }

    class ToDoClass
    {

        public static void Main(string[] args)
        {
            Console.Title = "Lucas ToDo | Made By Lucas";
            MainMenu();
        }

        public static void ChooseOption()
        {
            InputLine();
            int option = Convert.ToInt32(Console.ReadLine());
            try
            {
                switch (option)
                {
                    case 1:
                        AddTask();
                        break;
                    case 2:
                        RemoveTask();
                        break;
                    case 3:
                        ShowTasks();
                        break;
                    case 4:
                        CompleteTasks();
                        break;
                    case 5:
                        UnCompleteTasks();
                        break;
                    case 6:
                        System.Environment.Exit(0);
                        break;
                    default:
                        MainMenu();
                        break;
                }
            }
            catch (Exception e)
            {
                MainMenu();
                return;
            }
        }

        /* ---=== ToDo Functios ===---*/
        public static void AddTask()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.txt");
            List<string> tasks = new List<string>();

            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist
            }

            GradientPrinter printer = new GradientPrinter();
            while (true)
            {
                ClearPrint();
                printer.PrintCenter("Enter The Task You Should Do: (Exit To leave): ");
                InputLine();
                string task = Console.ReadLine();

                if (task.ToLower() == "exit")
                {
                    break;
                }

                printer.PrintCenter("Enter The Description of the task: (Skip To Skip, Exit To Leave)");
                InputLine();
                string taskDescription = Console.ReadLine();

                if (taskDescription.ToLower() == "exit")
                {
                    tasks.Add($"{task}"); // Save the entered task
                    break;
                }
                else if (taskDescription.ToLower() == "skip")
                {
                    tasks.Add($"{task}"); // Save the entered task
                }
                else
                {
                    tasks.Add($"\u001b[1m{task}: \u001b[0m {taskDescription}"); // Save the entered task
                }

            }

            try
            {
                // Sort the tasks alphabetically
                tasks.Sort();

                // Save all tasks to a file
                File.WriteAllLines(filePath, tasks);
                File.Encrypt(filePath); // Encrypt the file if necessary
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving tasks: {ex.Message}");
            }

            MainMenu();
        }


        public static void RemoveTask()
        {
            GradientPrinter printer = new GradientPrinter();
            List<string> tasks = new List<string>();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.txt");

            // Check if the file exists and read tasks
            if (fileCheck())
            {
                File.Decrypt(filePath);
                tasks.AddRange(File.ReadAllLines(filePath));
                File.Encrypt(filePath);
            }
            else
            {
                printer.PrintCenter("You don't have any tasks.");
                return; // Exit if no tasks
            }

            while (true)
            {
                if (fileCheck())
                {
                    ClearPrint(); // Clear the screen or output

                    // Prepare the display string for current tasks
                    string finalString = "Enter the number of the task you want to remove: (Exit to leave):\nCurrent Tasks:\n";
                    int count = 1;
                    foreach (var task in tasks)
                    {
                        finalString += $"{count}. {task}\n";
                        count++;
                    }

                    printer.PrintCenter(finalString);

                    InputLine(); // Display input line
                    string input = Console.ReadLine();

                    if (input.ToLower() == "exit")
                    {
                        MainMenu();
                        break; // Exit the loop if the user types "exit"
                    }

                    // Try to parse the input as a task number
                    if (int.TryParse(input, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                    {
                        string taskToRemove = tasks[taskNumber - 1]; // Get the task by index
                        tasks.RemoveAt(taskNumber - 1); // Remove the task

                        printer.PrintCenter($"Task '{taskToRemove}' has been removed.");
                        System.Threading.Thread.Sleep(1500);
                    }
                    else
                    {
                        printer.PrintCenter("Invalid input. Please enter a valid task number.");
                        System.Threading.Thread.Sleep(1500);
                    }

                    // Optionally, save the updated tasks back to the file
                    File.WriteAllLines(filePath, tasks);
                    File.Encrypt(filePath); // Encrypt the file after updating
                }
            }
        }


        public static void ShowTasks()
        {
            GradientPrinter printer = new GradientPrinter();
            List<string> tasks = new List<string>();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.txt");

            if (fileCheck())
            {
                File.Decrypt(filePath);
                tasks.AddRange(File.ReadAllLines(filePath));
                File.Encrypt(filePath);

                string TasksString = "";
                int count = 1;
                foreach (var task in tasks)
                {
                    TasksString += $"{count}. {task} \n";
                    count++;
                }

                ClearPrint();
                printer.PrintCenter($"This is All the Taskes That You Have: (Exit to Leave) \n {TasksString}");
                InputLine();
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    MainMenu();
                }
            }

        }


        public static void CompleteTasks()
        {
            GradientPrinter printer = new GradientPrinter();
            List<string> tasks = new List<string>();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.txt");

            // Check if the file exists and read tasks
            if (fileCheck())
            {
                File.Decrypt(filePath);
                tasks.AddRange(File.ReadAllLines(filePath));
                File.Encrypt(filePath);

            }
            else
            {
                fileCheck();
                return; // Exit if no tasks
            }

            // Loop to allow user to complete tasks
            while (true)
            {
                ClearPrint(); // Clear the screen or output
                if (tasks.Count == 0)
                {
                    printer.PrintCenter("No tasks available to complete.");
                    System.Threading.Thread.Sleep(1500);
                    break; // Exit if no tasks are available
                }

                // Prepare the display string for current tasks
                string FinalString = "Enter the number of the task you want to complete: (Exit to leave):\nCurrent Tasks:\n";
                int count = 1;
                foreach (var task in tasks)
                {
                    FinalString += $"{count}. {task}\n";
                    count++;
                }

                printer.PrintCenter(FinalString);

                InputLine(); // Display input line
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    MainMenu();
                    break; // Exit the loop if the user types "exit"
                }

                // Try to parse the input as a task number
                if (int.TryParse(input, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                {
                    if (tasks[taskNumber - 1].Contains("(Completed)"))
                    {
                        printer.PrintCenter("The Task Is Already Completed... \nTry Another Task.");
                        System.Threading.Thread.Sleep(1500);
                    }
                    else
                    {
                        string completedTask = tasks[taskNumber - 1]; // Get the task by index
                        tasks[taskNumber - 1] = $"{completedTask} (Completed)"; // Mark as completed

                        printer.PrintCenter($"Task '{completedTask}' has been marked as completed.");
                        System.Threading.Thread.Sleep(1500);
                    }
                }
                else
                {
                    printer.PrintCenter("Invalid input. Please enter a valid task number.");
                    System.Threading.Thread.Sleep(1500);
                }

                // Optionally, save the updated tasks back to the file
                File.WriteAllLines(filePath, tasks);
                File.Encrypt(filePath);
            }
        }

        public static void UnCompleteTasks()
        {
            GradientPrinter printer = new GradientPrinter();
            List<string> tasks = new List<string>();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.txt");

            // Check if the file exists and read tasks
            if (fileCheck())
            {
                File.Decrypt(filePath);
                tasks.AddRange(File.ReadAllLines(filePath));
                File.Encrypt(filePath);

            }
            else
            {
                fileCheck();
                return; // Exit if no tasks
            }

            // Loop to allow user to complete tasks
            while (true)
            {
                ClearPrint(); // Clear the screen or output
                if (tasks.Count == 0)
                {
                    printer.PrintCenter("No tasks available to uncomplete.");
                    System.Threading.Thread.Sleep(1500);
                    break; // Exit if no tasks are available
                }

                // Prepare the display string for current tasks
                string FinalString = "Enter the number of the task you want to uncomplete: (Exit to leave):\nCurrent Tasks:\n";
                int count = 1;
                foreach (var task in tasks)
                {
                    FinalString += $"{count}. {task}\n";
                    count++;
                }

                printer.PrintCenter(FinalString);

                InputLine(); // Display input line
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    MainMenu();
                    break; // Exit the loop if the user types "exit"
                }

                // Try to parse the input as a task number
                if (int.TryParse(input, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                {
                    if (tasks[taskNumber - 1].Contains("(Completed)"))
                    {
                        string completedTask = tasks[taskNumber - 1]; // Get the task by index
                        tasks[taskNumber - 1] = $"{completedTask.Replace("(Completed)","")}"; // Mark as uncompleted

                        printer.PrintCenter($"Task '{completedTask}' has been marked as UnCompleted.");
                        System.Threading.Thread.Sleep(1500);
                    }
                    else
                    {
                        printer.PrintCenter("The Task Is Already UnCompleted... \nTry Another Task.");
                        System.Threading.Thread.Sleep(1500);
                    }
                }
                else
                {
                    printer.PrintCenter("Invalid input. Please enter a valid task number.");
                    System.Threading.Thread.Sleep(1500);
                }

                // Optionally, save the updated tasks back to the file
                File.WriteAllLines(filePath, tasks);
                File.Encrypt(filePath);
            }
        }


        /* ---=== Methodes ===---*/
        public static void Menu()
        {
            GradientPrinter printer = new GradientPrinter();
            printer.PrintCenter(Options());
        }

        public static void ClearPrint()
        {
            Console.Clear();
            GradientPrinter printer = new GradientPrinter();
            printer.PrintCenter(Title());
        }

        public static void InputLine()
        {
            string user = System.Environment.UserName;
            string input = $"┌───({user}@Lucas-ToDo)─[~]\r\n└──$ ";

            GradientPrinter printer = new GradientPrinter();
            printer.Print(input);
        }

        public static void print(object text)
        {
            Console.WriteLine(text.ToString());
        }

        public static void MainMenu()
        {
            ClearPrint();
            Menu();
            ChooseOption();
        }

        public static bool fileCheck()
        {
            GradientPrinter printer = new GradientPrinter();
            List<string> tasks = new List<string>();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.txt");

            if (File.Exists(filePath))
            {
                File.Decrypt(filePath);
                tasks.AddRange(File.ReadAllLines(filePath));
                File.Encrypt(filePath);
                if (tasks.Count == 0) // Check if there are no tasks
                {
                    ClearPrint();
                    printer.PrintCenter("You Don't Have Any Tasks.");
                    for (int i = 3; i > 0; i--)
                    {
                        if (i != 3)
                        {
                            ClearPrint();
                            printer.PrintCenter($"Returning in {i}...");
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            printer.PrintCenter($"Returning in {i}...");
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    ClearPrint();
                    Menu();
                    ChooseOption();
                    return false; // Ensure to exit the method after redirecting
                }
                else
                {
                    return true;
                }
            }
            else
            {
                ClearPrint();
                printer.PrintCenter("You Don't Have Any Tasks.");
                for (int i = 3; i > 0; i--)
                {
                    if (i != 3)
                    {
                        ClearPrint();
                        printer.PrintCenter($"Returning in {i}...");
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        printer.PrintCenter($"Returning in {i}...");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                ClearPrint();
                Menu();
                ChooseOption();
                return false; // Ensure to exit the method after redirecting
            }
        }


        /* ---=== Fields ===--- */
        public static string Title()
        {
            string Title = """

██╗     ██╗   ██╗ ██████╗ █████╗ ███████╗    ████████╗ ██████╗ ██████╗  ██████╗ 
██║     ██║   ██║██╔════╝██╔══██╗██╔════╝    ╚══██╔══╝██╔═══██╗██╔══██╗██╔═══██╗
██║     ██║   ██║██║     ███████║███████╗       ██║   ██║   ██║██║  ██║██║   ██║
██║     ██║   ██║██║     ██╔══██║╚════██║       ██║   ██║   ██║██║  ██║██║   ██║
███████╗╚██████╔╝╚██████╗██║  ██║███████║       ██║   ╚██████╔╝██████╔╝╚██████╔╝
╚══════╝ ╚═════╝  ╚═════╝╚═╝  ╚═╝╚══════╝       ╚═╝    ╚═════╝ ╚═════╝  ╚═════╝ 
                                                                                
""";
        return Title;
        }

        public static string Options()
        {
            string options = "What I Can Offer You: \n1. Add Task \n2. Remove Task \n3. Show Taskes \n4. Complete Task \n5. UnComplete Task\n6. Exit";

            return options;
        }
    }
}
