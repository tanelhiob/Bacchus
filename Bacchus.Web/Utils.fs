module Utils

open System
open System.Globalization

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

