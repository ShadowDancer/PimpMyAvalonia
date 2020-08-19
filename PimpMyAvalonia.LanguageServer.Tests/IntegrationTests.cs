using PimpMyAvalonia.LanguageServer;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;

namespace PimpyMyAvalonia.LanguageServer.Tests
{

    public class IntegrationTests
    {
        private const string CsprojColiaCs = "C:\\Users\\przem\\source\\repos\\PimpMyAvalonia\\AvaloniaSample\\AvaloniaSample.csproj";

        [Fact]
        public void TestCsprojResolution()
        {
            string directory = "C:\\Users\\przem\\source\\repos\\PimpMyAvalonia\\AvaloniaSample\\Views\\MainWindow.axaml";

            var mapper = new TextDocumentToProjectMapper(Substitute.For<ILogger<TextDocumentToProjectMapper>>());

            string csprojPath = mapper.GetProjectForDocument(directory);

            Assert.Equal(CsprojColiaCs, csprojPath);
        }

        [Fact]
        public async Task TestMetadataResolution()
        {
            AvaloniaMetadataRepository repository = new AvaloniaMetadataRepository();
            var metadataTask = repository.GetMetadataForProject(CsprojColiaCs);
            Assert.NotNull(await metadataTask);
        }
    }
}
