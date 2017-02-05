module Netgular.TypeScriptEmitter.Emitter

open System.IO;

open Netgular.TypeScriptModel;
open Netgular.TypeScriptEmitter.DomainTypes;

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

let getAccessModifierString = function
    | Public -> "public"
    | Private -> "private"

let getExpressionString = function
    | TSNumberLiteral n -> n.ToString()
    | TSStringLiteral s -> s

let emitInterfaceMember ctx = function
    | TSField (typeRef, name) -> emit ctx "\t%s: %s;\n" name (getTypeRefString typeRef)

let emitInterface ctx interfaceDef =
    emit ctx "interface %s {\n" interfaceDef.name 
    interfaceDef.members |> Seq.iter (emitInterfaceMember ctx)
    emit ctx "\n}\n"

let emitStatement ctx = function
    | _ -> emit ctx "\n"

let emitClassMember ctx = function
    | TSClassField(fieldType, accessModifier, name, initializer) ->
        emit ctx "%s %s %s" (getTypeRefString fieldType) (getAccessModifierString accessModifier) name
        match initializer with
        | Some(expr) -> emit ctx " = %s;\n" (getExpressionString expr)
        | None -> emit ctx ";\n"
    | TSMethod(returnType, parameters, accessModifier, name, body) ->
        let getParamString = function
            | TSMethodParameter(paramTypeRef, paramName) -> sprintf "%s %s" (getTypeRefString paramTypeRef) paramName
        let parametersString = parameters |> Seq.map getParamString |> String.concat ", "
        emit ctx "%s %s %s(%s) {\n" (getTypeRefString returnType) (getAccessModifierString accessModifier) name parametersString
        emitStatement (indent ctx) body
        emit ctx "}\n"
    | TSConstructor(parameters, body) -> ()

let emitClass ctx (classDef: TSClassDef) =
    emit ctx "export class %s {\n" classDef.className
    classDef.members |> Seq.iter (ctx |> indent |> emitClassMember)
    emit ctx "}\n"