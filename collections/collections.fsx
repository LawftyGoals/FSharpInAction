// EXERCISE 7.1 HIGHER ORDER FUNCTIONS HOF

let calculate calculation calcName firstNumber lastNumber =
    let ans = calculation firstNumber lastNumber
    printfn $"{firstNumber} {calcName} {lastNumber} = {ans}"
    ans

let add fn sn = fn + sn
let subtract fn sn = fn - sn

calculate add "add" 1 2

// LIST NOT C# LIST (WHICH IS A RESIZEARRAY IN F#)
let a = [ 1; 2; 3 ]
let b = [ 4; 5; 6 ]

let c = a @ b //combines list
let d = 0 :: a //appends to front of a
printfn $"{c} {d}"


// DEBUGGING

type Result =
    { HomeTea: string
      HomeGoals: int
      AwayTea: string
      AwayGoals: int }

let create home hg away ag =
    { HomeTea = home
      HomeGoals = hg
      AwayTea = away
      AwayGoals = ag }

let results =
    [ create "Messiville" 2 "Ronaldo City" 1
      create "Messiville" 3 "Bale Town" 1
      create "Ronaldo City" 2 "Bale Town" 3
      create "Bale Town" 2 "Messiville" 5
      create "Ronaldo City" 0 "Messiville" 10

      ]

let isAwayWin result = result.AwayGoals > result.HomeGoals

let wonAwayTheMost =
    results
    |> List.filter isAwayWin
    |> List.countBy (fun result -> result.AwayTea)
    |> List.maxBy (fun (_team, count) -> count)


// EXERCISE 7.2
//attempt
let ronaldoCount =
    let filtered =
        results
        |> List.filter (fun result -> result.HomeTea = "Ronaldo City" || result.AwayTea = "Ronaldo City")

    filtered.Length
//answer
let ronaldoCountAns =
    results
    |> List.filter (fun result -> result.HomeTea = "Ronaldo City" || result.AwayTea = "Ronaldo City")
    |> List.length


// LIST FUNCTIONS
//ALT + , IS INFO PANEL VERY HELPFULL
[ 1; 2; 3; 4 ] |> List.map (fun x -> x.ToString())

//EXERCISE 7.3
//attempt
let mostgoals =
    results
    |> List.collect (fun result -> [ result.AwayTea, result.AwayGoals; result.HomeTea, result.HomeGoals ])
    |> List.groupBy (fun (name, _goals) -> name)
    |> List.map (fun (name, results) -> name, results |> List.sumBy (fun (_name, goals) -> goals))
    |> List.maxBy snd

//answer
let mostgoalsAns =
    results
    |> List.collect (fun result ->
        [ {| Tea = result.HomeTea
             Goals = result.HomeGoals |}
          {| Tea = result.AwayTea
             Goals = result.AwayGoals |} ])
    |> List.groupBy (fun result -> result.Tea)
    |> List.map (fun (team, games) -> team, games |> List.sumBy (fun game -> game.Goals))
    |> List.maxBy snd

// RANGES SLICES AND INDEXING
let numbers = [ 1..10 ]
let secondItem = numbers[1]
let oneToSix = numbers[1..6]

let sqrs = [ for i in 1..10 -> i * i ]

let test = [ 0..2..9 ]


// .NET ARRAY
let numArray = [| 1..10 |]
let secondArrIt = numArray[1]
numArray[1] <- 4
printfn $"{secondArrIt} {numArray[1]}"

// .NET SEQUENCE A IENUMERABLE WRAPPER
let numSeq = seq { 1..10 }
let secondEl = numSeq |> Seq.item 1
// let worksOr = numSeq.item 1 does not work must be piped?
let mappedSeq = numSeq |> Seq.map (fun num -> num * num)

let comp =
    seq {
        1
        2

        if System.DateTime.Today.DayOfWeek = System.DayOfWeek.Tuesday then
            99

        4
    }

// MAP F# version of dictionary
let lookupMap = Map [ (1, 2); (3, 4) ]
let newLookupMap = lookupMap.Add(5, 6)
let newLookupMap2 = lookupMap |> Map.add (7, 8)

//SET
type setType = { Name: string }

let salesEmployees =
    Set[{ Name = "Boss" }
        { Name = "Babs" }]

let bonusEmployees = Set [ { Name = "Babs" }; { Name = "Tanya" } ]

let allBonusesForSalesStaff = salesEmployees |> Set.isSubset bonusEmployees
let salesWithoutBonuses = salesEmployees - bonusEmployees
let allEmployees = salesEmployees + bonusEmployees

let listOfNumbers = [ 1; 1; 2; 3; 4; 5; 5 ]
let numSet = Set.ofList listOfNumbers
let numArray2 = Set.toArray numSet
