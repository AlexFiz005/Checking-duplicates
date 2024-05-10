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
            Console.WriteLine("**** Поиск дубликатов ****");
            Console.WriteLine("Укажите путь к папке:");
            Scanfolder scanfolder = new Scanfolder();
            scanfolder.ScanFolder(Console.ReadLine());
            scanfolder.CompareImages();


            Console.ReadLine();
        }
    }
}
