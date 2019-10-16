using System;
using System.Globalization;
using System.Threading;
using Chel;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = typeof(Runtime).Assembly.GetName().Version;
            Console.WriteLine($"Chel sandbox {version}");

            var runtime = new Runtime();
            runtime.RegisterCommandType(typeof(Nop));

            var session = runtime.NewSession();

            Console.WriteLine("Type 'exit' to exit.");
            Console.WriteLine("Type 'help' for help.");

            var input = Console.ReadLine();
            while(input != "exit")
            {
                session.Execute(input, result => 
                {
                    var previousColor = Console.ForegroundColor;

                    if(!result.Success)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(result);

                    Console.ForegroundColor = previousColor;
                });
                Console.WriteLine();
                
                input = Console.ReadLine();
            }
        }
    }
}
