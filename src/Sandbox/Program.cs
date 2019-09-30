using System;
using Chel;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chel sandbox");

            var runtime = new Runtime();
            runtime.RegisterCommandType(typeof(Greeter));

            var session = runtime.NewSession();

            Console.WriteLine("Type 'exit' to exit.");

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
