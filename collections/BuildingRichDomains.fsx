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
