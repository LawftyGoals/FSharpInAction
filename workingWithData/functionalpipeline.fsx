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

inputs |> Seq.takeWhile isNotExit


Console.Write "Please enter your command: "
Console.ReadLine()
