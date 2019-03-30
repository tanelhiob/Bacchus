open System
open Suave
open System.Threading
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Actions

[<EntryPoint>]
let main _ =

    let app =  choose [
        path "/auctions" >=> GET >=> list
        path "/bid" >=> POST >=> OK "bidded"
        list
    ]

    let cts = new CancellationTokenSource()
    let conf = { defaultConfig with cancellationToken = cts.Token }

    let _, server = startWebServerAsync conf app
    
    Async.Start(server, cts.Token) 
    printfn "Make requests now"
    Console.ReadKey true |> ignore
    
    cts.Cancel()
    
    0
