module Views

open Suave.Html
open Suave.Form
open Bacchus.Business.Auction
open Suave

type Search = {
    Category: string
    Name: string
}

let searchForm : Form<Search> =
    let properties = [
        TextProp((fun f -> <@ f.Name @>), [])
        TextProp((fun f -> <@ f.Category @>), [])
    ]
    Form(properties, [])

let renderAuctionTableRow (auction: Auction) =
    tag "tr" [] [
        tag "td" [] [ Text (auction.ProductId.ToString()) ]
        tag "td" [] [ Text auction.ProductName ]
        tag "td" [] [ Text auction.ProductCategory ]
        tag "td" [] [ Text auction.ProductDescription ]
        tag "td" [] [ Text (auction.BiddingEndDate.ToLocalTime().ToString()) ]
    ]

let auctionsView (auctions: Auction list) =
    html [] [
        head [] [
            title [] "Auctions"
        ]
        body [] [
            tag "h1" [] [
                Text "Auctions"
            ]
            div [] [
                tag "form" ["method", "GET"] [
                    input (fun f -> <@ f.Name @>) [] searchForm
                    input (fun f -> <@ f.Category @>) [] searchForm
                    tag "input" ["type", "submit"; "value", "Search"] [] 
                ]
            ]
            tag "table" [] [
                tag "thead" [] [
                    tag "tr" [] [
                        tag "th" [] [ Text "Id" ]
                        tag "th" [] [ Text "Name" ]
                        tag "th" [] [ Text "Category" ]
                        tag "th" [] [ Text "Description" ]
                        tag "th" [] [ Text "End time" ]
                    ]
                ]
                tag "tbody" [] (auctions |> List.map renderAuctionTableRow)
            ]
        ]
    ] |> htmlToString