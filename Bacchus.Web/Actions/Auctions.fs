module Auctions

open Suave
open Suave.Html
open Utils
open MasterView

type Search = {
    Name: string option
    Category: string option
}

let private renderAuctionTableRow (auction: AuctionsService.Provider.Auction) =
    tr [] [
        td [] [
            a (sprintf "/bid/%A" auction.ProductId) ["class","btn btn-success"] [Text "bid"]
        ]
        td [] [Text auction.ProductName]
        td [] [Text auction.ProductCategory]
        td [] [Text auction.ProductDescription]
        td [] [Text ((auction.BiddingEndDate |> toEstonianTime).ToString("HH:mm:ss dd/MM"))]
    ]
   
let private rendedAuctionsTable (auctions: AuctionsService.Provider.Auction list) =
    table ["class","table"] [
        thead [] [
            tr [] [
                th [] []
                th [] [Text "Name"]
                th [] [Text "Category"]
                th [] [Text "Description"]
                th [] [Text "End time"]
            ]
        ]
        tbody [] (auctions |> List.map renderAuctionTableRow)
    ]

let private renderSearch (search: Search) (categories: string list) =
 
    let renderOption activeOption value =
        let isActive = activeOption |> Option.bind (fun active -> active = value |> Some) |> Option.defaultValue false        
        let optionBuilder =  if isActive then selectedOption else option
        optionBuilder value [] [Text value]

    let renderEmptyOption activeOption =
        let isActiveOptionSelected = Option.isSome activeOption
        let optionBuilder = if isActiveOptionSelected then option else selectedOption
        optionBuilder null [] [Text "-- filter category --"]

    let renderOptions allOptions activeOption =
        let emptyOption = renderEmptyOption activeOption
        let options = allOptions |> List.map (renderOption activeOption)
        emptyOption::options

    let searchValue = search.Name |> Option.defaultValue null
    let categoryOptions = search.Category |> renderOptions categories

    form "GET" ["class","form-inline ml-auto"] [
        div ["class","input-group"] [
            input "search" "name" ["class","form-control"; "value", searchValue; "id", "name"; "placeholder", "search..."] []
            select "category" ["class","form-control"] categoryOptions
            div ["class","input-group-append"] [
                submitButton ["class","btn btn-primary"] [Text "Search"]   
                resetButton ["class", "btn btn-secondary"] [Text "Reset"]
            ]
        ]    
    ]

let view (search, categories, auctions) =
    [
        div ["class","d-flex"] [
            h3 ["class","mr-auto"] [Text "Auctions"]
            renderSearch search categories                
        ]
        rendedAuctionsTable auctions      
    ] |> masterView "Auctions" |> htmlToString
     
let private loadSearch dict =
    let name = dict ^^ "name" |> Option.ofChoice
    let category = dict ^^ "category" |> Option.ofChoice
    { Name = name; Category = category }

let index ctx = async {    

    let filterByName textOption (auction: AuctionsService.Provider.Auction) =
        match textOption with
        | Some text when textHasContent text -> auction.ProductName.ToLower().Contains(text.ToLower())
        | _ -> true

    let filterByCategory categoryOption (auction: AuctionsService.Provider.Auction) =
        match categoryOption with
        | Some category when textHasContent category -> auction.ProductCategory = category
        | _ -> true
    
    let! auctions = AuctionsService.listAuctionsAsync ()
    
    let search = loadSearch ctx.request.query

    let uniqueCategories =
        auctions
        |> List.map (fun auction -> auction.ProductCategory)
        |> List.distinct

    let filteredAuctions =
        auctions
        |> List.filter (filterByName search.Name)
        |> List.filter (filterByCategory search.Category)
       
    return Some (search, uniqueCategories, filteredAuctions)
}
