using PimpMyAvalonia.LanguageServer;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.IO;
using PimpMyAvalonia.LanguageServer.Tests;
using PimpMyAvalonia.LanguageServer.ProjectModel;

namespace PimpyMyAvalonia.LanguageServer.Tests
{

    public class IntegrationTests
    {
        private string CsProjLocation;

        public IntegrationTests()
        {
            CsProjLocation = Path.GetFullPath(Path.Combine(TestUtils.GetApplicationRoot(), "..\\AvaloniaSample\\AvaloniaSample.csproj"));   
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
            //MSBuildLocator.RegisterMSBuildPath("C:\\Program Files\\dotnet\\sdk\\3.1.401");
            var loader = new AvaloniaMetadataLoader(Substitute.For<ILanguageServer>());
            AvaloniaMetadataShepard shepard = new AvaloniaMetadataShepard(loader, new ProjectShepard());
            var metadataTask = shepard.GetMetadataForProject(CsProjLocation);
            Assert.NotNull(await metadataTask);
        }
    }
}
