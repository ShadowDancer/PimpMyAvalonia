using Avalonia.Ide.CompletionEngine;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using Avalonia.Ide.CompletionEngine.DnlibMetadataProvider;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace PimpMyAvalonia.LanguageServer
{
    public class AvaloniaMetadataRepository
    {
        public ConcurrentDictionary<string, Task<Metadata>> ProjectMetadata { get; } = new ConcurrentDictionary<string, Task<Metadata>>();

        public Task<Metadata> GetMetadataForProject(string projectPath)
        {
            var metadataTask =  ProjectMetadata.GetOrAdd(projectPath, CreateMetadataForProject);
            return metadataTask;
        }


        private Task<Metadata> CreateMetadataForProject(string project)
        {
            if (project.EndsWith("AvaloniaSample.csproj"))
            {
                string dir = Path.GetDirectoryName(project);
                var assemblyPath = Path.Combine(dir, "bin\\Debug\\netcoreapp3.1", "AvaloniaSample.dll");
                TaskCompletionSource<Metadata> tcs = new TaskCompletionSource<Metadata>();
                Task.Run(() =>
                {
                    var metadata = CreateMetadataForAssembly(assemblyPath);
                    tcs.SetResult(metadata);
                });
                return tcs.Task;
            }
            return Task.FromResult<Metadata>(null);
        }

        private static Metadata CreateMetadataForAssembly(string assemblyPath)
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
    }
}
