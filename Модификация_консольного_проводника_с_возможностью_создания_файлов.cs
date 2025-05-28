using System;
using System.IO;

class Program
{
    static void Main()
    {
        while (true)
        {
            // 1. Просмотр доступных дисков
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

            // 2. Получение информации о диске
            DriveInfo selectedDrive = drives[diskChoice - 1];
            Console.WriteLine($"\nИнформация о диске {selectedDrive.Name}:");
            Console.WriteLine($"Файловая система: {selectedDrive.DriveFormat}");
            Console.WriteLine($"Объем: {selectedDrive.TotalSize / 1024 / 1024} МБ");
            Console.WriteLine($"Свободно: {selectedDrive.AvailableFreeSpace / 1024 / 1024} МБ\n");

            bool inDisk = true;
            while (inDisk)
            {
                try
                {
                    // 3. Просмотр содержимого каталога
                    Console.WriteLine($"\nТекущий каталог: {currentPath}");
                    string[] dirs = Directory.GetDirectories(currentPath);
                    string[] files = Directory.GetFiles(currentPath);

                    Console.WriteLine("Каталоги:");
                    for (int i = 0; i < dirs.Length; i++)
                        Console.WriteLine($"  D{i + 1}. {Path.GetFileName(dirs[i])}");

                    Console.WriteLine("Файлы:");
                    for (int i = 0; i < files.Length; i++)
                        Console.WriteLine($"  F{i + 1}. {Path.GetFileName(files[i])}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Нет доступа к содержимому каталога.");
                }

                Console.WriteLine("\nКоманды:");
                Console.WriteLine("cd <номер> - перейти в подкаталог");
                Console.WriteLine("up - вернуться в родительский каталог");
                Console.WriteLine("open <номер> - открыть текстовый файл");
                Console.WriteLine("mkdir - создать каталог");
                Console.WriteLine("mkfile - создать текстовый файл");
                Console.WriteLine("del <D/F><номер> - удалить каталог или файл");
                Console.WriteLine("exit - вернуться к выбору диска");

                Console.Write("\nВведите команду: ");
                string[] cmd = Console.ReadLine().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                if (cmd.Length == 0)
                    continue;

                switch (cmd[0].ToLower())
                {
                    case "cd":
                        if (cmd.Length > 1 && int.TryParse(cmd[1], out int dirNum))
                        {
                            string[] dirs = Directory.GetDirectories(currentPath);
                            if (dirNum >= 1 && dirNum <= dirs.Length)
                                currentPath = dirs[dirNum - 1];
                            else
                                Console.WriteLine("Неверный номер каталога.");
                        }
                        else
                        {
                            Console.WriteLine("Введите номер каталога.");
                        }
                        break;

                    case "up":
                        DirectoryInfo parent = Directory.GetParent(currentPath);
                        if (parent != null)
                            currentPath = parent.FullName;
                        else
                            Console.WriteLine("Это корневой каталог.");
                        break;

                    case "open":
                        if (cmd.Length > 1 && int.TryParse(cmd[1], out int fileNum))
                        {
                            string[] files = Directory.GetFiles(currentPath);
                            if (fileNum >= 1 && fileNum <= files.Length)
                            {
                                string fileToOpen = files[fileNum - 1];
                                if (Path.GetExtension(fileToOpen).ToLower() == ".txt")
                                {
                                    try
                                    {
                                        string content = File.ReadAllText(fileToOpen);
                                        Console.WriteLine($"\nСодержимое файла {Path.GetFileName(fileToOpen)}:\n");
                                        Console.WriteLine(content);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Можно открывать только текстовые файлы (.txt).");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неверный номер файла.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Введите номер файла.");
                        }
                        break;

                    case "mkdir":
                        Console.Write("Введите имя нового каталога: ");
                        string newDir = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newDir))
                        {
                            string dirPath = Path.Combine(currentPath, newDir);
                            try
                            {
                                if (!Directory.Exists(dirPath))
                                {
                                    Directory.CreateDirectory(dirPath);
                                    Console.WriteLine($"Каталог '{newDir}' успешно создан.");
                                }
                                else
                                {
                                    Console.WriteLine("Каталог с таким именем уже существует.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка при создании каталога: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Имя каталога не может быть пустым.");
                        }
                        break;

                    case "mkfile":
                        Console.Write("Введите имя нового файла (обязательно с расширением .txt): ");
                        string newFile = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(newFile) || Path.GetExtension(newFile).ToLower() != ".txt")
                        {
                            Console.WriteLine("Ошибка: имя файла должно иметь расширение .txt");
                            break;
                        }

                        string fullPath = Path.Combine(currentPath, newFile);

                        if (File.Exists(fullPath))
                        {
                            Console.Write($"Файл {newFile} уже существует. Перезаписать? (y/n): ");
                            if (Console.ReadLine().ToLower() != "y")
                            {
                                Console.WriteLine("Создание файла отменено.");
                                break;
                            }
                        }

                        Console.WriteLine("Введите текст для записи в файл (завершите ввод пустой строкой):");
                        string text = "";
                        string line;
                        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                        {
                            text += line + Environment.NewLine;
                        }

                        try
                        {
                            File.WriteAllText(fullPath, text);
                            Console.WriteLine($"Файл {newFile} успешно создан.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
                        }
                        break;

                    case "del":
                        if (cmd.Length > 1)
                        {
                            string target = cmd[1];
                            if (target.StartsWith("D", StringComparison.OrdinalIgnoreCase) && int.TryParse(target.Substring(1), out int delDirNum))
                            {
                                string[] dirs = Directory.GetDirectories(currentPath);
                                if (delDirNum >= 1 && delDirNum <= dirs.Length)
                                {
                                    string dirToDelete = dirs[delDirNum - 1];
                                    Console.Write($"Удалить каталог {Path.GetFileName(dirToDelete)}? (y/n): ");
                                    if (Console.ReadLine().ToLower() == "y")
                                    {
                                        try
                                        {
                                            Directory.Delete(dirToDelete, true);
                                            Console.WriteLine("Каталог удалён.");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Ошибка при удалении каталога: {ex.Message}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Неверный номер каталога.");
                                }
                            }
                            else if (target.StartsWith("F", StringComparison.OrdinalIgnoreCase) && int.TryParse(target.Substring(1), out int delFileNum))
                            {
                                string[] files = Directory.GetFiles(currentPath);
                                if (delFileNum >= 1 && delFileNum <= files.Length)
                                {
                                    string fileToDelete = files[delFileNum - 1];
                                    Console.Write($"Удалить файл {Path.GetFileName(fileToDelete)}? (y/n): ");
                                    if (Console.ReadLine().ToLower() == "y")
                                    {
                                        try
                                        {
                                            File.Delete(fileToDelete);
                                            Console.WriteLine("Файл удалён.");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Неверный номер файла.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неверный формат команды удаления.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Укажите объект для удаления (например, del D1 или del F2).");
                        }
                        break;

                    case "exit":
                        inDisk = false;
                        break;

                    default:
                        Console.WriteLine("Неизвестная команда.");
                        break;
                }
            }
        }
        Console.WriteLine("Работа завершена.");
    }
}
