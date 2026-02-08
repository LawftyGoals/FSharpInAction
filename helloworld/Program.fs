// For more information see https://aka.ms/fsharp-console-apps
module System.DateTime

let name = "awesome"
let time = DateTime.UtcNow

printfn $"Hello from F#: My name is {name}, the time is {time}"

let greetPerson name age =
    $"Hello, {name}, you are {age} years old"

let text = greetPerson "Ballsy" 20
printfn $"{text}"
