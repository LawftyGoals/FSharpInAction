//Chapter 9

//OPTIONS
let mutable presentNumber: int option = Some 10
let missingNumber: int option = None
let mandatoryNumber: int = 10

printfn $"{presentNumber.Value}"

presentNumber <- Some 11
printfn $"{presentNumber.Value}"

presentNumber <- None

printfn $"{missingNumber}"

printfn $"{presentNumber.IsNone}"

let description =
    match presentNumber with
    | Some number -> $"the number is {number}"
    | None -> "is not a number"

printfn $"{description}"


//OPTION.MAP

type Customer = { Name: string; Age: int }



let getAge customer =
    match customer with
    | Some c -> Some c.Age
    | None -> None

let getAgeMap customer = customer |> Option.map (fun c -> c.Age)

let classifyAge age =
    age
    |> Option.map (fun a ->
        match a with
        | a when a < 18 -> "Child"
        | _ -> "Adult")
(*    match age with
    | age when age < 18 -> "Child"
    | age when age < 64 -> "Adult"
    | _ -> "Senior"*)

let optionalClassification optionalCustomer =
    optionalCustomer |> Option.map getAgeMap |> Option.map classifyAge

//Compose operator

let operatorClassification optionalCustomer =
    optionalCustomer |> Option.map (getAge >> classifyAge)

open System.IO

let tryGetFileContents fileName =
    if File.Exists fileName then
        Some(File.ReadAllText fileName)
    else
        printfn "aint no file here boss"
        None

let countWords (text: string) = text.Split " " |> Array.length

let countWordsInFile fileName =
    fileName |> tryGetFileContents |> Option.map countWords


let targetFile = Path.Combine(__SOURCE_DIRECTORY__, "boss.txt")
countWordsInFile targetFile




//vibetest

let getTextFromFile fileName =
    if File.Exists fileName then
        Some(File.ReadAllText fileName)
    else
        None

let wordCount (text: string) = text.Split " " |> Array.length

let getWordCountFromFile fileName =
    fileName |> getTextFromFile |> Option.map wordCount


getWordCountFromFile "boss.txt"


//OPTION BIND
type OptionalCustomer = { Name: string; Age: int option }
let bob = Some { Name = "Name"; Age = Some 42 }

let getAgeS b = b.Age


let ageToGroup: int option = bob |> Option.bind getAgeS

match ageToGroup with
| Some age -> age
| None -> 0
