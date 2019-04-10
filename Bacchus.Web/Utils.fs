module Utils

let (|HasTextValue|DoesntHaveTextValue|) input =
    match input with 
    | Some text when text <> "" -> HasTextValue text
    | _ -> DoesntHaveTextValue