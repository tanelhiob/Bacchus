module Forms

open Suave.Utils.Collections

type Search = {
    Name: string option
    Category: string option
}

let loadSearch dict =
    let name = dict ^^ "name" |> Option.ofChoice
    let category = dict ^^ "category" |> Option.ofChoice
    { Name = name; Category = category }