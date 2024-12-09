using LocalOnnxApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Spectre.Console;
using System.Collections;

namespace LocalOnnxApp
{
    //    Copyright (c) 2024
    //    Author      : Bruno Capuano
    //    Change Log  :
    //    - Sample console application to use a local model hosted in ollama and semantic memory for search
    //
    //    The MIT License (MIT)
    //
    //    Permission is hereby granted, free of charge, to any person obtaining a copy
    //    of this software and associated documentation files (the "Software"), to deal
    //    in the Software without restriction, including without limitation the rights
    //    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    //    copies of the Software, and to permit persons to whom the Software is
    //    furnished to do so, subject to the following conditions:
    //
    //    The above copyright notice and this permission notice shall be included in
    //    all copies or substantial portions of the Software.
    //
    //    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    //    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    //    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    //    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    //    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    //    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    //    THE SOFTWARE.
#pragma warning disable SKEXP0001,SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052, SKEXP0055, SKEXP0070  // Type is for evaluation purposes only and is subject to change or removal in future updates. 



    internal static partial class LocalLlms
    {
        internal static async Task Superhero(IServiceProvider services)
        {
            var kernelService = services.GetRequiredService<IKernelService>();
            var kernel = kernelService.Kernel;
            

            var question = "What is Bruno's favourite super hero?";
            AnsiConsole.WriteLine($"This program will answer the following question: {question}");
            AnsiConsole.WriteLine("1st approach will be to ask the question directly to the Phi-3 model.");
            AnsiConsole.WriteLine("2nd approach will be to add facts to a semantic memory and ask the question again");
            AnsiConsole.WriteLine("");

            AnsiConsole.WriteLine($"Phi-3 response (no memory).");
            var response = kernel.InvokePromptStreamingAsync(question);
            await foreach (var result in response)
            {
                AnsiConsole.Write(result.ToString());
            }

            // separator
            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("==============");
            AnsiConsole.WriteLine("");

            // get the embeddings generator service
            var embeddingGenerator = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
            var memory = kernelService.SemanticTextMemory;

            // add facts to the collection
            const string collectionName = "fanFacts";

            await memory.SaveInformationAsync(collectionName, id: "info1", text: "Gisela's favourite super hero is Batman");
            await memory.SaveInformationAsync(collectionName, id: "info2", text: "The last super hero movie watched by Gisela was Guardians of the Galaxy Vol 3");
            await memory.SaveInformationAsync(collectionName, id: "info3", text: "Bruno's favourite super hero is Invincible");
            await memory.SaveInformationAsync(collectionName, id: "info4", text: "The last super hero movie watched by Bruno was Aquaman II");
            await memory.SaveInformationAsync(collectionName, id: "info5", text: "Bruno don't like the super hero movie: Eternals");


            AnsiConsole.WriteLine($"Phi-3 response (using semantic memory).");

            response = kernel.InvokePromptStreamingAsync(
        promptTemplate: @"Question: {{$input}}
        Answer the question using the memory content: {{Recall}}",
        arguments: new KernelArguments()
        {
            { "input", question },
            { "collection", collectionName }
        });
            await foreach (var result in response)
            {
                AnsiConsole.Write(result.ToString());
            }

            AnsiConsole.WriteLine($"");
            var prompt = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Press key to continue: ").AllowEmpty());
        }
    }
}