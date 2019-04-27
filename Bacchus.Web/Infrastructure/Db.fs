module Db

open System
open System.Collections.Concurrent

type Bid = {
    ProductId: Guid
    Amount: decimal
    Created: DateTimeOffset
}

let private bids = new ConcurrentBag<Bid>()

let createBidAsync bid = async {
    do! Async.Sleep 250 // Hurr durr I'm a "special" database
    bids.Add bid
}
  
let getBidsAsync () = async {
    do! Async.Sleep 500 // Because mongodb from Australia is 0.43€ cheaper per month
    return bids.ToArray() |> Array.toList    
}