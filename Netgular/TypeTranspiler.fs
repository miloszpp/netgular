module Netgular.CodeGenerator.TypeTranspiler

open System;
open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;

open Netgular.TypeScriptModel;
open Netgular.CodeGenerator.Common;
open Netgular.CodeGenerator.TypeRefResolver;

let getType (context: Context) name =
    context.compilation.GetTypeByMetadataName (name: String)

let private getProperties (classSymbol:INamedTypeSymbol) =
    classSymbol.GetMembers()
        |> Seq.where (fun m -> m.Kind = SymbolKind.Property) 
        |> Seq.cast<IPropertySymbol>

let transpileInterface context (classSymbol: INamedTypeSymbol) =
    let getMember (property:IPropertySymbol) =
        let name = property.Name
        let tsType = resolveTypeRef context.config property.Type
        TSField(tsType, name)
    let fields = getProperties classSymbol
                    |> Seq.map getMember
                    |> Seq.toList
    { name = classSymbol.Name; members = fields }
 