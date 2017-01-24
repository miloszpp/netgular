module Netgular.Transpiler

open System;
open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;
open Microsoft.CodeAnalysis.CSharp.Symbols;

open Netgular.TypeScriptModel;

type Context = {
    compilation: CSharpCompilation
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

let transpileClass context (classSymbol:INamedTypeSymbol) =
    let transpileProperty (property:IPropertySymbol) =
        let name = property.Name
        let tsType = match property.Type.Name with
                     | "String" -> TSString
                     | "Int32" -> TSNumber
        TSField(tsType, name)

    let fields = classSymbol.GetMembers()
                    |> Seq.where (fun m -> m.Kind = SymbolKind.Property) 
                    |> Seq.cast<IPropertySymbol>
                    |> Seq.map transpileProperty
                    |> Seq.toList

    { name = classSymbol.Name; members = fields }
   