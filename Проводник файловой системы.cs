using System;
using System.IO;

class Program
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
                continue;

            string currentPath = drives[diskChoice - 1].RootDirectory.FullName;

            DriveInfo selectedDrive = drives[diskChoice - 1];
            Console.WriteLine($"\nИнформация о диске {selectedDrive.Name}:");
            Console.WriteLine($"Файловая система: {selectedDrive.DriveFormat}");
            Console.WriteLine($"Объем: {selectedDrive.TotalSize / 1024 / 1024} МБ");
            Console.WriteLine($"Свободно: {selectedDrive.AvailableFreeSpace / 1024 / 1024} МБ\n");

            bool inDisk = true;
            while (inDisk)
            {
                Console.WriteLine($"\nТекущий каталог: {currentPath}");
                string[] dirs = Directory.GetDirectories(currentPath);
                string[] files = Directory.GetFiles(currentPath);

                Console.WriteLine("Каталоги:");
                for (int i = 0; i < dirs.Length; i++)
                    Console.WriteLine($"  D{i + 1}. {Path.GetFileName(dirs[i])}");

                Console.WriteLine("Файлы:");
                for (int i = 0; i < files.Length; i++)
                    Console.WriteLine($"  F{i + 1}. {Path.GetFileName(files[i])}");

                Console.WriteLine("\nКоманды:");
                Console.WriteLine("cd <номер> - перейти в подкаталог");
                Console.WriteLine("up - вернуться в родительский каталог");
                Console.WriteLine("open <номер> - открыть текстовый файл");
                Console.WriteLine("mkdir - создать каталог");
                Console.WriteLine("mkfile - создать текстовый файл");
                Console.WriteLine("del <D/F><номер> - удалить каталог или файл");
                Console.WriteLine("exit - вернуться к выбору диска");

                Console.Write("\nВведите команду: ");
                string[] cmd = Console.ReadLine().Split(' ');

                switch (cmd[0])
                {
                    case "cd":
                        if (cmd.Length > 1 && int.TryParse(cmd[1], out int dirNum) && dirNum >= 1 && dirNum <= dirs.Length)
                            currentPath = dirs[dirNum - 1];
                        break;
                    case "up":
                        if (Directory.GetParent(currentPath) != null)
                            currentPath = Directory.GetParent(currentPath).FullName;
                        break;
                    case "open":
                        if (cmd.Length > 1 && int.TryParse(cmd[1], out int fileNum) && fileNum >= 1 && fileNum <= files.Length)
                        {
                            string fileToOpen = files[fileNum - 1];
                            if (Path.GetExtension(fileToOpen) == ".txt")
                            {
                                Console.WriteLine(File.ReadAllText(fileToOpen));
                            }
                            else
                            {
                                Console.WriteLine("Можно открывать только текстовые файлы.");
                            }
                        }
                        break;
                    case "mkdir":
                        Console.Write("Введите имя нового каталога: ");
                        string newDir = Console.ReadLine();
                        Directory.CreateDirectory(Path.Combine(currentPath, newDir));
                        break;
                    case "mkfile":
                        Console.Write("Введите имя нового файла (с .txt): ");
                        string newFile = Console.ReadLine();
                        Console.WriteLine("Введите текст для записи в файл (завершите пустой строкой):");
                        string text = "";
                        string line;
                        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                            text += line + Environment.NewLine;
                        File.WriteAllText(Path.Combine(currentPath, newFile), text);
                        break;
                    case "del":
                        if (cmd.Length > 1)
                        {
                            if (cmd[1].StartsWith("D") && int.TryParse(cmd[1].Substring(1), out int delDirNum) && delDirNum >= 1 && delDirNum <= dirs.Length)
                            {
                                Console.Write($"Удалить каталог {Path.GetFileName(dirs[delDirNum - 1])}? (y/n): ");
                                if (Console.ReadLine().ToLower() == "y")
                                    Directory.Delete(dirs[delDirNum - 1], true);
                            }
                            else if (cmd[1].StartsWith("F") && int.TryParse(cmd[1].Substring(1), out int delFileNum) && delFileNum >= 1 && delFileNum <= files.Length)
                            {
                                Console.Write($"Удалить файл {Path.GetFileName(files[delFileNum - 1])}? (y/n): ");
                                if (Console.ReadLine().ToLower() == "y")
                                    File.Delete(files[delFileNum - 1]);
                            }
                        }
                        break;
                    case "exit":
                        inDisk = false;
                        break;
                }
            }
        }
        Console.WriteLine("Работа завершена.");
    }
}

