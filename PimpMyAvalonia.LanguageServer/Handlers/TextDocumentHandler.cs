using Avalonia.Ide.CompletionEngine;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using PimpMyAvalonia.LanguageServer.Handlers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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

        public async Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            var text = request.ContentChanges.FirstOrDefault()?.Text;

            var buffer = _bufferManager.GetBuffer(request.TextDocument);
            int offset = 0;
            int characterToRemove = 0;
            foreach (var change in request.ContentChanges)
            {
                offset = Utils.PositionToOffset(change.Range.Start, buffer);
                characterToRemove = 0;
                if (change.Range.Start != change.Range.End)
                {
                    characterToRemove = Utils.PositionToOffset(change.Range.End, buffer) - offset;
                }

                _bufferManager.UpdateBuffer(request.TextDocument, offset, text, characterToRemove);
            }
            if (request.ContentChanges.Count() == 1)
            {
                var bufferWithContentChange = _bufferManager.GetBuffer(request.TextDocument);
                await ApplyTextManipulations(request, text, buffer, bufferWithContentChange, offset, characterToRemove);
            }
            return Unit.Value;
        }

        private async Task ApplyTextManipulations(DidChangeTextDocumentParams request, string text, string buffer, string changedBuffer, int position, int deletedCharacters)
        {
            TextManipulator textManipulator = new TextManipulator(changedBuffer, position);
            var manipulations = textManipulator.ManipulateText(new TextChangeAdapter(position, text, buffer.Substring(position, deletedCharacters)));

            if(manipulations.Count == 0)
            {
                return;
            }

            var edits = manipulations.Select(n =>
            {
                var start = Utils.OffsetToPosition(n.Start, changedBuffer);
                
                switch (n.Type)
                {
                    case ManipulationType.Insert:
                        return new TextEdit()
                        {
                            NewText = n.Text,
                            Range = new Range(start, start)
                        };
                    case ManipulationType.Delete:
                        var end = Utils.OffsetToPosition(n.End, changedBuffer);
                        return new TextEdit()
                        {
                            NewText = "",
                            Range = new Range(start, end)
                        };
                    default:
                        throw new NotSupportedException();
                }
            }).ToList();
            if (edits.Count > 0)
            {
                await _router.ApplyWorkspaceEdit(new ApplyWorkspaceEditParams()
                {
                    Edit = new WorkspaceEdit()
                    {
                        DocumentChanges = new Container<WorkspaceEditDocumentChange>(new WorkspaceEditDocumentChange(new TextDocumentEdit()
                        {
                            TextDocument = request.TextDocument,
                            Edits = new TextEditContainer(edits)
                        }))
                    }
                });
            }
        }

        public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            _bufferManager.CreateBuffer(new VersionedTextDocumentIdentifier()
            {
                Uri = request.TextDocument.Uri,
                Version = request.TextDocument.Version
            }, request.TextDocument.Text);
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
