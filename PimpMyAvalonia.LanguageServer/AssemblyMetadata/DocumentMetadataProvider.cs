using Avalonia.Ide.CompletionEngine;
using PimpMyAvalonia.LanguageServer.ProjectModel;
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
        private readonly AvaloniaMetadataShepard _metadataRepository;
        private readonly ProjectShepard _projectShepard;

        public DocumentMetadataProvider(
            TextDocumentToProjectMapper documentMapper, 
            AvaloniaMetadataShepard metadataRepository,
            ProjectShepard projectShepard)
        {
            _documentMapper = documentMapper;
            _metadataRepository = metadataRepository;
            _projectShepard = projectShepard;
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
