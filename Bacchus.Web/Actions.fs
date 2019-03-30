module Actions

open Suave
open Bacchus.Business
open Suave.Successful

let list (httpContext: HttpContext) = async {

    let! auctions = Auction.listAuctionsAsync()
    let html = Views.auctionsView auctions
    return! OK html httpContext
}

let search (httpContext: HttpContext) = async {

    return! OK "success" httpContext
}