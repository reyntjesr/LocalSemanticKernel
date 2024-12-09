using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using LocalOnnxApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console;
using System.Collections;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LocalOnnxApp
{

    internal partial class LocalLlms
    {
        internal static async Task AskQuestions(IServiceProvider services)
        {
            var kernelService = services.GetRequiredService<IKernelService>();
            var kernel = kernelService.Kernel;
            // Create a new chat
            StringBuilder builder = new();

            // Save some information to the memory
            var collectionName = "AskQuestionsCollection";
            // User question & answer loop
            bool nextQuestion = true;
            while (nextQuestion)
            {
                var prompt = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Ask a Question: ").AllowEmpty());
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    nextQuestion = false;
                    continue;
                }
                // Invoke the kernel with the user input
                var response = kernel.InvokePromptStreamingAsync(
                    promptTemplate: @"Question: {{$input}}
       You are an AI assistant that helps people find information: {{Recall}}"
                ,
                    arguments: new KernelArguments()
                    {
            { "input", prompt },
            { "collection", collectionName }
                    });

                AnsiConsole.Write("\nAssistant > ");

                await foreach (var message in response)
                {
                    AnsiConsole.Write(message.ToString());
                }

                AnsiConsole.WriteLine();
            }
        }
    }
}