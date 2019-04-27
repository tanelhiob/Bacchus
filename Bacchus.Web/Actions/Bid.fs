module Bid

open System
open Suave
open Suave.Html
open Utils

let view (auction: AuctionsService.Provider.Auction, isPostBack) =
    let endText = auction.BiddingEndDate.ToString()
    let timeUntilEnd = (auction.BiddingEndDate - DateTimeOffset.Now).ToString()

    let successMessage =
        if isPostBack then div [] [ Text "bid placed!" ]
        else Text ""

    [
        tag "h1" [] [ Text auction.ProductName ]
        div [] [ Text auction.ProductDescription ]
        div [] [ Text auction.ProductCategory ]
        div [] [ Text (sprintf "%s -> %s" endText timeUntilEnd) ] 

        tag "form" ["method", "POST"] [
            tag "input" ["type", "number"; "step", "0.01"; "name", "amount"] []
            tag "button" ["type", "submit"] [ Text "Bid" ]
        ]

        successMessage

        p [] [
            tag "a" ["href", "/"] [ Text "back to index"]
        ]
    ] |> MasterView.masterView (sprintf "auction %A" auction.ProductId) |> htmlToString

let private getAuctionAsyncOption id = async {
    match id with
    | ValidGuid guid -> return! AuctionsService.getAuctionAsync guid
    | InvalidGuid -> return None
}
    
let bidGet id _ = asyncOption { 
    let! auction = getAuctionAsyncOption id
    return (auction, false)
}

let private getAmountOption dict =
    dict ^^ "amount"
    |> Option.ofChoice
    |> Option.bind (function | ValidDecimal decimal -> Some decimal | InvalidDecimal -> None)

let private createBidAsync (auction: AuctionsService.Provider.Auction) amount = async {
    do! Db.createBidAsync {
        ProductId = auction.ProductId
        Amount = amount
        Created = DateTimeOffset.UtcNow
    }
}

let bidPost id ctx = asyncOption {
    let! amount  = Async.result (getAmountOption ctx.request.form)
    let! auction = getAuctionAsyncOption id

    return! async {
        do! createBidAsync auction amount
        return Some (auction, true)
    }
}