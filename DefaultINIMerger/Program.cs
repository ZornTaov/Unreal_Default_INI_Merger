using System;
using System.Text;
using System.Threading.Tasks;

namespace DefaultINIMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Paste File 1 (is also output): ");
            string path1 = Console.ReadLine();
            IniFile data1 = new IniFile();
            data1.ReadFile(path1);
            Console.Write("Paste File 2: ");
            string path2 = Console.ReadLine();
            IniFile data2 = new IniFile();
            data2.ReadFile(path2);
            data1.Merge(data2);
            data1.WriteFile(path1 + ".merged");
            Console.WriteLine("Files merged!");
            Console.ReadKey();
        }
    }
}
