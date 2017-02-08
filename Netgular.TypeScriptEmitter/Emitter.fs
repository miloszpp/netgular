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

let getOperatorString = function
    | Plus -> "+"

let rec getExpressionString = function
    | TSNumberLiteral n -> n.ToString()
    | TSStringLiteral s -> sprintf "\"%s\"" s
    | TSMethodCall(target, methodName, args) -> 
        let argsString = args |> Seq.map getExpressionString |> String.concat ", "
        sprintf "(%s).%s(%s)" (getExpressionString target) methodName argsString
    | TSFieldAccess(target, fieldName) ->
        sprintf "(%s).%s" (getExpressionString target) fieldName
    | TSBinary(op, a, b) ->
        sprintf "%s %s %s" (getExpressionString a) (getOperatorString op) (getExpressionString b)
    | TSThis ->
        "this"

let emitInterfaceMember ctx = function
    | TSField (typeRef, name) -> emit ctx "\t%s: %s;\n" name (getTypeRefString typeRef)

let emitInterface ctx interfaceDef =
    emit ctx "interface %s {\n" interfaceDef.name 
    interfaceDef.members |> Seq.iter (emitInterfaceMember ctx)
    emit ctx "\n}\n"

let rec emitStatement ctx = function
    | TSReturn(expr) -> emit ctx "return %s;\n" (getExpressionString expr)
    | TSLet(name, expr) -> emit ctx "let %s = %s;\n" name (getExpressionString expr)
    | TSStatementList(statements) -> statements |> Seq.iter (emitStatement ctx)
    | TSNoOp -> ()

let emitClassMember ctx = function
    | TSClassField(fieldType, accessModifier, name, initializer) ->
        emit ctx "%s %s %s" (getTypeRefString fieldType) (getAccessModifierString accessModifier) name
        match initializer with
        | Some(expr) -> emit ctx " = %s;\n" (getExpressionString expr)
        | None -> emit ctx ";\n"
    | TSMethod(returnType, parameters, accessModifier, name, body) ->
        let getParamString = function
            | TSMethodParameter(paramTypeRef, paramName) -> sprintf "%s : %s" paramName (getTypeRefString paramTypeRef)
        let parametersString = parameters |> Seq.map getParamString |> String.concat ", "
        emit ctx "%s %s %s(%s) {\n" (getAccessModifierString accessModifier) (getTypeRefString returnType) name parametersString
        emitStatement (indent ctx) body
        emit ctx "}\n"
    | TSConstructor(parameters, body) ->
        let getParamString = function
            | TSConstructorParameter(paramTypeRef, paramName, access) -> 
                sprintf "%s %s : %s" (getAccessModifierString access) paramName (getTypeRefString paramTypeRef)
        let parametersString = parameters |> Seq.map getParamString |> String.concat ", "
        emit ctx "constructor(%s) {\n" parametersString
        emitStatement (indent ctx) body
        emit ctx "}\n"

let emitClass ctx (classDef: TSClassDef) =
    emit ctx "export class %s {\n" classDef.className
    classDef.members |> Seq.iter (ctx |> indent |> emitClassMember)
    emit ctx "}\n"