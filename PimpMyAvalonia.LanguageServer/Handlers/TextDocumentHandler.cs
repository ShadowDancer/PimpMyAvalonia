using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PimpMyAvalonia.LanguageServer
{
    class TextDocumentHandler : ITextDocumentSyncHandler
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

        public TextDocumentHandler(ILanguageServer router, TextDocumentBuffer bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Incremental;

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
            var text = request.ContentChanges.FirstOrDefault()?.Text;

            var buffer = _bufferManager.GetBuffer(documentPath) ?? "";
            foreach(var change in request.ContentChanges)
            {
                var position = Utils.PositionToOffset(change.Range.Start, buffer);
                var characterToRemove = 0;
                if(change.Range.Start != change.Range.End)
                {
                    characterToRemove = Utils.PositionToOffset(change.Range.End, buffer) - position;
                }

                _bufferManager.UpdateBuffer(documentPath, position, text, characterToRemove);
            }

            return Unit.Task;
        }

        public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            _bufferManager.CreateBuffer(request.TextDocument.Uri.ToUri().LocalPath, request.TextDocument.Text);
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
