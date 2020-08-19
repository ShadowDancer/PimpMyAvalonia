using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using System.Threading.Tasks;


namespace PimpMyAvalonia.Vs
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class ViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            try
            {
                if (!(AdaptersFactory.GetWpfTextView(textViewAdapter) is IWpfTextView view)) return;
                view.Closed += OnViewClosed;
                Microsoft.VisualStudio.Text.ITextBuffer buffer = view.TextBuffer;
                if (buffer == null) return;
                string ffn = buffer.GetFFN();
                if (ffn == null) return;
                if (!(Path.GetExtension(ffn) == ".xaml"
                    || Path.GetExtension(ffn) == ".axaml"))return;
                System.Collections.Generic.List<IContentType> content_types = ContentTypeRegistryService.ContentTypes.ToList();
                IContentType new_content_type = content_types.Find(ct => ct.TypeName == "axaml");
                Type type_of_content_type = new_content_type.GetType();
                System.Reflection.Assembly assembly = type_of_content_type.Assembly;
                buffer.ChangeContentType(new_content_type, null);
            }
            catch (Exception)
            {
            }
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (!(sender is IWpfTextView view)) return;
            view.Closed -= OnViewClosed;
        }


    }

    public static class BufferExtensions
        {
                public static string GetFFN(this ITextBuffer buffer)
    {
        if (buffer == null)
        {
            return null;
        }

        Microsoft.VisualStudio.Text.Projection.IElisionBuffer projection = buffer as Microsoft.VisualStudio.Text.Projection.IElisionBuffer;
        if (projection != null)
        {
            ITextBuffer source_buffer = projection.SourceBuffer;
            return source_buffer.GetFFN();
        }
        buffer.Properties.TryGetProperty(typeof(Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer), out Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer bufferAdapter);
        if (bufferAdapter != null)
        {
            Microsoft.VisualStudio.Shell.Interop.IPersistFileFormat persistFileFormat = bufferAdapter as Microsoft.VisualStudio.Shell.Interop.IPersistFileFormat;
            string ppzsFilename = null;
            if (persistFileFormat != null)
            {
                persistFileFormat.GetCurFile(out ppzsFilename, out uint iii);
            }

            return ppzsFilename;
        }
        return null;
    }

}

}
