module MasterView

open Suave.Html

let view name content = 
    html [] [
        head [] [
            title [] name
        ]
        body [] content
    ]