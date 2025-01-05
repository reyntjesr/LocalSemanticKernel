# Local SemanticKernel

## Introduction
This project is inspired by Stephen Toub's [blog post][1] on building a simple console-based .NET chat application using semantic-kernel. Unlike the original examples that utilize OpenAI, this project aims to integrate Microsoft Phi 3 and the Nomic embedding model, showcasing my journey in adapting the examples with these technologies. The initial examples were successfully recreated using the Semantic Kernel Hugging Face plugin. However, I encountered challenges with the RAG example, leading to a [Stack Overflow question][2] and eventually finding a solution in [Bruno Capuano's blog post][3]. I also added Bruno's example to the program.

## Usage
To get started, ensure all NuGet packages are downloaded. This project was tested with a local server in LM Studio, running a "Microsoft Phi-3 mini 4k instruct q4 3b Q_K_M gguf" large language model alongside the "nomic embed text v1 5 Q8_0 gguf" model for local text embedding.
### ONNX example
Somebody suggested I should use a local model with ONNX [Stack Overflow question][4]. So I also created the same examples but now with the ONNX models. The example is created with the help of [this example][5] on microsoft's semantic kernel github page.
### vector database
A notable experiment involves the use of the Qdrant vector database, which is currently not operational. When addding information to the vector database I get an error from the Microsoft.SemanticKernel.Connectors.Qdrant package: "Response status code does not indicate success: 400 (Bad Request)". Contributions to resolve this issue are highly welcomed.

## Getting Started
1. **Environment Setup**: Ensure your development environment is set up with .NET and LM Studio.
2. **Install Dependencies**: Download and install all required NuGet packages.
3. **Local Server Configuration**: Set up a local server in LM Studio configured with the specified Microsoft Phi-3 and Nomic embedding models.
4. **Run the Application**: Follow the provided examples to run the application. For troubleshooting, refer to the linked resources.

## Configuring the ONNX sample

The sample can be configured by using the command line with .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to avoid the risk of leaking secrets into the repository, branches and pull requests.

### Using .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

```powershell
dotnet user-secrets set "Onnx:ModelId" ".. Onnx model id" (default: phi-3)
dotnet user-secrets set "Onnx:ModelPath" ".. your Onnx model folder path .." 
dotnet user-secrets set "Onnx:EmbeddingModelPath" ".. your Onnx model file path .."
dotnet user-secrets set "Onnx:EmbeddingVocabPath" ".. your Onnx model vocab file path .."
```

## Contributing
If you have suggestions for improving the Qdrant vector database integration or any other aspect of the project, please feel free to contribute. Your insights and pull requests are welcome.

## Resources
- [Demystifying Retrieval-Augmented Generation with .NET][1]
- [Using a Text Embedding Model Locally with Semantic Kernel - Stack Overflow][2]
- [Building Intelligent Applications with Local RAG in .NET and Phi - Bruno Capuano's Blog][3]

[1]: https://devblogs.microsoft.com/dotnet/demystifying-retrieval-augmented-generation-with-dotnet/
[2]: https://stackoverflow.com/questions/78677557/using-a-text-embedding-model-locally-with-semantic-kernel
[3]: https://techcommunity.microsoft.com/t5/educator-developer-blog/building-intelligent-applications-with-local-rag-in-net-and-phi/ba-p/4175721
[4]: https://stackoverflow.com/a/78974356/90603
[5]: https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Demos/OnnxSimpleRAG
