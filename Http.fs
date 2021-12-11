module Http

open HttpFs.Client

let fetchPage (url) =
    Request.createUrl Get url
    |> Request.responseAsString

