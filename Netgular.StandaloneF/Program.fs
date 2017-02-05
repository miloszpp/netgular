open Microsoft.CodeAnalysis.MSBuild

open System

open Netgular.CodeGenerator.DomainTypes
open Netgular.CodeGenerator.TypeTranspiler
open Netgular.CodeGenerator.ServiceGenerator
open Netgular.TypeScriptEmitter.DomainTypes
open Netgular.TypeScriptEmitter.Emitter

[<EntryPoint>]
let main argv = 

    let projectPath = "../../../Netgular.Examples.WebApi/Netgular.Examples.Webapi.csproj";
    let workspace = MSBuildWorkspace.Create();
    let project = workspace.OpenProjectAsync(projectPath).Result;
    let compilation = project.GetCompilationAsync().Result;
    
    let config = { nullableMode = Null }
    let context = { compilation = compilation; config = config }

    let symbol = getType context "Netgular.Examples.WebApi.Book"
    let tsModel = transpileInterface context symbol
            
    let services = generateAllServices context project

    services |> Seq.iter (emitClass defaultContext)

    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
