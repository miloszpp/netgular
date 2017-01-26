﻿// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Netgular.Transpiler
open Netgular.Config

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

    let ctx = parseSource sampleCode
    let config = { nullableMode = NullableMode.Undefined }

    let withCtx f = f ctx

    //let getTypeCtx = getType ctx
    //let transpileCtx = transpileClass ctx

    let pipeline = withCtx getType >> withCtx (transpileInterface config)

    printfn "%A" <| pipeline "HelloWorld.Book"
    0 // return an integer exit code

