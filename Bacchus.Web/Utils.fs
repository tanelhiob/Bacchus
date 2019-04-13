module Utils

open System

let textHasContent (text: string) =
    String.IsNullOrWhiteSpace text |> not
    
let (|ValidGuid|InvalidGuid|) (input: string) =
    match Guid.TryParse (input) with
    | (true, guid) -> ValidGuid guid
    | (false, _) -> InvalidGuid
