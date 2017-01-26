﻿module Netgular.Resolvers

open Microsoft.CodeAnalysis;

open Netgular.TypeScriptModel;
open Netgular.Utils;
open Netgular.Config;

let toNamedTypeSymbol: (ITypeSymbol -> INamedTypeSymbol option) = function
    | :? INamedTypeSymbol as named -> Some named
    | _ -> None    

let resolvePrimitive config (namedSymbol: INamedTypeSymbol) =
        match namedSymbol.SpecialType with
        | SpecialType.System_String -> Some TSStringRef
        | SpecialType.System_Int32 -> Some TSNumberRef
        | SpecialType.System_Int64 -> Some TSNumberRef
        | SpecialType.System_Int16 -> Some TSNumberRef
        | SpecialType.System_Double -> Some TSNumberRef
        | _ -> None

let rec resolveEnumerable config (namedSymbol: INamedTypeSymbol) =
    let enumerable = namedSymbol.Interfaces |> Seq.exists (fun i -> i.Name = "IEnumerable")
    match enumerable, namedSymbol.IsGenericType with
    | true, true -> namedSymbol.TypeArguments |> Seq.head |> resolveTypeRef config |> TSArray |> Some
    | true, false -> TSArray TSAnyRef |> Some
    | _ -> None

and resolveNullable config (namedSymbol: INamedTypeSymbol) =
    if namedSymbol.Name = "Nullable" then
        let inner = namedSymbol.TypeArguments |> Seq.head |> (resolveTypeRef config)
        Some <| match config.nullableMode with
                | NullableMode.Disabled -> inner
                | NullableMode.Null -> TSUnion [inner; TSNull]
                | NullableMode.Undefined -> TSUnion [inner; TSUndefined]
                | NullableMode.NullUndefined -> TSUnion [inner; TSNull; TSUndefined]
    else None

and resolveGeneric config (namedSymbol: INamedTypeSymbol) = 
    if (namedSymbol.IsGenericType) then
        let typeArgs = namedSymbol.TypeArguments |> Seq.map (resolveTypeRef config)
        TSGenericTypeRef (namedSymbol.Name, typeArgs) |> Some
    else None

and resolveTypeNameRef config (namedSymbol: INamedTypeSymbol) =
    Some <| TSTypeRef namedSymbol.Name

and resolveTypeRef config (symbol:ITypeSymbol) =
    opt {
        let! namedSymbol = toNamedTypeSymbol symbol
        let resolvers = [ 
            resolvePrimitive;
            resolveEnumerable;
            resolveNullable;
            resolveGeneric;
            resolveTypeNameRef
        ]
        let resolve resolver = resolver config namedSymbol
        return! resolvers |> Seq.tryPick resolve
        } |> getOrElse TSAnyRef
