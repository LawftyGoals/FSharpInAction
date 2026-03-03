open System
let isEvenSecond (args: Timers.ElapsedEventArgs) = args.SignalTime.Second % 2 = 0

let printTime (args: Timers.ElapsedEventArgs) =
    printfn $"Event was rasied at{args.SignalTime}"

let t = new System.Timers.Timer(Interval = 1000., Enabled = true)

t.Elapsed |> Event.filter isEvenSecond |> Event.add printTime

t.Start()
t.Stop()

open System

let inputs: string seq =
    seq {
        while true do
            Console.Write "Please enter your command: "
            Console.ReadLine()
    }

let isNotExit input = input = "D"


let bar =
    inputs
    |> Seq.takeWhile isNotExit
    |> Seq.choose (fun inp -> if inp = null then None else Some inp)
    |> Seq.fold
        (fun acc test ->
            if test = "S" then
                Seq.append acc (seq { "S" })
            else
                acc |> Seq.append (seq { "N" }))
        (seq { })


let seq1 = seq { "A" }

let test2 = seq1 |> Seq.append (seq { "B" })

for str in test2 do
    printf $"{str}\n"


// DESERIALIZE JSON TO F#
open System.Text.Json

module Deserializers =
    type Brand = Brand of string

    type Strings =
        | Six
        | Seven
        | Eight
        | Twelve

    type Pickup =
        | Single
        | Humbucker

    type Kind =
        | Acoustic
        | Electric of Pickup list

    type Guitar =
        { Brand: Brand
          Strings: Strings
          Kind: Kind }

    let ibanezGuitar =
        """{"Brand": "Ibanez", "Strings": "6", "Pickups": ["H", "S", "H"]}"""

// fails: let deserializedGuitar = JsonSerializer.Deserialize<Guitar> ibanezGuitar
#r "nuget:FsToolkit.ErrorHandling"
open FsToolkit.ErrorHandling

type RawGuitar =
    { Brand: string
      Strings: string
      Pickups: string list }

let tryAsFullGuitar (raw: RawGuitar) =
    result {
        let! brand =
            if String.IsNullOrWhiteSpace raw.Brand then
                Error "Brand is mandatory"
            else
                Ok(Deserializers.Brand raw.Brand)

        let! strings =
            match raw.Strings with
            | "6" -> Ok Deserializers.Six
            | "7" -> Ok Deserializers.Seven
            | "8" -> Ok Deserializers.Eight
            | "12" -> Ok Deserializers.Twelve
            | value -> Error $"Invalid strings value {value}"

        let! pickups =
            raw.Pickups
            |> List.traverseResultM (fun pickup ->
                match pickup with
                | "S" -> Ok Deserializers.Single
                | "H" -> Ok Deserializers.Humbucker
                | value -> Error $"Invalid pickups value {value}")

        return
            { Deserializers.Guitar.Brand = brand
              Deserializers.Guitar.Strings = strings
              Deserializers.Guitar.Kind =
                match pickups with
                | [] -> Deserializers.Acoustic
                | pickups -> Deserializers.Electric pickups

            }
    }

let domainGuitar =
    Deserializers.ibanezGuitar |> JsonSerializer.Deserialize |> tryAsFullGuitar

// JSON SERIALIZATION
let createReport (guitars: Deserializers.Guitar list) =
    guitars
    |> List.countBy (fun guitar -> guitar.Brand)
    |> List.map (fun (Deserializers.Brand brand, count) -> {| Brand = brand; Guitars = count |})
    |> JsonSerializer.Serialize


//JSON TYPE PROVIDER

#r "nuget:FSharp.Data"
open FSharp.Data

[<Literal>]
let SampleJson =
    """{
"Brand": "Ibanex",
"Strings": "7",
"Pickups": ["H", "S", "H"] }"""

type GuitarJson = JsonProvider<const(__SOURCE_DIRECTORY__ + "\sample-guitar.json")>
let sample = GuitarJson.GetSample()
let description = $"{sample.Brand} has {sample.Strings} strings"

//type ManyGuitarsOverHTTP =
//    JsonProvider<"https://raw.githubusercontent.com/isaacabraham/fsharp-in-action/main/sample-guitars.json">

//let inventory = ManyGuitarsOverHTTP.GetSamples()
//printfn $"You have {inventory.Length} guitars in stock"
//let fullInventory = ManyGuitarsOverHttp.Load(__SOURCE_DIRECTORY__ + "/full-inventory.json")

//HTML TYPE PROVIDER
type LondonBoroughs = HtmlProvider<"https://en.wikipedia.org/wiki/List_of_London_boroughs">

let boroughs =
    LondonBoroughs.GetSample().Tables.``List of boroughs and local authorities``

boroughs.Rows |> Array.map (fun row -> row.Borough)


#r "nuget:FSharp.Data"
open FSharp.Data
// CSVPROVIDER
type testCSV = CsvProvider<const(__SOURCE_DIRECTORY__ + "/test.csv")>
let csvSample = testCSV.GetSample()

for r in csvSample.Rows do
    printfn $"{r.Username}"


// DATA VISUALIZATION
#r "nuget: Plotly.NET"
open Plotly.NET

let density =
    boroughs.Rows
    |> Array.map (fun row -> row.Borough, row.``Population (2022 est)`` / row.``Area - sq mi``)

density |> Array.sortBy snd |> Chart.Column |> Chart.show
