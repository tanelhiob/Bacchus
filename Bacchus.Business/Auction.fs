namespace Bacchus.Business

open FSharp.Data

module Auction =
    
    type private Provider = JsonProvider<"http://uptime-auction-api.azurewebsites.net/api/auction", RootName = "Auction">
    
    type Auction = Provider.Auction

    let listAuctionsAsync () = async {
        let! auctions = Provider.AsyncGetSamples()
        return auctions |> Array.toList
    }