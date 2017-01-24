// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Netgular.Transpiler

open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;
open Microsoft.CodeAnalysis.CSharp.Symbols;



let sampleCode = """
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    class Book
    {
        public string Title { get; set; }
    }
}
"""

[<EntryPoint>]
let main argv = 

    let ctx = parseSource sampleCode

    let getTypeCtx = getType ctx
    let transpileCtx = transpileClass ctx

    let pipeline = getTypeCtx >> transpileCtx

    printfn "%A" <| pipeline "HelloWorld.Book"
    0 // return an integer exit code

