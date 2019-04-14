﻿module Bid

open System
open Suave
open Suave.Html
open Suave.Successful
open Suave.RequestErrors
open Bacchus.Business
open Utils

let view (auction: Auction.Auction) =
    let endText = auction.BiddingEndDate.ToString()
    let timeUntilEnd = (auction.BiddingEndDate - DateTimeOffset.Now).ToString()

    [
        tag "h1" [] [ Text auction.ProductName ]
        div [] [ Text auction.ProductDescription ]
        div [] [ Text auction.ProductCategory ]
        div [] [ Text (sprintf "%s -> %s" endText timeUntilEnd) ] 

        tag "form" ["method", "POST"] [
            tag "input" ["type", "number"; "step", "0.01"; "name", "amount"] []
            tag "button" ["type", "submit"] [ Text "Bid" ]
        ]

        p [] [
            tag "a" ["href", "/"] [ Text "back to index"]
        ]
    ] |> MasterView.view (sprintf "auction %A" auction.ProductId) |> htmlToString

let private getAuctionAsync id = async {
    match id with
    | ValidGuid guid -> return! Auction.getAuctionAsync guid        
    | InvalidGuid -> return None
}
    
let bidGet id ctx = async {
    let! auctionOption = getAuctionAsync id
    match auctionOption with
    | Some auction -> 
        let html = view auction
        return! OK html ctx
    | None ->
        return! NOT_FOUND "auction not found" ctx
}

let private getAmountOption dict =
    dict ^^ "amount"
    |> Option.ofChoice
    |> Option.bind (function | ValidDecimal decimal -> Some decimal | _ -> None)

let createBidAsync (auction: Auction.Auction) amount = async {
    let bid: Db.Bid = {
        ProductId = auction.ProductId
        Amount = amount
        Created = DateTimeOffset.UtcNow
    }
    do! Db.createBidAsync bid
}

let bidPost id ctx = async {
    let! auctionOption = getAuctionAsync id 
    match auctionOption with
    | Some auction ->
        let amountOption = getAmountOption ctx.request.form
        match amountOption with
        | Some amount ->
            do! createBidAsync auction amount
            let html = view auction
            return! OK html ctx
        | None -> return! BAD_REQUEST "" ctx
    | None -> return! NOT_FOUND "" ctx
}