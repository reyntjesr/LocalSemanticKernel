# Local SemanticKernel

## Introduction
This project is inspired by Stephen Toub's [blog post][1] on building a simple console-based .NET chat application using semantic-kernel. Unlike the original examples that utilize OpenAI, this project aims to integrate Microsoft Phi 3 and the Nomic embedding model, showcasing my journey in adapting the examples with these technologies. The initial examples were successfully recreated using the Semantic Kernel Hugging Face plugin. However, I encountered challenges with the RAG example, leading to a [Stack Overflow question][2] and eventually finding a solution in [Bruno Capuano's blog post][3].

## Usage
To get started, ensure all NuGet packages are downloaded. This project was tested with a local server in LM Studio, running a "Microsoft Phi-3 mini 4k instruct q4 3b Q_K_M gguf" large language model alongside the "nomic embed text v1 5 Q8_0 gguf" model for local text embedding.

A notable experiment involves the use of the Qdrant vector database, which is currently not operational. Contributions to resolve this issue are highly welcomed.

## Getting Started
1. **Environment Setup**: Ensure your development environment is set up with .NET and LM Studio.
2. **Install Dependencies**: Download and install all required NuGet packages.
3. **Local Server Configuration**: Set up a local server in LM Studio configured with the specified Microsoft Phi-3 and Nomic embedding models.
4. **Run the Application**: Follow the provided examples to run the application. For troubleshooting, refer to the linked resources.

## Contributing
If you have suggestions for improving the Qdrant vector database integration or any other aspect of the project, please feel free to contribute. Your insights and pull requests are welcome.

## Resources
- [Demystifying Retrieval-Augmented Generation with .NET][1]
- [Using a Text Embedding Model Locally with Semantic Kernel - Stack Overflow][2]
- [Building Intelligent Applications with Local RAG in .NET and Phi - Bruno Capuano's Blog][3]

[1]: https://devblogs.microsoft.com/dotnet/demystifying-retrieval-augmented-generation-with-dotnet/
[2]: https://stackoverflow.com/questions/78677557/using-a-text-embedding-model-locally-with-semantic-kernel
[3]: https://techcommunity.microsoft.com/t5/educator-developer-blog/building-intelligent-applications-with-local-rag-in-net-and-phi/ba-p/4175721
