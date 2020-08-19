using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace PimpMyAvalonia.Vs
{
    public class AvaloniaXamlLanguageContentDefinition
    {
        [Export]
        [Name("axaml")]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition AxamlContentTypeDefinition;

        [Export]
        [FileExtension(".axaml")]
        [ContentType("axaml")]
        internal static FileExtensionToContentTypeDefinition AxamlFileExtensionDefinition;
    }
}
