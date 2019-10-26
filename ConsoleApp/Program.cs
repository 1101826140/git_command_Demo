using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }

        private void write()
        {
            Console.WriteLine("test2");
        }
        private void write2()
        {
            Console.WriteLine("test2");
        }
    }
}
