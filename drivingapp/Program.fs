// For more information see https://aka.ms/fsharp-console-apps
open System
open Car

let remaining = drive 2 8

printfn $"You have {remaining.RemainingGas} gas left."

let inp = Console.ReadLine() |> int

let rem = drive inp 5

if rem.Empty then
    printfn $"you are out of gas"
else
    printfn $"gas left: {rem.RemainingGas}"
