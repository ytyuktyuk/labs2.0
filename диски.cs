using System;
using System.IO;

class DiskExplorer
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Доступные диски:");

            DriveInfo[] drives = DriveInfo.GetDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {drives[i].Name}");
            }
            Console.WriteLine("0. Выход");
            Console.Write("Выберите диск: ");

            if (!int.TryParse(Console.ReadLine(), out int diskChoice) || diskChoice == 0)
                break;

            if (diskChoice < 1 || diskChoice > drives.Length)
            {
                Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                Console.ReadKey();
                continue;
            }

            DriveInfo selectedDrive = drives[diskChoice - 1];
            Console.Clear();
            Console.WriteLine($"Информация о диске {selectedDrive.Name}:");
            Console.WriteLine($"Файловая система: {selectedDrive.DriveFormat}");
            Console.WriteLine($"Объем: {selectedDrive.TotalSize / (1024 * 1024 * 1024)} ГБ");
            Console.WriteLine($"Свободно: {selectedDrive.AvailableFreeSpace / (1024 * 1024 * 1024)} ГБ");

            string currentPath = selectedDrive.RootDirectory.FullName;

            while (true)
            {
                Console.WriteLine($"\nТекущий каталог: {currentPath}");
                try
                {
                    string[] directories = Directory.GetDirectories(currentPath);
                    string[] files = Directory.GetFiles(currentPath);

                    Console.WriteLine("\nКаталоги:");
                    for (int i = 0; i < directories.Length; i++)
                    {
                        Console.WriteLine($"  D{i + 1}. {Path.GetFileName(directories[i])}");
                    }

                    Console.WriteLine("\nФайлы:");
                    for (int i = 0; i < files.Length; i++)
                    {
                        Console.WriteLine($"  F{i + 1}. {Path.GetFileName(files[i])}");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Нет доступа к содержимому каталога.");
                }

                Console.WriteLine("\nКоманды:");
                Console.WriteLine("cd <номер> - перейти в подкаталог");
                Console.WriteLine("up - перейти в родительский каталог");
                Console.WriteLine("exit - выбрать другой диск");

                Console.Write("\nВведите команду: ");
                string input = Console.ReadLine();
                string[] parts = input.Split(' ', 2);

                if (parts[0] == "exit")
                    break;

                if (parts[0] == "up")
                {
                    DirectoryInfo parent = Directory.GetParent(currentPath);
                    if (parent != null)
                        currentPath = parent.FullName;
                    else
                        Console.WriteLine("Это корневой каталог.");
                }
                else if (parts[0] == "cd" && parts.Length == 2 && int.TryParse(parts[1], out int dirIndex))
                {
                    try
                    {
                        string[] dirs = Directory.GetDirectories(currentPath);
                        if (dirIndex >= 1 && dirIndex <= dirs.Length)
                        {
                            currentPath = dirs[dirIndex - 1];
                        }
                        else
                        {
                            Console.WriteLine("Неверный номер каталога.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Неизвестная команда.");
                }
            }
        }

        Console.WriteLine("Программа завершена.");
    }
}
