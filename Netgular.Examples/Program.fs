// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open System.IO

open Netgular.Transpiler
open Netgular.Config
open Netgular.TypeScriptEmitter

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.MSBuild

let sampleCode = """
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    class Page 
    {
        public string Text { get; set; }
        public int Length { get; set; }
    }

    class Book
    {
        public string Title { get; set; }
        public int? NumPages { get; set; }
        public double Price { get; set; }
        public IEnumerable<Page> Pages { get; set; }
    }
}
"""

[<EntryPoint>]
let main argv = 

    let projectPath = "../../../Netgular.Examples.WebApi/Netgular.Examples.Webapi.csproj"

    let msWorkplace = MSBuildWorkspace.Create()
    let project = msWorkplace.OpenProjectAsync(projectPath).Result

    let comp: Compilation = project.GetCompilationAsync().Result

    //let ctx = parseSource sampleCode
    let ctx = { compilation = comp; model = null }
    let config = { nullableMode = NullableMode.Undefined }

    let withCtx f = f ctx

    let writer = System.Console.Out

    let pipeline = 
        withCtx getType >> 
        withCtx (transpileInterface config) >> 
        emitInterface writer

    printfn "%A" <| pipeline "Netgular.Examples.WebApi.Book"
    0 // return an integer exit code

