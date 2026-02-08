let name = "awesome"
let time = System.DateTime.UtcNow

printfn $"Hello from F#: My name is {name}, the time is {time}"

let greetPerson name age =
    $"Hello, {name}, you are {age} years old"

let text = greetPerson "Ballsy" 20
printfn $"{text}"

//3.1 page 31
(*Create a function that takes in three numbers as input arguments. Add the first two
together and bind it to a symbol inProgress. Then, multiply that by the third argument and bind it to a symbol answer. Finally, return a string that says The answer is
{answer} using string interpolation.
*)

let myfujnc a b c =
    let inProgress = a + b
    let answer = inProgress * c
    $"The answer is: {answer}"


let add (a: int) (b: int) =
    let answer = a + b
    answer

let shtick = add 1 13
//3.4 Generics

let explicit = ResizeArray<int>()
let typeHole = ResizeArray<_>()
let omitted = ResizeArray()

typeHole.Add "10"
omitted.Add "20"

let combineElements<'T> (a: 'T) (b: 'T) (c: 'T) =
    let output = ResizeArray<'T>()
    output.Add a
    output.Add b
    output.Add c
    output

let strEls = combineElements "s" "T" "v"

let intEls = combineElements 1 2 3

let floatEls = combineElements 1.0 2.0 3.0


// EXERCISE 6.3
