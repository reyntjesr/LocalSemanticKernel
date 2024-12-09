using LocalOnnxApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Spectre.Console;
using System.Numerics.Tensors;

#pragma warning disable SKEXP0001,SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052, SKEXP0055, SKEXP0070  // Type is for evaluation purposes only and is subject to change or removal in future updates. 

namespace LocalOnnxApp
{
    internal static partial class LocalLlms
    {
        internal static async Task Amphibians(IServiceProvider services)
        {
            var kernelService = services.GetRequiredService<IKernelService>();
            var kernel = kernelService.Kernel;
            var embeddingGenerator = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
            var semanticMemory = kernelService.SemanticTextMemory;

            // add facts to the collection
            const string MemoryCollectionName = "animalFacts";
            // Download a document and create embeddings for it
            string input = "What is an amphibian?";
            string[] examples = [ "What is an amphibian?",
                              "Cos'è un anfibio?",
                              "A frog is an amphibian.",
                              "Frogs, toads, and salamanders are all examples.",
                              "Amphibians are four-limbed and ectothermic vertebrates of the class Amphibia.",
                              "Amphibians are not four-limbed and ectothermic vertebrates of the class Amphibia.",
                              "They are four-limbed and ectothermic vertebrates.",
                              "A frog is green.",
                              "A tree is green.",
                              "It's not easy bein' green.",
                              "A dog is a mammal.",
                              "A dog is a man's best friend.",
                              "You ain't never had a friend like me.",
                              "Rachel, Monica, Phoebe, Joey, Chandler, Ross"];
            for (int i = 0; i < examples.Length; i++)
                await semanticMemory.SaveInformationAsync(MemoryCollectionName, examples[i], $"paragraph{i}");
            var embed = await embeddingGenerator.GenerateEmbeddingsAsync([input]);
            ReadOnlyMemory<float> inputEmbedding = (embed)[0];
            // Generate embeddings for each chunk.
            IList<ReadOnlyMemory<float>> embeddings = await embeddingGenerator.GenerateEmbeddingsAsync(examples);
            // Print the cosine similarity between the input and each example
            float[] similarity = embeddings.Select(e => TensorPrimitives.CosineSimilarity(e.Span, inputEmbedding.Span)).ToArray();
            similarity.AsSpan().Sort(examples.AsSpan(), (f1, f2) => f2.CompareTo(f1));
            Console.WriteLine("Similarity Example");
            for (int i = 0; i < similarity.Length; i++)
                Console.WriteLine($"{similarity[i]:F6}   {examples[i]}");
            var prompt = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Press key to continue: ").AllowEmpty());
        }
    }
}