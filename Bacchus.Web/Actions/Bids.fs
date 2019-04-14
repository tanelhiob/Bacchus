﻿module Bids

open Suave.Html
open Suave.Successful

let private renderBidsTableRow (bid: Db.Bid) =
    tag "tr" [] [
        tag "td" [] [ Text (bid.ProductId.ToString()) ]
        tag "td" [] [ Text (bid.Amount.ToString()) ]
        tag "td" [] [ Text (bid.Created.ToString("yyyy-MM-dd hh:mm:ss")) ]
    ]

let view (bids: Db.Bid list) =
    [
        tag "table" [] [
            tag "thead" [] [
                tag "tr" [] [
                    tag "th" [] [ Text "ProductId" ]
                    tag "th" [] [ Text "Amount" ]
                    tag "th" [] [ Text "Created" ]
                ]
            ]
            tag "tbody" [] (List.map renderBidsTableRow bids)
        ]
    ] |> MasterView.view "bids" |> htmlToString

let index ctx = async {
    let! bids = Db.getBidsAsync ()
    let html = view bids
    return! OK html ctx
}