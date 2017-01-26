module Netgular.Config

type NullableMode =
    | Disabled
    | Null
    | Undefined
    | NullUndefined

type Config = {
     nullableMode: NullableMode
    }