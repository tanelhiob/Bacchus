module MasterView

open Suave.Html

let table = tag "table"
let thead = tag "thead"
let tbody = tag "tbody"
let th = tag "th"
let tr = tag "tr"
let td = tag "td"

let header = tag "header"
let article = tag "article"
let nav = tag "nav"
let ul = tag "ul"
let li = tag "li"

let h3 = tag "h3"

let form method attributes = tag "form" (("method", method)::attributes)
let label ``for`` attributes = tag "label" (("for", ``for``)::attributes)
let input ``type`` name attributes = tag "input" (("type", ``type``)::("name", name)::attributes)
let select name attributes = tag "select" (("name", name)::attributes)
let option value attributes = tag "option" (("value", value)::attributes)
let selectedOption value attributes = tag "option" (("value", value)::("selected",null)::attributes)
let submitButton attributes = tag "button" (("type", "submit")::attributes)
let resetButton attributes = tag "button" (("type", "reset")::attributes)

let masterView name content = 
    html [] [
        head [] [
            title [] name
            
            //link [ "rel","stylesheet"; "type","text/css"; "href","/bootstrap/bootstrap-reboot.min.css" ]
            //link [ "rel","stylesheet"; "type","text/css"; "href","/bootstrap/bootstrap-grid.min.css" ]
            link [ "rel","stylesheet"; "type","text/css"; "href","/bootstrap/bootstrap.min.css" ]
            //script ["src", "/bootstrap/bootstrap.bundle.min.js"] []
            script ["src", "/bootstrap/bootstrap.min.js"] []

            link [ "rel","stylesheet"; "type","text/css"; "href","/master.css" ]
        ]
        body [] [
            nav ["class","navbar"] [
                ul ["class", "nav"] [
                    li ["class", "nav-item"] [
                        a "/" ["class", "nav-link active"] [Text "Auctions"]
                    ]
                    li ["class", "nav-item"] [
                        a "/bids" ["class", "nav-link"] [Text "Bids"]
                    ]
                ]
            ]
            article ["class", "container"] content
        ]
    ]