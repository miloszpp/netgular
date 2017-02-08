[<AutoOpen>]
module Netgular.CodeGenerator.DomainTypes

open System;
open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;

type NullableMode =
    | Disabled
    | Null
    | Undefined
    | NullUndefined

type Config = {
     nullableMode: NullableMode;
     apiPath: String
    }

type Context = {
    compilation: Compilation
    config: Config
    }