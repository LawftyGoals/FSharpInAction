//EXERCISE 5.1

let buildPerson (forname: string) (surname: string) (age: int) = forname, surname

let buildPerson2 (forname: string) (surname: string) (_age: int) = forname, surname

let buildPerson3 (forname: string) (surname: string) (age: int) =
    (forname, surname), (age, if age < 18 then "child" else "adult")

let _, ages = buildPerson3 "ted" "talks" 15

let _, desc = ages

printfn $"{desc}"

let buildPerson4 (name: struct (string * string)) =
    let struct (fname, sname) = name
    struct ("Dr.", fname, sname)


let buildPerson5 (name: string * string) (_age: int) =
    let fname, sname = name
    fname, sname

let testar, testor = buildPerson5 ("ji", "jo") 120

printfn $"{testar} {testor}"


// RECORDS

type Address =
    { Line1: string
      Town: string
      Country: string }

type Person =
    { FirstName: string
      LastName: string
      Age: int
      Address: Address }

let isaac =
    { FirstName = "isaac"
      LastName = "abraham"
      Age = 42
      Address =
        { Line1 = "the road 1"
          Town = "London"
          Country = "England" }

    }

printfn $"{isaac.FirstName} {isaac.LastName} {isaac.Age}"

// copy old values apply new lastname
let isaac2 = { isaac with LastName = "bruiser" }

//mutable record field
type mutetabetable = { mutable Test: string }

let issac = { Test = "bubbly bubbly" }
issac.Test <- "not bubbly"

printfn $"{issac.Test}"

// EQUALITY CHECK

let wowza = { Test = "busta" }
let lolza = { Test = "busta" }

let samezies = (wowza = lolza)

// SPOILER ALERT THEY ARE THE SAME
if samezies then
    printfn "wowz and lolza are the same"
else
    printfn "not the same"

// STACK?
[<Struct>]
type stackableRecord = { Stackable: string }
// WILL NOW BE DEFINED ON THE STACK NOT THE HEAP: means it will be passed as a value type
// not a reference type
let stackable = { Stackable = "word" }


// EXERCISE 5.3
type Name = { surname: string; firstname: string }

type customer =
    { name: Name
      address: Address
      creditRating: int }

type supplyer =
    { name: Name
      address: Address
      balance: float
      duedate: System.DateTime }
// ANONYMOUS RECORDS - dont need a definition "type"

let anon = {| Name = "an anonymous record" |}

let anon2 =
    {| anon with
        Name = "still an anonymous record"
        Type = "updating and adding fields" |}

//CURRYING
let add (fn: int) (sn: int) = fn + sn

let addFive = add 5
let addTen = addFive 10

printfn $"{addTen}"

//EXERCISE 6.2
//1
let add2 fn sn = fn + sn
let addTen2 = add2 10

let addMore = addTen2 3

printfn $"{addMore}"

//2
let multiply fn sn = fn * sn
//3
let addTenAndDouble nn = multiply (add2 10 nn) 2

let lol = addTenAndDouble 5
printfn $"{lol}"

//4
let timesTwo = multiply 2

let addTenAndDouble2 nn = timesTwo (addTen2 nn)

printfn $"{addTenAndDouble2 6}"

//Exercise 6.2 Pipelines
let gas = 100.0

let drive distance gas =
    if distance > 50 then gas / 2.0
    elif distance > 25 then gas - 10.0
    elif distance > 0 then gas - 1.0
    else gas

let finalState = gas |> drive 55 |> drive 26 |> drive 1

printfn $"{finalState}"
