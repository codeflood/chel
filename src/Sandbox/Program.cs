using System;
using Chel;
using Chel.Commands;
using Sandbox.Commands;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = typeof(Runtime).Assembly.GetName().Version;
            Console.WriteLine($"Chel sandbox {version}");

            var runtime = new Runtime();
            runtime.RegisterCommandType(typeof(Echo));
            runtime.RegisterCommandType(typeof(Exit));

            var session = runtime.NewSession();

            Console.WriteLine("Type 'exit' to exit.");
            Console.WriteLine("Type 'help' for help.");
            Console.WriteLine();

            var exit = false;
            while(!exit)
            {
                var input = Console.ReadLine();

                session.Execute(input, result => 
                {
                    if(result is ExitResult)
                        exit = true;

                    var previousColor = Console.ForegroundColor;

                    if(!result.Success)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine();
                    Console.WriteLine(result);

                    Console.ForegroundColor = previousColor;
                });
            }
        }
    }
}
