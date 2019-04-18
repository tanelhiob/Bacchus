open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

[<EntryPoint>]
let main _ =

    let app = choose [
        GET >=> path "/" >=> Auctions.renderIndex
        GET >=> pathScan "/bid/%s" Bid.renderBidGet
        POST >=> pathScan "/bid/%s" Bid.renderBidPost
        GET >=> path "/bids" >=> Bids.renderIndex
        NOT_FOUND "path not found" ]
    
    startWebServer defaultConfig app

    0
