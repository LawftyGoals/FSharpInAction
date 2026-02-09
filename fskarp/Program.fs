// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"


// EXERCISE 5.1
let buildPerson (forname: string) (surname: string) (age: int) =
    forname, surname

let fname, sname = buildPerson "me" "my" 10
printf $"{fname} {sname}"
