module Actions

open System
open System.Globalization
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

let bidGet (id: string) ctx = async {

    let! auctionOption = getAuctionAsync id

    return!
        match auctionOption with
        | Some auction ->
            let html = Views.bidView auction
            OK html ctx
        | None ->
            NOT_FOUND (sprintf "Active auction %s not found" id) ctx
}

let private getAmountOption dict =
    let amountTextOption = dict ^^ "amount" |> Option.ofChoice
    match amountTextOption with
    | Some amountText ->
        match Decimal.TryParse (amountText, NumberStyles.Any, CultureInfo.InvariantCulture) with
        | (true, amount) -> Some amount
        | (false, _) -> None
    | None -> None

let bidPost (id: string) (ctx: HttpContext) = async {

    let! auctionOption = getAuctionAsync id

    return! 
        match auctionOption with
        | Some auction ->
            let amountOption = getAmountOption ctx.request.form
            match amountOption with
            | Some amount -> 
                let bid : Db.Bid = {
                    ProductId = auction.ProductId
                    Amount = amount
                    Created = DateTimeOffset.UtcNow                    
                }               
                async {
                    do! Db.createBidAsync bid
                    let html = Views.bidView auction
                    return! OK html ctx
                }
            | None ->
                BAD_REQUEST "Bid amount not sent" ctx
        | None ->
            NOT_FOUND (sprintf "Active auction %s not found" id) ctx

}

let listBidsGet (ctx: HttpContext) = async {
    let! bids = Db.getBidsAsync ()
    let html = Views.bidsView bids
    return! OK html ctx
}