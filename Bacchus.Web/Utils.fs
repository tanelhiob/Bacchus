﻿module Utils

open System
open System.Globalization
open Suave.Successful
open Suave.RequestErrors

// all hail satans black magic
let asyncToAsyncOption async =
    async |> Async.bind (Some >> Async.result)

let toEstonianTime (dateTimeOffset: DateTimeOffset) =
    let estonianTimeZone = TimeZoneInfo.FindSystemTimeZoneById "FLE Standard Time"
    TimeZoneInfo.ConvertTime (dateTimeOffset, estonianTimeZone)

let textHasContent (text: string) =
    String.IsNullOrWhiteSpace text |> not
    
let (|ValidGuid|InvalidGuid|) (input: string) =
    match Guid.TryParse (input) with
    | (true, guid) -> ValidGuid guid
    | (false, _) -> InvalidGuid

let (|ValidDecimal|InvalidDecimal|) (input: string) = 
    match Decimal.TryParse (input, NumberStyles.Any, CultureInfo.InvariantCulture) with
    | (true, amount) -> ValidDecimal amount
    | (false, _) -> InvalidDecimal

let render action view ctx = async {
    match! action ctx with
    | Some result -> return! OK (view result) ctx
    | None -> return! BAD_REQUEST "the train crashed" ctx
}