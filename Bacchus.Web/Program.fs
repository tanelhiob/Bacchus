open Suave
open Suave.Filters
open Suave.Operators

[<EntryPoint>]
let main _ =

    let app = choose [
        GET >=> path "/" >=> Actions.listAuctionsGet
        GET >=> pathScan "/bid/%s" Actions.bidGet
        POST >=> pathScan "/bid/%s" Actions.bidPost
        GET >=> path "/bids" >=> Actions.listBidsGet
        RequestErrors.NOT_FOUND "path not found" ]
    
    startWebServer defaultConfig app

    0
