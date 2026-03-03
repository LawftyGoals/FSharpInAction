// INTERFACES

open System

type MyDisposableType() =
    interface IDisposable with
        member _.Dispose() = printfn "Disposing"


type MyInterface =
    abstract Capitalize: string -> string
    abstract Add: int -> int


type MyImplementation() =
    interface MyInterface with
        member this.Capitalize text = text.ToUpper()
        member this.Add number = number + 1

// wont work -> let implementation  = MyImplementation ()
//Important to define as MyInterface
let implementation: MyInterface = MyImplementation()
implementation.Capitalize "test"

type MyInterfaceAsRecord =
    { Capitalize: string -> string
      Add: int -> int }

//OBJECT EXPRESSIONS
let implementation2 =
    { new MyInterface with
        member this.Capitalize text = text.ToUpper()
        member _.Add number = number + 1 } // _ can also be used as is not used in method body

let text = implementation2.Add 3

//INTERFACES ON F#TYPES

type Person =
    { Name: string
      Age: int }

    interface System.ICloneable with
        member this.Clone() = { Name = this.Name; Age = this.Age }


let SetupWebApp () =
    let gl = [ 1; 2; 3 ]
    gl |> List.map (fun i -> i.ToString()) |> ignore // prevents a return value.

// TUPLES
let y = 1, 2 // regular reference tuple
let x = struct (1, 2) // struct tupple "value based"


// TRYPARSE
let optionParse parser (value: string) =
    match parser value with
    | true, v -> Some v
    | false, _ -> None

let parseIntOption = optionParse System.Int32.TryParse

let maybeANumber = parseIntOption "123"

// SINGLE METHOD INTERFACES
open System

type IDisplayTime =
    abstract Display: DateTime -> string

let makeIDisplayTime implementation =
    { new IDisplayTime with
        member _.Display date = implementation date }


let normalPrinter = makeIDisplayTime (fun date -> $"The tiem is now {date}!")

let shortPrinter =
    makeIDisplayTime (fun date -> $"It's {date.ToShortTimeString()}.")


// PARTIAL APLICATION AND PIPELINES
open System.IO

module File =
    let append (path: string) (text: string) =
        File.AppendAllText(path, text)
        path

File.WriteAllText("text.txt", "test")

let fileInfo =
    "text.txt"
    |> File.ReadAllText
    |> fun text -> text.ToUpper()
    |> File.append "otherfile.txt"
    |> FileInfo
