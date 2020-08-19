using System.Collections.Concurrent;
using System.Text;
using Microsoft.Language.Xml;

namespace PimpMyAvalonia.LanguageServer
{
    /// <summary>
    /// Contains latest version of each file
    /// </summary>
    class TextDocumentBuffer
    {
        private ConcurrentDictionary<string, string> _buffers = new ConcurrentDictionary<string, string>();

        public void UpdateBuffer(string documentPath, string buffer)
        {
            _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
        }

        public string GetBuffer(string documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }
    }
}
