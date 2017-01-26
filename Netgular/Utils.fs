module Netgular.Utils

type OptionBuilder() =
    member x.Bind(v,f) = Option.bind f v
    member x.Return v = Some v
    member x.ReturnFrom o = o

let opt = OptionBuilder()

let getOrElse p o = match o with | Some v -> v | _ -> p
