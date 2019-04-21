module AuctionsService

open FSharp.Data
    
type private Provider = JsonProvider<"http://uptime-auction-api.azurewebsites.net/api/auction", RootName = "Auction">
    
let listAuctionsAsync () = async {
    let! auctions = Provider.AsyncGetSamples ()
    return auctions |> Array.toList
}

let getAuctionAsync id = async {
    let! auctions = Provider.AsyncGetSamples ()
    let auctionOption = auctions |> Array.tryFind (fun auction -> auction.ProductId = id)
    return auctionOption
}