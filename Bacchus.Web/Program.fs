open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Utils

[<EntryPoint>]
let main _ =

    let app = choose [
        GET >=> path "/" >=> render Auctions.index Auctions.view
        GET >=> pathScan "/bid/%s" (fun id -> render (Bid.bidGet id) Bid.view)
        POST >=> pathScan "/bid/%s" (fun id -> render (Bid.bidPost id) Bid.view)
        GET >=> path "/bids" >=> render Bids.index Bids.view
        NOT_FOUND "path not found" ]
    
    startWebServer defaultConfig app

    0
