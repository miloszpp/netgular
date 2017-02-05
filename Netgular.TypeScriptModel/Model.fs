module Netgular.TypeScriptModel

type TSTypeRef =
    | TSNumberRef
    | TSStringRef
    | TSTypeRef of string
    | TSGenericTypeRef of string * TSTypeRef seq
    | TSArray of TSTypeRef
    | TSUnion of TSTypeRef list
    | TSTypeParameterRef
    | TSAnyRef
    | TSNull
    | TSUndefined

type TSInterfaceMember =
    | TSField of TSTypeRef * string

type TSInterfaceDef = {
    name: string
    members: TSInterfaceMember seq
    }

type TSAccessModifier = Public | Private

type TSExpression =
    | TSNumberLiteral of decimal
    | TSStringLiteral of string
    
type TSFieldInitializer = TSExpression option

type TSMethodParameter = TSMethodParameter of (TSTypeRef * string)

type TSConstructorParameters = TSConstructorParameters of (TSTypeRef * string * TSAccessModifier) list

type TSStatement =
    | TSMethodCall of TSExpression * string * TSExpression list
    | TSReturn of TSExpression
    | TSLet of string * TSExpression
    | TSStatementList of TSStatement seq
    | TSNoOp

type TSClassMember =
    | TSClassField of TSTypeRef * TSAccessModifier * string * TSFieldInitializer
    | TSMethod of TSTypeRef * TSMethodParameter seq * TSAccessModifier * string * TSStatement
    | TSConstructor of TSConstructorParameters * TSStatement

type TSClassDef = {
    className: string
    members: TSClassMember list
}