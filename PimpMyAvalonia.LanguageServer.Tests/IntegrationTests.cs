using PimpMyAvalonia.LanguageServer;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.IO;
using PimpMyAvalonia.LanguageServer.Tests;

namespace PimpyMyAvalonia.LanguageServer.Tests
{

    public class IntegrationTests
    {
        private string CsProjLocation;

        public IntegrationTests()
        {
            CsProjLocation = Path.Combine(TestUtils.GetApplicationRoot(), "..\\AvaloniaSample\\AvaloniaSample.csproj");   
        }

        [Fact]
        public void TestCsprojResolution()
        {
            string directory = Path.Combine(TestUtils.GetApplicationRoot(), "..\\AvaloniaSample\\Views\\MainWindow.axaml");

            var mapper = new TextDocumentToProjectMapper(Substitute.For<ILogger<TextDocumentToProjectMapper>>());

            string csprojPath = mapper.GetProjectForDocument(directory);

            Assert.Equal(CsProjLocation, csprojPath);
        }

        [Fact]
        public async Task TestMetadataResolution()
        {
            var loader = new AvaloniaMetadataLoader(Substitute.For<ILanguageServer>());
            AvaloniaMetadataRepository repository = new AvaloniaMetadataRepository(loader);
            var metadataTask = repository.GetMetadataForProject(CsProjLocation);
            Assert.NotNull(await metadataTask);
        }
    }
}
