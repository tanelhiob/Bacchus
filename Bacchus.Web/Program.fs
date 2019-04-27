open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Utils
open System.IO

[<EntryPoint>]
let main _ =

    let app = choose [
        GET >=> path "/" >=> render Auctions.index Auctions.view
        GET >=> pathScan "/bid/%s" (fun id -> render (Bid.bidGet id) Bid.view)
        POST >=> pathScan "/bid/%s" (fun id -> render (Bid.bidPost id) Bid.view)
        GET >=> path "/bids" >=> render Bids.index Bids.view
        Files.browseHome
        NOT_FOUND "path not found" ]
    
    let myHomeFolder = Path.Combine(Directory.GetCurrentDirectory(), "public") 
    let config = { defaultConfig with homeFolder = Some(myHomeFolder) }   
    
    startWebServer config app

    0
