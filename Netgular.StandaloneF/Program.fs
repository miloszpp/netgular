// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Microsoft.CodeAnalysis.MSBuild

open Netgular.Context
open Netgular.Config
open Netgular.Transpiler
open Netgular.ServiceGeneration

[<EntryPoint>]
let main argv = 

    let projectPath = "../../../Netgular.Examples.WebApi/Netgular.Examples.Webapi.csproj";
    let workspace = MSBuildWorkspace.Create();
    let project = workspace.OpenProjectAsync(projectPath).Result;
    let compilation = project.GetCompilationAsync().Result;

    let context = { compilation = compilation }
    let config = { nullableMode = Null }

    let symbol = getType context "Netgular.Examples.WebApi.Book"
    let tsModel = transpileInterface config context symbol

            //TypeScriptEmitter.emitInterface(Console.Out, tsModel);

    let services = generateAllServices config context project

    printfn "%A" services
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
