using System;
using Microsoft.CodeAnalysis.MSBuild;

namespace Netgular.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectPath = "../../../Netgular.Examples.WebApi/Netgular.Examples.Webapi.csproj";
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(projectPath).Result;
            var compilation = project.GetCompilationAsync().Result;

            var context = new Context.Context(compilation);
            var config = new Config.Config(Config.NullableMode.Null);

            var symbol = Transpiler.getType(context, "Netgular.Examples.WebApi.Book");
            var tsModel = Transpiler.transpileInterface(config, context, symbol);

            //TypeScriptEmitter.emitInterface(Console.Out, tsModel);

            var services = ServiceGeneration.generateAllServices(config, context, project);

            Console.ReadKey();
        }
    }
}
