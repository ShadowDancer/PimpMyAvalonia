
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using Avalonia.Ide.CompletionEngine.DnlibMetadataProvider;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using PimpMyAvalonia.LanguageServer.ProjectModel;
using System;
using System.Threading.Tasks;
using Metadata = Avalonia.Ide.CompletionEngine.Metadata;

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

        public async Task<Metadata> CreateMetadataForProject(WorkspaceProject project)
        {
            string projectName = project.Name;
            var begin = new WorkDoneProgressBegin
            {
                Title = "Avalonia",
                Message = "Loading metadata for " + projectName,
                Percentage = 0
            };
            using IWorkDoneObserver manager = await _languageServer.WorkDoneManager.Create(begin);


            var outputDllPath = project.OutputFile;
            if(outputDllPath == null)
            {
                manager.OnNext(new WorkDoneProgressReport
                {
                    Message = "Failed to load metadata for " + projectName +  ", building project may solve the problem",
                    Percentage = 100
                });
            }

            var metadata = await CreateMetadataForAssembly(outputDllPath);            
            manager.Dispose();

            if(metadata != null)
            {
                manager.OnNext(new WorkDoneProgressReport
                {
                    Message = "Loaded metadata for " + projectName,
                    Percentage = 100,
                });
            }
            else
            {
                manager.OnNext(new WorkDoneProgressReport
                {
                    Message = "Failed to load metadata for " + projectName,
                    Percentage = 100
                });
            }

            return metadata;
        }
    }
}
