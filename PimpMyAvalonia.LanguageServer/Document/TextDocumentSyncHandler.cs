using MediatR;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PimpMyAvalonia.LanguageServer
{
    class TextDocumentSyncHandler : ITextDocumentSyncHandler
    {
        private readonly ILanguageServer _router;
        private readonly TextDocumentBuffer _bufferManager;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.xaml"
            },
            new DocumentFilter()
            {
                Pattern = "**/*.axaml"
            }
        );

        private SynchronizationCapability _capability;

        public TextDocumentSyncHandler(ILanguageServer router, TextDocumentBuffer bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                SyncKind = Change
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "xml");
        }

        public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToUri().LocalPath;
            var text = request.ContentChanges.FirstOrDefault()?.Text;

            _bufferManager.UpdateBuffer(documentPath, text);

            _router.Window.LogInfo($"Updated buffer for document: {documentPath}\n{text}");

            return Unit.Task;
        }

        public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            _bufferManager.UpdateBuffer(request.TextDocument.Uri.ToUri().LocalPath, request.TextDocument.Text);
            return Unit.Task;
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
            };
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }
    }

}
