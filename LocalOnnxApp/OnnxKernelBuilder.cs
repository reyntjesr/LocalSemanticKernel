using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel;
using Serilog;
using Serilog.Core;
using ML = Microsoft.Extensions.Logging;
using SL = Serilog;

#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0001

namespace LocalOnnxApp
{
    public class OnnxKernelBuilder
    {
        private readonly HostApplicationBuilder _appBuilder;
        public OnnxKernelBuilder(string[] args)
        {
           

            _appBuilder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
            //var configuration = new ConfigurationBuilder()
            //       .SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.json")
            //       .Build();
            var services = _appBuilder.Services;
            services.AddSerilog(config =>
            {
                config.ReadFrom.Configuration(_appBuilder.Configuration);
            });

            services.AddSingleton<ILoggerService, LogService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddScoped<IKernelService,KernelService>(x =>
            new KernelService(
                services.BuildServiceProvider().GetService<ILoggerService>()!,
                services.BuildServiceProvider().GetService<IConfigurationService>()!)
            );

            Host = _appBuilder.Build();

            var myService = Host.Services.GetRequiredService<ILoggerService>();
            myService.LogInformation();
        }

        public IHost Host { get; }
    }
    public interface ILoggerService
    {
        void LogInformation();
        void Write(string message);
    }
    class LogService: ILoggerService
    {
        //private readonly ILogger<LogService> _logger;
        private readonly Logger _logger;

        public LogService()
        {
            _logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5)
               .CreateLogger();
        }

        public void LogInformation()
        {
            _logger.Information(" running at: {time}", DateTimeOffset.Now);
        }

        public void Write(string message) =>
            _logger.Information("Info: {Msg}", message);
    }

    public interface IKernelService
    {
        SemanticTextMemory SemanticTextMemory { get; }
        Kernel Kernel { get; }
    }
    public class KernelService : IKernelService
    {
        private readonly ILoggerService? _logger;
        private readonly IConfigurationService? _config;
        public KernelService(ILoggerService loggerService, IConfigurationService configService)
        {
            _config = configService!;
            _logger = loggerService!;
            // Load the services
            var builder = Kernel.CreateBuilder()
                .AddOnnxRuntimeGenAIChatCompletion(_config.ChatModelId, _config.ChatModelPath)
                .AddBertOnnxTextEmbeddingGeneration(_config.EmbeddingModelPath, _config.EmbeddingVocabPath); 

            // Build Kernel
            Kernel = builder.Build();

            // Get the instances of the services

            builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Trace));
            var chatService = Kernel.GetRequiredService<IChatCompletionService>();
            var embeddingService = Kernel.GetRequiredService<ITextEmbeddingGenerationService>();

            // Create a memory store and a semantic text memory
            var memoryStore = new VolatileMemoryStore();
            SemanticTextMemory = new SemanticTextMemory(memoryStore, embeddingService);

            // Loading it for Save, Recall and other methods
            Kernel.ImportPluginFromObject(new TextMemoryPlugin(SemanticTextMemory));

        }

        public SemanticTextMemory SemanticTextMemory { get; }
        public Kernel Kernel { get; }
    }


    public interface IConfigurationService
    {
        string ChatModelPath { get; }
        string ChatModelId { get; }
        string EmbeddingModelPath { get; }
        string EmbeddingVocabPath { get; }
    }
    public class  ConfigurationService : IConfigurationService
    {
        public ConfigurationService()
        {
            // Ensure you follow the preparation steps provided in the README.md
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            // Path to the folder of your downloaded ONNX PHI-3 model
            ChatModelPath = config["Onnx:ModelPath"]!;
            ChatModelId = config["Onnx:ModelId"] ?? "phi-3";
            // Path to the file of your downloaded ONNX BGE-MICRO-V2 model
            EmbeddingModelPath = config["Onnx:EmbeddingModelPath"]!;
            // Path to the vocab file your ONNX BGE-MICRO-V2 model
            EmbeddingVocabPath = config["Onnx:EmbeddingVocabPath"]!;
        }
        public string ChatModelPath { get; }
        public string ChatModelId { get; }
        public string EmbeddingModelPath { get; }
        public string EmbeddingVocabPath { get; }
    }
   
}
