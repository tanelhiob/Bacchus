open Suave
open Suave.Filters
open Suave.Operators

[<EntryPoint>]
let main _ =

    let app = choose [
        GET >=> path "/" >=> Auctions.index
        GET >=> pathScan "/bid/%s" Bid.bidGet
        POST >=> pathScan "/bid/%s" Bid.bidPost
        GET >=> path "/bids" >=> Bids.index
        RequestErrors.NOT_FOUND "path not found" ]
    
    startWebServer defaultConfig app

    0
