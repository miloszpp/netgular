module Netgular.Transpiler

open System;
open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;

open Netgular.TypeScriptModel;
open Netgular.Utils;
open Netgular.Config;
open Netgular.Resolvers;

type Context = {
    compilation: Compilation
    model: SemanticModel
    }

let parseSource source =
    let tree = CSharpSyntaxTree.ParseText (source:string)
    let root = tree.GetRoot() :?> CompilationUnitSyntax
    let compilation = CSharpCompilation.Create("DummyAssembly").AddReferences(MetadataReference.CreateFromFile(typedefof<obj>.Assembly.Location)).AddSyntaxTrees(tree)
    let model = compilation.GetSemanticModel(tree, true)
    { compilation = compilation; model = model }

let getType context name =
    context.compilation.GetTypeByMetadataName name

let private getProperties (classSymbol:INamedTypeSymbol) =
    classSymbol.GetMembers()
        |> Seq.where (fun m -> m.Kind = SymbolKind.Property) 
        |> Seq.cast<IPropertySymbol>

let transpileInterface config context (classSymbol: INamedTypeSymbol) =
    let getMember (property:IPropertySymbol) =
        let name = property.Name
        let tsType = resolveTypeRef config property.Type
        TSField(tsType, name)
    let fields = getProperties classSymbol
                    |> Seq.map getMember
                    |> Seq.toList
    { name = classSymbol.Name; members = fields }
 