using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using PimpMyAvalonia.LanguageServer.AssemblyMetadata;
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
                    .ConfigureLogging(logging =>
                    {
                        logging.AddFile("Logs/myapp-{Date}.txt");
                    })
                    .WithServices(ConfigureServices)
                    

                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<CompletionHandler>()
                );

            await server.WaitForExit;
        }



        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TextDocumentBuffer>();
            services.AddSingleton<AvaloniaMetadataRepository>();
            services.AddSingleton<AvaloniaMetadataLoader>();
            services.AddSingleton<TextDocumentToProjectMapper>();
            services.AddSingleton<DocumentMetadataProvider>();
        }
    }
}
