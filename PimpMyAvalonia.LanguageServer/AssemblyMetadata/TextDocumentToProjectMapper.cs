﻿using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace PimpMyAvalonia.LanguageServer
{
    public class TextDocumentToProjectMapper
    {
        private readonly ILogger<TextDocumentToProjectMapper> _logger;

        public TextDocumentToProjectMapper(ILogger<TextDocumentToProjectMapper> logger)
        {
            _logger = logger;
        }

        private ConcurrentDictionary<string, string> DocumentToCsprojMapping { get; } = new ConcurrentDictionary<string, string>();

        public string GetProjectForDocument(string documentPath)
        {
            return Path.GetFullPath(DocumentToCsprojMapping.GetOrAdd(documentPath, FindProjectFor));
        }

        private string FindProjectFor(string documentPath)
        {
            string path;
            string newPath = Path.GetDirectoryName(documentPath);
            do
            {
                path = newPath;
                string[] projects = Directory.GetFiles(path, "*.csproj");
                if (projects.Any())
                {
                    return projects[0];
                }

                newPath = Path.GetDirectoryName(path);
            }
            while (path != newPath);

            return null;
        }
    }
}
