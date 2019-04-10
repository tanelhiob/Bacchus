module Views

open System
open Suave.Html
open Bacchus.Business.Auction

let private masterView name content = 
    html [] [
        head [] [
            title [] name
        ]
        body [] content
    ]

let private renderAuctionTableRow (auction: Auction) =
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

let private renderSearch (search: Forms.Search) (categories: string list) =
 
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

let auctionsView search categories (auctions: Auction list) =
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
    ] |> masterView "auctions" |> htmlToString
     
let bidView (auction: Auction) =
    let endText = auction.BiddingEndDate.ToString()
    let timeUntilEnd = (auction.BiddingEndDate - DateTimeOffset.Now).ToString()

    [
        tag "h1" [] [ Text auction.ProductName ]
        div [] [ Text auction.ProductDescription ]
        div [] [ Text auction.ProductCategory ]
        div [] [ Text (sprintf "%s -> %s" endText timeUntilEnd) ] 

        tag "form" ["method", "POST"] [
            tag "input" ["type", "number"; "step", "0.01"; "name", "amount"] []
            tag "button" ["type", "submit"] [ Text "Bid" ]
        ]

        p [] [
            tag "a" ["href", "/"] [ Text "back to index"]
        ]
    ] |> masterView (sprintf "auction %A" auction.ProductId) |> htmlToString

let private renderBidsTableRow (bid: Db.Bid) =
    tag "tr" [] [
        tag "td" [] [ Text (bid.ProductId.ToString()) ]
        tag "td" [] [ Text (bid.Amount.ToString()) ]
        tag "td" [] [ Text (bid.Created.ToString("yyyy-MM-dd hh:mm:ss")) ]
    ]

let bidsView (bids: Db.Bid list) =
    [
        tag "table" [] [
            tag "thead" [] [
                tag "tr" [] [
                    tag "th" [] [ Text "ProductId" ]
                    tag "th" [] [ Text "Amount" ]
                    tag "th" [] [ Text "Created" ]
                ]
            ]
            tag "tbody" [] (List.map renderBidsTableRow bids)
        ]
    ] |> masterView "bids" |> htmlToString
