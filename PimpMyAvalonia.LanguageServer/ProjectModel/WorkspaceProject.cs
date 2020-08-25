using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.IO;
using System.Linq;

namespace PimpMyAvalonia.LanguageServer.ProjectModel
{

    public class WorkspaceProject
    {
        public string ProjectDirectory { get; }
        public string BinariesDirectory { get; }

        public string Name { get; }
        public string FilePath { get; }

        public string OutputFile { get; private set; }

        public WorkspaceProject(string path)
        {
            FilePath = path;
            ProjectDirectory = Path.GetDirectoryName(path);
            BinariesDirectory = Path.Combine(ProjectDirectory, "bin");
            Name = Path.GetFileNameWithoutExtension(path);

            OutputFile = Directory.GetFiles(BinariesDirectory, Name + ".dll", SearchOption.AllDirectories).FirstOrDefault();


        }

        internal void UpdateDll(string path)
        {
            OutputFile = path;
        }
    }
}
