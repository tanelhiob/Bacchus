module Auctions

open Suave
open Suave.Html
open Bacchus.Business
open Utils

type Search = {
    Name: string option
    Category: string option
}

let private renderAuctionTableRow (auction: Auction.Auction) =
    tag "tr" [] [
        tag "td" [] [ Text (auction.ProductId.ToString()) ]
        tag "td" [] [ Text auction.ProductName ]
        tag "td" [] [ Text auction.ProductCategory ]
        tag "td" [] [ Text auction.ProductDescription ]
        tag "td" [] [ Text (auction.BiddingEndDate.ToLocalTime().ToString()) ]
        tag "td" [] [ 
            tag "a" ["href", sprintf "/bid/%A" auction.ProductId] [ Text "bid" ]
        ]
    ]

let private renderSearch (search: Search) (categories: string list) =
 
    let renderOption activeOption option =
        if (activeOption |> Option.bind ((=) option >> Some) |> Option.defaultValue false) then
            tag "option" ["value", option; "selected", null] [ Text option ]
        else
            tag "option" ["value", option] [ Text option ]
        
    let renderEmptyOption activeOption =
        if Option.isSome activeOption then
            tag "option" ["value", null] [ Text "-- select category --"]
        else
            tag "option" ["selected", null; "value", null] [ Text "-- select category --"]

    let renderOptions allOptions activeOption =
        let emptyOption = [ renderEmptyOption activeOption ]
        let options = allOptions |> List.map (renderOption activeOption)
        emptyOption @ options

    tag "form" ["method", "GET"] [
        tag "input" ["type", "search"; "name", "name"; "value", (Option.defaultValue null search.Name)] []
        tag "select" ["name", "category"] (renderOptions categories search.Category)
        tag "button" ["type", "submit"] [ Text "Search" ]   
        tag "button" ["type", "reset"; "value", "reset"] [ Text "reset"]
    ]

let view (search, categories, (auctions: Auction.Auction list)) =
    [
        tag "h1" [] [
            Text "Auctions"
        ]
        div [] [
            renderSearch search categories
        ]
        tag "table" [] [
            tag "thead" [] [
                tag "tr" [] [
                    tag "th" [] [ Text "Id" ]
                    tag "th" [] [ Text "Name" ]
                    tag "th" [] [ Text "Category" ]
                    tag "th" [] [ Text "Description" ]
                    tag "th" [] [ Text "End time" ]
                    tag "th" [] [ ]
                ]
            ]
            tag "tbody" [] (auctions |> List.map renderAuctionTableRow)
        ]
    ] |> MasterView.view "auctions" |> htmlToString
     
let loadSearch dict =
    let name = dict ^^ "name" |> Option.ofChoice
    let category = dict ^^ "category" |> Option.ofChoice
    { Name = name; Category = category }

let index ctx = async {    

    let filterByName textOption (auction: Auction.Auction) =
        match textOption with
        | Some text when textHasContent text -> auction.ProductName.ToLower().Contains(text.ToLower())
        | _ -> true

    let filterByCategory categoryOption (auction: Auction.Auction) =
        match categoryOption with
        | Some category when textHasContent category -> auction.ProductCategory = category
        | _ -> true
    
    let! auctions = Auction.listAuctionsAsync ()
    
    let search = loadSearch ctx.request.query

    let uniqueCategories = auctions
                           |> List.map (fun auction -> auction.ProductCategory)
                           |> List.distinct

    let filteredAuctions = auctions
                           |> List.filter (filterByName search.Name)
                           |> List.filter (filterByCategory search.Category)
       
    return Ok (search, uniqueCategories, filteredAuctions)
}

let renderIndex = render index view
