module Netgular.TypeScriptModel

type TSType =
    | TSNumber
    | TSString

type TSClassMember =
    | TSField of TSType * string

type TSClass = {
    name: string
    members: TSClassMember list
    }
