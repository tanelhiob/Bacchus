module Actions

open System
open System.Globalization
open Suave
open Bacchus.Business
open Suave.Successful
open Suave.RequestErrors

open Utils

let listAuctionsGet (ctx: HttpContext) = async {    

    let search : Forms.Search = Forms.loadSearch ctx.request.query

    let! auctions = Auction.listAuctionsAsync ()

    let categories =
        auctions
        |> List.map (fun auction -> auction.ProductCategory)
        |> List.distinct

    let filterByName (textOption: string option) (auction: Auction.Auction) =
        match textOption with
        | HasTextValue text -> auction.ProductName.ToLower().Contains(text.ToLower())
        | DoesntHaveTextValue -> true

    let filterByCategory (categoryOption: string option) (auction: Auction.Auction) =
        match categoryOption with
        | HasTextValue category -> auction.ProductCategory = category
        | DoesntHaveTextValue -> true
         
    let filteredAuctions =
        auctions
        |> List.filter (filterByName search.Name)
        |> List.filter (filterByCategory search.Category)
       
    let html = Views.auctionsView search categories filteredAuctions

    return! OK html ctx
}

let private getAuctionAsync (id: string) = async {  
    match Guid.TryParse(id) with
    | (true, guid) -> 
        let! auctions = Auction.listAuctionsAsync () 
        return auctions |> List.tryFind (fun auction -> auction.ProductId = guid)
    | (false, _) ->
        return None
}

let bidGet (id: string) (ctx: HttpContext) = async {

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