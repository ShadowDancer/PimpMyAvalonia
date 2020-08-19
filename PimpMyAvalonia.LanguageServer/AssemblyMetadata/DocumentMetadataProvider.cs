using Avalonia.Ide.CompletionEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace PimpMyAvalonia.LanguageServer.AssemblyMetadata
{
    /// <summary>
    /// Providers metadata for text documents
    /// </summary>
    public class DocumentMetadataProvider
    {
        private readonly TextDocumentToProjectMapper _documentMapper;
        private readonly AvaloniaMetadataRepository _metadataRepository;

        public DocumentMetadataProvider(TextDocumentToProjectMapper documentMapper, AvaloniaMetadataRepository metadataRepository)
        {
            _documentMapper = documentMapper;
            _metadataRepository = metadataRepository;
        }

        public Metadata GetMetadataForDocument(string documentPath)
        {
            string projectPath = _documentMapper.GetProjectForDocument(documentPath);
            if (projectPath == null)
            {
                return null;
            }
            var metadataTask = _metadataRepository.GetMetadataForProject(projectPath);
            if (!metadataTask.IsCompletedSuccessfully)
            {
                return null;
            }
            return metadataTask.Result;
        }
    }
}
