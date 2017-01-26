module Netgular.TypeScriptEmitter

open System.IO;

open Netgular.TypeScriptModel;

let rec getTypeRefString = function
    | TSNumberRef -> "number"
    | TSStringRef -> "string"
    | TSTypeRef(name) -> name
    | TSGenericTypeRef(name, typeParams) -> sprintf "%s<%s>" name (typeParams |> Seq.map getTypeRefString |> String.concat ", ")
    | TSArray(inner) -> sprintf "%s[]" <| getTypeRefString inner
    | TSUnion(innerTypes) -> sprintf "(%s)" (innerTypes |> Seq.map getTypeRefString |> String.concat " | ")
    | TSTypeParameterRef -> sprintf "?"
    | TSAnyRef -> "any"
    | TSNull -> "null"
    | TSUndefined -> "undefined"

let emitMember (writer:TextWriter) = function
    | TSField (typeRef, name) -> fprintf writer "\t%s: %s;\n" name (getTypeRefString typeRef)

let emitInterface (writer:TextWriter) interfaceDef =
    fprintf writer "interface %s {\n" interfaceDef.name 
    interfaceDef.members |> Seq.iter (emitMember writer)
    fprintf writer "\n}\n"
