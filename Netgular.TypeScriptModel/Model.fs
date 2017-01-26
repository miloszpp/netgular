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
