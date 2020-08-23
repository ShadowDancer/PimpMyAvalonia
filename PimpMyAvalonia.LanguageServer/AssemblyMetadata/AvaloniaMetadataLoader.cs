using Avalonia.Ide.CompletionEngine;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using Avalonia.Ide.CompletionEngine.DnlibMetadataProvider;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PimpMyAvalonia.LanguageServer
{
    public class AvaloniaMetadataLoader
    {
        private readonly ILanguageServer _languageServer;

        public AvaloniaMetadataLoader(ILanguageServer languageServer)
        {
            _languageServer = languageServer;
        }

        private async Task<Metadata> CreateMetadataForAssembly(string assemblyPath)
        {
            try
            {
                var metadataReader = new MetadataReader(new DnlibMetadataProvider());
                Metadata metadata = metadataReader.GetForTargetAssembly(assemblyPath);
                return metadata;
            }
            catch (Exception ex)
            {
                //Log.Logger.Error(ex, "Error creating XAML completion metadata");
                return null;
            }
            finally
            {
                //Log.Logger.Verbose("Finished AvaloniaDesigner.CreateCompletionMetadataAsync()");
            }
        }

        public async Task<Metadata> CreateMetadataForProject(string project)
        {

            string projectName = Path.GetFileNameWithoutExtension(project);
            var begin = new WorkDoneProgressBegin
            {
                Title = "Loading metadata for " + projectName,
                Percentage = 0
            };
            IWorkDoneObserver manager = await _languageServer.WorkDoneManager.Create(begin);

            await Task.Delay(2000);
            manager.OnNext(new WorkDoneProgressReport
            {
                Message = "Loaded metadata for " + projectName,
                Percentage = 5,
            });

            if (project.EndsWith("AvaloniaSample.csproj"))
            {
                string dir = Path.GetDirectoryName(project);
                var assemblyPath = Path.Combine(dir, "bin\\Debug\\netcoreapp3.1", "AvaloniaSample.dll");
                TaskCompletionSource<Metadata> tcs = new TaskCompletionSource<Metadata>();
                _ = Task.Run(async () =>
                  {
                      var metadata = await CreateMetadataForAssembly(assemblyPath);
                      manager.OnNext(new WorkDoneProgressReport
                      {
                          Message = "Loaded metadata for " + projectName,
                          Percentage = 100,
                      });
                      manager.Dispose();
                      tcs.SetResult(metadata);
                  });
                return await tcs.Task;
            }

            manager.OnNext(new WorkDoneProgressReport
            {
                Message = "Failed to load metadata for " + projectName,
                Percentage = 100
            });
            manager.Dispose();
            return null;
        }
    }
}
