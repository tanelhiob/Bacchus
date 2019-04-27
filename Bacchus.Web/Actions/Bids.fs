module Bids

open Suave.Html
open MasterView
open System

let private renderBidsTableRow (bid: Db.Bid) =
    let remainingTime = DateTimeOffset.UtcNow - bid.Created
    tr [] [
        td [] [Text (sprintf "%A" bid.ProductId)]
        td [] [Text (sprintf "%.2f" bid.Amount)]
        td [] [Text (remainingTime.ToString(""))]
    ]

let view bids =
    [
        h3 [] [Text "Bids"]
        table ["class","table"] [
            thead [] [
                tr [] [
                    th [] [Text "ProductId"]
                    th [] [Text "Amount"]
                    th [] [Text "Created"]
                ]
            ]
            tbody [] (List.map renderBidsTableRow bids)
        ]
    ] |> masterView "Bids" |> htmlToString

let index _ = async {
    let! bids = Db.getBidsAsync ()
    return Some bids
}