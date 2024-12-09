// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace LocalOnnxApp
{

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            OnnxKernelBuilder builder = new(args);


            var choice = AnsiConsole.Prompt(MenuOperations.MainMenu());
            while (choice.Id > -1)
            {
                var func = choice.Id switch
                {
                    0 => LocalLlms.AskQuestions(builder.Host.Services),
                    1 => LocalLlms.Amphibians(builder.Host.Services),
                    2 => LocalLlms.Superhero(builder.Host.Services),
                    3 => LocalLlms.TrainOnWebPage(builder.Host.Services,true),
                    4 => LocalLlms.TrainOnWebPage(builder.Host.Services,false),
                    _ => throw new NotImplementedException()
                };
                await func;
                AnsiConsole.Clear();
                choice = AnsiConsole.Prompt(MenuOperations.MainMenu());
            }

            Console.WriteLine("Program Finished");
            await builder.Host.RunAsync();
        }
        internal static async Task EmptyTask() {
            // return an empty task
            await Task.CompletedTask;
        }
    }
 
}
