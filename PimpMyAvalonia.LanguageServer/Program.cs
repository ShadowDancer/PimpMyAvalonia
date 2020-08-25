using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using PimpMyAvalonia.LanguageServer.AssemblyMetadata;
using PimpMyAvalonia.LanguageServer.Handlers;
using PimpMyAvalonia.LanguageServer.ProjectModel;
using Server;
namespace PimpMyAvalonia.LanguageServer
{
    public class Program
    {

        static async Task Main(string[] args)
        {
            var server = await OmniSharp.Extensions.LanguageServer.Server.LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithServices(ConfigureServices)
                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<CompletionHandler>()
                    .WithHandler<FileChangedHandler>()
                );

            await server.WaitForExit;
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TextDocumentBuffer>();
            services.AddSingleton<ProjectShepard>();
            services.AddSingleton<AvaloniaMetadataShepard>();
            services.AddSingleton<AvaloniaMetadataLoader>();
            services.AddSingleton<TextDocumentToProjectMapper>();
            services.AddSingleton<DocumentMetadataProvider>();
        }
    }
}
