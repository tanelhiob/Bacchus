module Bids

open Suave.Html
open MasterView

let private renderBidsTableRow (bid: Db.Bid) =
    tr [] [
        td [] [Text (sprintf "%A" bid.ProductId)]
        td [] [Text (sprintf "%.2f" bid.Amount)]
        td [] [Text (bid.Created.ToString("HH:mm:ss dd/MM/yyyy"))]
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