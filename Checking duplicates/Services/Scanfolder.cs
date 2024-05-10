using System.Security.Cryptography;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison;

namespace Checking_duplicates.Services
{
    /// <summary>
    /// Класс для сканирования папок и сравнения изображений на дубликаты.
    /// </summary>
    internal class Scanfolder
    {
        /// <summary>
        /// Список всех файлов изображений в папке для сканирования.
        /// </summary>
        List<string> imageFiles = new List<string>();
        private string FolderPath = "";
        /// <summary>
        /// Сканирует указанную папку и добавляет все файлы изображений в список.
        /// </summary>
        /// <param name="folderPath">Путь к папке для сканирования.</param>
        public void ScanFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                int imageCount = 0;
                imageFiles.Clear(); // Очищаем список перед добавлением новых файлов, если это необходимо.

                string[] patterns = { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif" }; // Массив шаблонов расширений файлов.
                foreach (string pattern in patterns) // Добавляем файлы по каждому шаблону в список.
                {
                    imageFiles.AddRange(Directory.GetFiles(folderPath, pattern, SearchOption.TopDirectoryOnly));
                }
                imageCount = imageFiles.Count;
                Console.WriteLine("Файлов всего {0} ", imageCount);
                FolderPath = folderPath;
            }
        }

        /// <summary>
        /// Сравнивает изображения в списке на дубликаты и перемещает их в папку назначения.
        /// </summary>
        public void CompareImages()
        {
            /// <summary>
            /// Изображения группируются по хэшу в словаре imageGroups. 
            /// Если у нескольких изображений одинаковый хэш, они добавляются в один список.
            /// </summary>
            Dictionary<string, List<string>> imageGroups = new Dictionary<string, List<string>>();
            List<string> confirmedDuplicates = new List<string>();// Список, подтвержденных дубликатов.
            HashSet<string> movedFiles = new HashSet<string>();

            string destinationFolderPath = Path.Combine(FolderPath, "Duplicates");
            // Проверяет папку, если нет - создает ее.
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            // Хеширование всех изображений и группировка по хешу.
            foreach (string imagePath in imageFiles)
            {
                string hash = ComputeHash(imagePath);
                if (!imageGroups.ContainsKey(hash))
                {
                    imageGroups[hash] = new List<string>();
                }
                imageGroups[hash].Add(imagePath);
            }

            // Сравнение потенциальных дубликатов с использованием GroupDocs.Comparison.
            foreach (var group in imageGroups)
            {
                if (group.Value.Count > 1)
                {
                    for (int i = 0; i < group.Value.Count; i++)
                    {
                        for (int j = i + 1; j < group.Value.Count; j++)
                        {
                            string path1 = group.Value[i];
                            string path2 = group.Value[j];
                            try
                            {
                                using (Comparer comparer = new Comparer(path1))
                                {
                                    comparer.Add(path2);
                                    CompareOptions options = new CompareOptions();
                                    var result = comparer.Compare(options);

                                    if (result.Changes.Count == 0)
                                    {
                                        Console.WriteLine($"Изображения одинаковые: {path1} и {path2}");
                                        if (!confirmedDuplicates.Contains(path1))
                                        {
                                            confirmedDuplicates.Add(path1);
                                        }
                                        if (!confirmedDuplicates.Contains(path2))
                                        {
                                            confirmedDuplicates.Add(path2);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Изображения разные.");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при сравнении изображений: " + ex.Message);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Все подтвержденные дубликаты перемещаются в папку Duplicates 
            /// с уникальными именами, чтобы избежать конфликта имен.
            /// </summary>
            foreach (var filePath in confirmedDuplicates)
            {
                if (!movedFiles.Contains(filePath))
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
                    string destinationFilePath = Path.Combine(destinationFolderPath, uniqueFileName);
                    File.Move(filePath, destinationFilePath);
                    movedFiles.Add(filePath);
                }
            }

            Console.WriteLine("Всего дубликатов: {0}", confirmedDuplicates.Count);
        }

        /// <summary>
        /// Вычисляет хеш изображения по заданному пути.
        /// </summary>
        /// <param name="filePath">Путь к файлу изображения.</param>
        /// <returns>Строка, представляющая хеш изображения.</returns>
        private string ComputeHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
