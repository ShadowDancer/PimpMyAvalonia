using System.Collections.Concurrent;
using System.Text;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace PimpMyAvalonia.LanguageServer
{
    /// <summary>
    /// Contains latest version of each file
    /// </summary>
    class TextDocumentBuffer
    {
        private ConcurrentDictionary<DocumentUri, Buffer> _buffers = new ConcurrentDictionary<DocumentUri, Buffer>();

        public void CreateBuffer(VersionedTextDocumentIdentifier id, string data)
        {
            _buffers[id.Uri] = new Buffer(id, data);
        }

        public void UpdateBuffer(VersionedTextDocumentIdentifier id, int position, string newText, int charactersToRemove = 0)
        {
            if(!_buffers.TryGetValue(id.Uri, out Buffer buffer))
            {
                return;
            }

            if(charactersToRemove > 0)
            {
                buffer.Data.Remove(position, charactersToRemove);
            }

            buffer.Data.Insert(position, newText);
        }

        public string GetBuffer(TextDocumentIdentifier id)
        {
            return _buffers.TryGetValue(id.Uri, out var buffer) ? buffer.Data.ToString() : "";
        }
    }

    class Buffer
    {
        public Buffer(VersionedTextDocumentIdentifier id, string initialString)
        {
            Data.Append(initialString);
            Url = id.Uri;
            Version = (int)id.Version;
        }

        public StringBuilder Data { get; } = new StringBuilder();
        public DocumentUri Url { get; }
        public int Version { get; }
    }
}
