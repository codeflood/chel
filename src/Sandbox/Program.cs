using System;
using Chel.Abstractions.Parsing;
using Chel.Commands;
using Chel.Commands.Conditions;
using Chel.Parsing;
using Chel.Sandbox.Commands;
using Chel.Sandbox.Results;

namespace Chel.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = typeof(Runtime).Assembly.GetName().Version;
            Console.WriteLine($"Chel sandbox {version}");

            var runtime = new Runtime();

            runtime.RegisterCommandService<IParameterParser>(new ParameterParser());

            runtime.RegisterCommandType(typeof(Echo));
            runtime.RegisterCommandType(typeof(If));
            runtime.RegisterCommandType(typeof(Equals));
            runtime.RegisterCommandType(typeof(Greater));
            runtime.RegisterCommandType(typeof(Not));
            runtime.RegisterCommandType(typeof(Chel.Sandbox.Commands.Random));
            runtime.RegisterCommandType(typeof(Exit));

            var exit = false;
            var session = runtime.NewSession(result => 
            {
                if(result is ExitResult)
                    exit = true;

                var previousColor = Console.ForegroundColor;

                if(!result.Success)
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(result);

                Console.ForegroundColor = previousColor;
            });

            Console.WriteLine("Type 'exit' to exit.");
            Console.WriteLine("Type 'help' for help.");
            
            while(!exit)
            {
                Console.WriteLine();
                Console.Write("> ");
                var input = Console.ReadLine();
                // todo: allow SHIFT+ENTER to continue input

                session.Execute(input);
            }
        }
    }
}
