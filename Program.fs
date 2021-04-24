// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System

let negate x = x * -1
let square x = x * x
let print x = printfn "The number is: %d" x

let ``square, negate, then print`` x = x |> square |> negate |> print

let powAndSumList (numberList: int []) =
    numberList
    |> Array.map (fun number -> number * number)
    |> Array.sum
// Define a function to construct a message to print
let from whom = sprintf "from %s" whom

[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    ``square, negate, then print`` 10
    let doesLotsOfStuff = square >> square >> negate >> print
    doesLotsOfStuff 10
    0 // return an integer exit code
