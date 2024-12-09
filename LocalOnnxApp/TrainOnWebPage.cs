//using Elastic.Transport;
using LocalOnnxApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Build5Nines.SharpVector;
using Microsoft.SemanticKernel.Connectors.Sqlite;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Text;
using Spectre.Console;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable SKEXP0001,SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0050, SKEXP0052, SKEXP0055, SKEXP0070  // Type is for evaluation purposes only and is subject to change or removal in future updates. 
namespace LocalOnnxApp
{

    internal partial class LocalLlms
    {
        internal static async Task TrainOnWebPage(IServiceProvider services,bool sqlite)
        {
            var kernelService = services.GetRequiredService<IKernelService>();
            var kernel = kernelService.Kernel;
            var embeddingGenerator = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
            var semanticMemory = kernelService.SemanticTextMemory;


            IMemoryStore memoryStore = sqlite
                ? await SqliteMemoryStore.ConnectAsync("mydata.db")
                // vector size changed for nomic-embed-text-v1.5-GGUF
                : new QdrantMemoryStore("http://localhost:6333/", 768);

            // Download a document and create embeddings for it
            ISemanticTextMemory memory = new MemoryBuilder()
                .WithLoggerFactory(kernel.LoggerFactory)
                .WithMemoryStore(memoryStore)
                .WithTextEmbeddingGeneration(embeddingGenerator)
                .Build();
            string collectionName = "net7perf";
            IList<string> collections = await memory.GetCollectionsAsync();
            if (collections.Contains(collectionName))
            {
                AnsiConsole.WriteLine("Found database");
            }
            else
            {
                using HttpClient client = new();
                string s = await client.GetStringAsync("https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview");
                List<string> paragraphs =
                    TextChunker.SplitPlainTextParagraphs(
                        TextChunker.SplitPlainTextLines(
                            WebUtility.HtmlDecode(Regex.Replace(s, @"<[^>]+>|&nbsp;", "")),
                            128),
                        1024);
                for (int i = 0; i < paragraphs.Count; i++)
                    await memory.SaveInformationAsync(collectionName, paragraphs[i], $"paragraph{i}");
            }
            // Create a new chat
            var ai = kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory chat = new("You are an AI assistant that helps people find information.");
            StringBuilder builder = new();

            // User question & answer loop
            bool nextQuestion = true;
            while (nextQuestion)
            {
                var question = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Ask a Question: ").AllowEmpty());
                if (string.IsNullOrWhiteSpace(question))
                {
                    nextQuestion = false;
                    continue;
                }
                builder.Clear();
                await foreach (var result in memory.SearchAsync(collectionName, question, limit: 3))
                    builder.AppendLine(result.Metadata.Text);

                int contextToRemove = -1;
                if (builder.Length != 0)
                {
                    builder.Insert(0, "Here's some additional information: ");
                    contextToRemove = chat.Count;
                    chat.AddUserMessage(builder.ToString());
                }

                chat.AddUserMessage(question);

                builder.Clear();
                await foreach (var message in ai.GetStreamingChatMessageContentsAsync(chat))
                {
                    AnsiConsole.Write(message.ToString());
                    builder.Append(message.Content);
                }
                AnsiConsole.WriteLine();
                chat.AddAssistantMessage(builder.ToString());

                if (contextToRemove >= 0) chat.RemoveAt(contextToRemove);
                AnsiConsole.WriteLine();
                var prompt = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Press key to continue: ").AllowEmpty());
            }
        }
    }
}