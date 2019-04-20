module MasterView

open Suave.Html

let view name content = 
    html [] [
        head [] [
            title [] name
        ]
        body [] [
            tag "header" [] [
                a "/bids" [] [ Text "check existing bids" ]
            ]        
            tag "article" [] content
        ]
    ]