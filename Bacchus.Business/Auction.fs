namespace Bacchus.Business

open FSharp.Data
open System

module Auction =
    
    type private Provider = JsonProvider<"http://uptime-auction-api.azurewebsites.net/api/auction", RootName = "Auction">
    
    type Auction = Provider.Auction

    type Bid = {
        ProductId: int
        CreatedOn: DateTimeOffset
        BidderName: string
        Amount: int
    }
    
    let private bids = new System.Collections.Generic.List<Bid>()
    
    let listAuctionsAsync () = async {
        let! auctions = Provider.AsyncGetSamples()
        return auctions |> Array.toList
    }

    let createBidAsync bidderName productId amount = async {
        {
            ProductId = productId
            Amount = amount
            CreatedOn = DateTimeOffset.UtcNow
            BidderName = bidderName
        }
        |> bids.Add
    }