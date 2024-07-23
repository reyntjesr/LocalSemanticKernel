using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

/*curl http://localhost:1234/v1/chat/completions \
-H "Content-Type: application/json" \
-d '{ 
  "model": "microsoft/Phi-3-mini-4k-instruct-gguf/Phi-3-mini-4k-instruct-q4.gguf",
  "messages": [
    { "role": "system", "content": "Always answer in rhymes." },
    { "role": "user", "content": "Introduce yourself." }
  ], 
  "temperature": 0.7, 
  "max_tokens": -1,
  "stream": true
}'*/
#pragma warning disable SKEXP0070, SKEXP0003, SKEXP0001, SKEXP0011, SKEXP0052, SKEXP0055, SKEXP0050  // Type is for evaluation purposes only and is subject to change or removal in future updates. 
namespace LocalSemanticKernel
{

    internal partial class LocalLlms
    {
        internal static async Task AskQuestions()
        {
            //Suppress this diagnostic to proceed.
            // Initialize the Semantic kernel
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            var kernel = kernelBuilder
                 .AddHuggingFaceChatCompletion(                        // We use Semantic Kernel OpenAI API
                    model: "nomic-ai/nomic-embed-text-v1.5-GGUF/nomic-embed-text-v1.5.Q8_0.gguf",
                    endpoint: new Uri("http://localhost:1234"))
                .Build();

            // Create a new chat
            var ai = kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory chat = new("You are an AI assistant that helps people find information.");
            StringBuilder builder = new();

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
                chat.AddUserMessage(prompt!);

                builder.Clear();

                // Get the AI response streamed back to the console
                await foreach (var message in ai.GetStreamingChatMessageContentsAsync(chat, kernel: kernel))
                {
                    Console.Write(message);
                    builder.Append(message.Content);
                }
                Console.WriteLine();
                chat.AddAssistantMessage(builder.ToString());

                Console.WriteLine();
            }
        }
    }
}