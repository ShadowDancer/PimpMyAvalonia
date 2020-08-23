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
        private readonly AvaloniaMetadataLoader _metadataLoader;

        public AvaloniaMetadataRepository(AvaloniaMetadataLoader metadataLoader)
        {
            _metadataLoader = metadataLoader;
        }

        public ConcurrentDictionary<string, Task<Metadata>> ProjectMetadata { get; } = new ConcurrentDictionary<string, Task<Metadata>>();

        public Task<Metadata> GetMetadataForProject(string projectPath)
        {
            var metadataTask =  ProjectMetadata.GetOrAdd(projectPath, _metadataLoader.CreateMetadataForProject);
            return metadataTask;
        }
    }
}
