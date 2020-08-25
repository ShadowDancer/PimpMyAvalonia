using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using PimpMyAvalonia.LanguageServer.ProjectModel;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemWatcher = OmniSharp.Extensions.LanguageServer.Protocol.Models.FileSystemWatcher;

namespace PimpMyAvalonia.LanguageServer.Handlers
{
    class FileChangedHandler : IDidChangeWatchedFilesHandler
    {
        private readonly ProjectShepard _projectShepard;
        private readonly AvaloniaMetadataShepard _metadataShepard;

        public FileChangedHandler(ProjectShepard projectShepard, AvaloniaMetadataShepard metadataShepard)
        {
            _projectShepard = projectShepard;
            _metadataShepard = metadataShepard;
        }

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions()
        {
            var csProjWatcher = new FileSystemWatcher()
            {
                GlobPattern = "**/*.csproj",
                Kind = WatchKind.Create | WatchKind.Delete
            };
            var dllWatcher= new FileSystemWatcher()
            {
                GlobPattern = "**/*.dll",
                Kind = WatchKind.Create | WatchKind.Change
            };
            return new DidChangeWatchedFilesRegistrationOptions()
            {
                Watchers = new Container<FileSystemWatcher>(csProjWatcher, dllWatcher)
            };
        }

        public Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            foreach(var change in request.Changes)
            {
                string localPath;
                try
                {
                    localPath = change.Uri.ToUri().LocalPath;
                }
                catch
                {
                    continue; // this is invalid path
                }

                if (localPath?.EndsWith(".csproj") == true)
                {
                    if(change.Type == FileChangeType.Created)
                    {
                        _projectShepard.ProjectAdded(localPath);
                    }
                    else if(change.Type == FileChangeType.Deleted)
                    {
                        _projectShepard.ProjectRemoved(localPath);
                    }
                }
                if(localPath?.EndsWith(".dll") == true)
                {
                    string name = Path.GetFileNameWithoutExtension(localPath);
                    var projects = _projectShepard.GetProjectsByName(name);
                    if(projects.Count > 0)
                    {
                        string directory = Path.GetDirectoryName(localPath);
                        foreach(var project in projects)
                        {
                            if (directory.StartsWith(project.BinariesDirectory))
                            {
                                _metadataShepard.InvalidateMetadata(project.FilePath);
                                project.UpdateDll(localPath);
                            }
                        }
                    }
                }
            }

            return Unit.Task;
        }

        public void SetCapability(DidChangeWatchedFilesCapability capability)
        {
        }
    }
}
