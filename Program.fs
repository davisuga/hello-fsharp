open FSharp.Json
open System.Net
open System
open System.IO
open System.Text.RegularExpressions
open Newtonsoft.Json.Linq

type Ad = { url: string }
type ListingProps = { adList: Ad list }
type Results = { listingProps: ListingProps }


let fetchPage (url) =
     WebRequest.Create(Uri(url))
        .GetResponse()
        .GetResponseStream() 
        |> fun stream -> new StreamReader(stream) 
        |> fun reader -> reader.ReadToEnd()


let getSearchResults (term) =
    "https://mg.olx.com.br/belo-horizonte-e-regiao?q="
    + term
    |> fetchPage

    

let getDataJson (html: string) =
    let matches =
        Regex.Match(html, "data-json=\"(.+)\">.+/script>")


    let value =
        matches.Groups.[1].Value.Replace("&quot;", "\"")

    let rawObj =
        JObject
            .Parse(value)
            .SelectToken "listingProps.adList"
        |> string

    JArray.Parse(rawObj).ToString()

let getUrl (item) = item.url


let getDataFromUrl (url) =
    let page = url |> fetchPage
    getDataJson (page)


let getDataFromItem (item: Ad) = item |> getUrl |> getDataFromUrl

let getOnlyValidResults (results: Results) =
    results.listingProps.adList
    |> List.map getDataFromItem

[<EntryPoint>]
let main argv =
    "PS5" |> getSearchResults |> getDataJson |> Console.WriteLine


    0 // return an integer exit code
