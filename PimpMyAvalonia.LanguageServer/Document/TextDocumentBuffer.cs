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
        private ConcurrentDictionary<string, StringBuilder> _buffers = new ConcurrentDictionary<string, StringBuilder>();

        public void CreateBuffer(string documentPath, string buffer)
        {
            _buffers[documentPath] = new StringBuilder(buffer);
        }

        public void UpdateBuffer(string documentPath, int position, string newText, int charactersToRemove = 0)
        {
            if(!_buffers.TryGetValue(documentPath, out StringBuilder buffer))
            {
                return;
            }

            if(charactersToRemove > 0)
            {
                buffer.Remove(position, charactersToRemove);
            }

            buffer.Insert(position, newText);
        }

        public string? GetBuffer(string documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer.ToString() : null;
        }
    }
}
