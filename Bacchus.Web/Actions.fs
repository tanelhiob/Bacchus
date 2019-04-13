module Actions

open System
open Suave
open Bacchus.Business
open Suave.Successful
open Suave.RequestErrors

open Utils

let listAuctionsGet ctx = async {    

    let filterByName textOption (auction: Auction.Auction) =
        match textOption with
        | Some text when textHasContent text -> auction.ProductName.ToLower() = text.ToLower()
        | _ -> true

    let filterByCategory categoryOption (auction: Auction.Auction) =
        match categoryOption with
        | Some category when textHasContent category -> auction.ProductCategory = category
        | _ -> true
    
    let! auctions = Auction.listAuctionsAsync ()
    
    let search = Forms.loadSearch ctx.request.query

    let uniqueCategories = auctions
                           |> List.map (fun auction -> auction.ProductCategory)
                           |> List.distinct

    let filteredAuctions = auctions
                           |> List.filter (filterByName search.Name)
                           |> List.filter (filterByCategory search.Category)
       
    let html = Views.auctionsView search uniqueCategories filteredAuctions

    return! OK html ctx
}

let private getAuctionAsync id = async {
    match id with
    | ValidGuid guid -> return! Auction.getAuctionAsync guid        
    | InvalidGuid -> return None
}
    
let bidGet id ctx = asyncOption {
    let! auction = getAuctionAsync id
    let html = Views.bidView auction
    return! OK html ctx
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
            let html = Views.bidView auction
            return! OK html ctx
        | None -> return! BAD_REQUEST "" ctx
    | None -> return! NOT_FOUND "" ctx
}

let listBidsGet ctx = async {
    let! bids = Db.getBidsAsync ()
    let html = Views.bidsView bids
    return! OK html ctx
}