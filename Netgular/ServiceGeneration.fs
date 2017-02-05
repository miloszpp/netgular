module Netgular.ServiceGeneration

open System;
open System.Linq;

open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.CSharp;
open Microsoft.CodeAnalysis.CSharp.Syntax;

open Netgular.Utils
open Netgular.Context
open Netgular.Transpiler
open Netgular.Resolvers
open Netgular.TypeScriptModel

let getBaseClasses (symbol: INamedTypeSymbol) =
    let rec getBaseClassesRec (classSymbol: INamedTypeSymbol) acc =
        let interfaces = Seq.toList classSymbol.Interfaces
        let updatedAcc = [classSymbol] @ interfaces @ acc
        match Option.ofObj(classSymbol.BaseType) with
        | Some(baseTypeSymbol) -> getBaseClassesRec baseTypeSymbol updatedAcc
        | None -> updatedAcc
    getBaseClassesRec symbol []

let findControllers (ctx: Context) (project: Project) =
    let compilation = ctx.compilation
    let targetType = compilation.GetTypeByMetadataName("System.Web.Http.ApiController")
    seq {
     for document in project.Documents do 
        let tree = document.GetSyntaxTreeAsync().Result
        let model = compilation.GetSemanticModel tree     
        let isController (node: ClassDeclarationSyntax) =
            model.GetDeclaredSymbol node |> getBaseClasses |> Seq.contains targetType
        yield! tree.GetRoot().DescendantNodes() 
                |> Seq.choose castAs<ClassDeclarationSyntax>
                |> Seq.where isController
                |> Seq.map (fun syntax -> syntax, model.GetDeclaredSymbol syntax)
    }

let findActions (controller: ClassDeclarationSyntax) =
    let isAction (m:MethodDeclarationSyntax) =
        let attributes = m.DescendantNodes() |> Seq.choose castAs<AttributeSyntax>
        System.Console.WriteLine(attributes)
        attributes |> Seq.exists (fun a -> a.Name.ToString().StartsWith "Http")
    controller.Members 
        |> Seq.choose castAs<MethodDeclarationSyntax>
        |> Seq.where isAction
    
let generateService config (controllerSyntax: ClassDeclarationSyntax, controllerSymbol: INamedTypeSymbol) =
    let actions = findActions controllerSyntax
    let generateMember (action: MethodDeclarationSyntax) =
        let actionSymbol = controllerSymbol.GetMembers(action.Identifier.Text) |> Seq.head :?> IMethodSymbol
        let name = actionSymbol.Name.ToString()
        let returnType = resolveTypeRef config actionSymbol.ReturnType
        let parameters = seq {
            for parameter in actionSymbol.Parameters do
                let typeRef = resolveTypeRef config parameter.Type
                yield TSMethodParameter (typeRef, parameter.Name.ToString())
        }
        TSMethod(returnType, parameters, TSAccessModifier.Public, name, TSNoOp)
    let className = controllerSymbol.Name.ToString()
    let methods = actions |> Seq.map generateMember |> Seq.toList
    { className = className; members = methods }

let generateAllServices config context project =
    seq {
        for controller in findControllers context project do
            yield generateService config controller
    }

