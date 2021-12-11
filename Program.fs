module OlxScrapper 

open FSharp.Json
open System.Net
open System
open FSharp.Data
open System.Text.RegularExpressions
open Http
open Hopac

// type Results = { listingProps: ListingProps }

let trace (x: string) =
    Console.WriteLine(x)
    x

[<Literal>]
let ResolutionFolder = __SOURCE_DIRECTORY__

type Results = JsonProvider<"./results.json">

let unEscape (x: string) = x.Replace("&quot;", "\"")

let buildSearchUrl (term, state, region) =
    $"https://{state}.olx.com.br/{region}?q={term}"

let getDataJson (html: string) =
    Regex
        .Match(
            html,
            "data-json=\"(.+&quot;})\">"
        )
        .Groups.[1]
        .Value
    |> unEscape
    |> trace
    
let getUrl (item: Results.AdList) = item.Url


let getProductPageData (url) =
    job {
        let! productPage = fetchPage url
        Console.WriteLine("Gettin data from url:")
        Console.WriteLine(url)
        return productPage  |> getDataJson
    }

// let optionsMap (x: 'a option list, fn: 'a -> 'b) =
//     x |> Array.map (Option.map fn)
let getDataFromItem =
    getUrl
    >> Option.map getProductPageData
    >> Option.toArray

let getOnlyValidResults (results: Results.Root) =
    results.ListingProps.AdList
    |> Array.map getDataFromItem
    |> Array.concat
    |> Job.conCollect
    |> run


let searchForProducts =
    buildSearchUrl
    >> fetchPage // Fetching search page
    >> run
    >> getDataJson // Getting results
    >> Results.Parse
    >> getOnlyValidResults // Removing ads
    >> Console.WriteLine

[<EntryPoint>]
let main argv =

    (argv[0], argv[1], argv[2])
    |> searchForProducts

    0 // return an integer exit code
