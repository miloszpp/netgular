[<AutoOpen>]
module Netgular.TypeScriptEmitter.DomainTypes

open System.IO

type Context = 
    { writer: TextWriter; indentLevel: int }
    
let emit ctx format =
    [1..ctx.indentLevel] |> Seq.iter (fun _ -> fprintf ctx.writer "\t")
    fprintf ctx.writer format

let indent ctx = { writer = ctx.writer; indentLevel = ctx.indentLevel + 1 }

let defaultContext = { writer = System.Console.Out; indentLevel = 0 }