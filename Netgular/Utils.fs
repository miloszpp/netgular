module Netgular.Utils

type OptionBuilder() =
    member x.Bind(v,f) = Option.bind f v
    member x.Return v = Some v
    member x.ReturnFrom o = o

let opt = OptionBuilder()

let getOrElse p o = match o with | Some v -> v | _ -> p

let castAs<'a when 'a : null> (o:obj) = 
    match o with
    | :? 'a as res -> Some(res)
    | _ -> None

type OrElseBuilder() =
    member x.Return a = Some a
    member x.ReturnFrom o = o
    member x.Combine (a, f) =
        match a with
        | Some(_) -> a
        | None -> f()
    member this.Zero = None
    member this.Delay f = f
    member this.Run f = f()

let orElse = OrElseBuilder()