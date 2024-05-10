using Checking_duplicates.Services;
using System.Runtime.InteropServices;
using GroupDocs.Comparison;
using GroupDocs.Comparison.Options;
namespace Checking_duplicates
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("**** Поиск дубликатов ****");
                Console.WriteLine("Укажите путь к папке:");
                string? folderPath = Console.ReadLine();
                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    Console.WriteLine("Указанный путь не существует или пустой.");
                    return;
                }

                Scanfolder scanfolder = new Scanfolder();
                scanfolder.ScanFolder(folderPath);
                scanfolder.CompareImages();

                Console.WriteLine("Процесс завершен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
