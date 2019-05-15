module Bid

open System
open Suave
open Suave.Html
open Utils
open MasterView

let private renderForm isPostBack =
    if not isPostBack then
        [
            div ["class","form-group"] [
                label "form-amount" [] [Text "Amount"]
                input "number" "amount" ["step", "0.01"; "value","00.00"; "id","form-amount"; "class","form-control"] []
            ]
            submitButton ["class","btn btn-success"] [Text "Bid"]
        ]
    else
        [
            div ["class","form-group"] [
                label "form-amount" [] [Text "Amount"]
                input "number" "amount" ["step", "0.01"; "value","00.00"; "id","form-amount";"disabled",null; "class","form-control"] []
                small ["class","form-text text-success"] [Text "Bid created!"]
            ]
            submitButton ["class","btn btn-success"; "disabled",null] [Text "Bid"]
        ]
    |> form "Post" []
    
let view (auction: AuctionsService.Provider.Auction, isPostBack) =
    let timeUntilEnd = auction.BiddingEndDate - DateTimeOffset.UtcNow   
    [
        h3 [] [Text auction.ProductName]

        dl ["class","row"] [
            dt ["class","col-sm-3"] [Text "Description"]
            dd ["class","col-sm-9"] [Text auction.ProductDescription]
            dt ["class","col-sm-3"] [Text "Category"]
            dd ["class","col-sm-9"] [Text auction.ProductCategory]
            dt ["class","col-sm-3"] [Text "Expiration"]
            dd ["class","col-sm-9"] [Text (timeUntilEnd.ToString())]
        ]

        renderForm isPostBack

    ] |> masterView auction.ProductName |> htmlToString

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
    do! createBidAsync auction amount |> asyncToAsyncOption
    return (auction, true)
}