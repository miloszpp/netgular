module Netgular.Context

open System;
open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;

type Context = {
    compilation: Compilation
    }