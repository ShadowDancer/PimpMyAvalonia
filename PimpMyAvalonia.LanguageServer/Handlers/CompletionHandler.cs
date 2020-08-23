using Avalonia.Ide.CompletionEngine;
using dnlib.DotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Language.Xml;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
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
            var position = Utils.PositionToOffset(request.Position, buffer.AsSpan());


            _router.Window.LogInfo("Compltion trigger  " + request.Context.TriggerCharacter + " " + request.Position.Line + ", " + request.Position.Character);
            _router.Window.LogInfo("Completing at "+ position + ": \r\n" + buffer.Insert(position, "$"));
            var completionResult = new CompletionEngine().GetCompletions(metdata, buffer.ToString(), position);

            if(completionResult == null)
            {
                return new CompletionList();
            }

            foreach(var completion in completionResult.Completions)
            {
                _router.Window.LogInfo($"Found completions: {completion.DisplayText} ({completion.InsertText})");
            }


            List<CompletionItem> mappedComlpletions = new List<CompletionItem>();

            for (int i = 0; i < completionResult.Completions.Count; i++)
            {
                var completion = MapCompletion(completionResult.Completions[i], request.Position, i.ToString().PadLeft(10,'0'));
                mappedComlpletions.Add(completion);
            }

            return new CompletionList(mappedComlpletions);
        }

        public CompletionItem MapCompletion(Completion n, Position pos, string sortText)
        {

            TextEdit edit = new TextEdit()
            {
                NewText = n.InsertText,
                Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(pos, pos)
            };
            CompletionItem item = new CompletionItem()
            {
                Kind = MapKind(n.Kind),
                Label = n.DisplayText,
                Detail = n.DisplayText,
                Documentation = n.Description,
                TextEdit = edit,
                InsertTextFormat = InsertTextFormat.PlainText,
                SortText = sortText
            };
            if (n.RecommendedCursorOffset != null)
            {
                edit.NewText = edit.NewText.Insert(n.RecommendedCursorOffset.Value, "$0");
                item.InsertTextFormat = InsertTextFormat.Snippet;
            }


            return item;
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }

        public CompletionItemKind MapKind(CompletionKind kind)
        {
            switch(kind)
            {
                case CompletionKind.Class: 
                    return CompletionItemKind.Class;
                case CompletionKind.Property: 
                    return CompletionItemKind.Property;
                case CompletionKind.AttachedProperty: 
                    return CompletionItemKind.Property;
                case CompletionKind.StaticProperty: 
                    return CompletionItemKind.Property;
                case CompletionKind.Namespace: 
                    return CompletionItemKind.Module;
                case CompletionKind.Enum: 
                    return CompletionItemKind.Enum;
                case CompletionKind.MarkupExtension: 
                    return CompletionItemKind.Function;
                case CompletionKind.Event: 
                    return CompletionItemKind.Event;
                case CompletionKind.AttachedEvent: 
                    return CompletionItemKind.Event;
                default:
                    return CompletionItemKind.Text;
            };
        }
    }

}