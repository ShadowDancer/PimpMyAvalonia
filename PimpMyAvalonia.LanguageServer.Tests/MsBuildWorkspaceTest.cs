//using Microsoft.Build.Framework;
//using Microsoft.Build.Locator;
//using Microsoft.CodeAnalysis.Host;
//using Microsoft.CodeAnalysis.MSBuild;
//using Microsoft.CodeAnalysis.Options;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace PimpMyAvalonia.LanguageServer.Tests
//{
//    public class MsBuildWorkspaceTest
//    {

//        string CsProjLocation { get; } = Path.GetFullPath(Path.Combine(TestUtils.GetApplicationRoot(), "..\\AvaloniaSample\\AvaloniaSample.csproj"));
//        [Fact]
//        public async Task test()
//        {
//            MSBuildLocator.RegisterMSBuildPath("C:\\Program Files\\dotnet\\sdk\\3.1.401");
//            MSBuildWorkspace ws = MSBuildWorkspace.Create();
//            ws.LoadMetadataForReferencedProjects = true;
//            try
//            {

//                var project = await ws.OpenProjectAsync(CsProjLocation);
//                var compilation = await project.GetCompilationAsync();

//                int z = 10;
//            }
//            catch(Exception ex)
//            {

//            }
            

//            int x = 5;
//        }
//    }
//}
