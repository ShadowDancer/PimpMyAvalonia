using Avalonia.Ide.CompletionEngine;
using dnlib.DotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Language.Xml;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using PimpMyAvalonia.LanguageServer;
using PimpMyAvalonia.LanguageServer.AssemblyMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class CompletionHandler : ICompletionHandler
    {
        private readonly ILanguageServer _router;
        private readonly TextDocumentBuffer _bufferManager;
        private readonly DocumentMetadataProvider _metadataProvider;
        private readonly ILogger<CompletionHandler> _logger;
        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.axaml"
            },
            new DocumentFilter()
            {
                Pattern = "**/*.xaml"
            }
        );

        private CompletionCapability _capability;

        public CompletionHandler(ILanguageServer router, TextDocumentBuffer bufferManager, DocumentMetadataProvider metadataProvider, ILogger<CompletionHandler> logger)
        {
            _router = router;
            _bufferManager = bufferManager;
            _metadataProvider = metadataProvider;
            _logger = logger;
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions
            {
                DocumentSelector = _documentSelector,
                ResolveProvider = false
            };
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToUri().LocalPath;
            var buffer = _bufferManager.GetBuffer(documentPath);

            if (buffer == null)
            {
                return new CompletionList();
            }

            var metdata = _metadataProvider.GetMetadataForDocument(documentPath);
            if(metdata == null)
            {
                return new CompletionList();
            }

            _logger.LogInformation("Metadata: " + metdata.Namespaces.Count);
            string stringBuffer = buffer.ToString();

            var lineStart = stringBuffer.IndexOf('\n', 0, request.Position.Line);
            var position = lineStart + request.Position.Character;

            _logger.LogInformation("Completing at "+ position + ": \r\n" + buffer);
            var completions = new CompletionEngine().GetCompletions(metdata, buffer.ToString(), position);

            var mappedComlpletions = completions.Completions.Select(n => new CompletionItem()
            {
                InsertText = n.InsertText,
                Kind = CompletionItemKind.Variable,
                Label = n.DisplayText,
                Detail = n.Description
            });
            return new CompletionList(mappedComlpletions);
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }
    }
}