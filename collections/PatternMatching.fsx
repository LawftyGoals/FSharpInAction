// EXERCISE 8.1
type CD = { YoH: int; HOD: bool; OD: float }

let testCustomer: CD = { YoH = 2; HOD = true; OD = 410.0 }
let baseOD = 250.0

let canTakeOutLoan =
    match testCustomer with
    | { YoH = 0 } -> false
    | { YoH = 1; HOD = true } when testCustomer.OD > baseOD -> false
    | { YoH = 1 } -> true
    | { HOD = true } when testCustomer.OD > baseOD * float testCustomer.YoH -> false
    | _ -> true

printfn $"{canTakeOutLoan}"



//RECURSIVE MATCHING

type OverdraftDetails =
    { Approved: bool
      MaxAmount: decimal
      CurrentAmount: decimal }

type Address = { Country: string }

type CustomerWithOverdraft =
    { YearsOfHistory: int
      Overdraft: OverdraftDetails
      Address: Address }


let canTakeOutALoanRecursive customer =
    match customer with
    | { YearsOfHistory = 0 } ->
        match customer.Overdraft with
        | { Approved = true } ->
            match customer.Address with
            | { Country = "NO" } -> true
            | _ -> false
        | { Approved = false } -> false
    | { YearsOfHistory = 1
        Address = { Country = "NO" } } ->
        match customer.Overdraft with
        | { Approved = true } -> true
        | { Approved = false } -> false
    | { YearsOfHistory = 1; Address = _ } -> false
    | _ -> true

let csmr =
    { YearsOfHistory = 1
      Overdraft =
        { Approved = true
          MaxAmount = 1000m
          CurrentAmount = 200m }

      Address = { Country = "US" } }

printfn $"{canTakeOutALoanRecursive csmr}"

// NESTING OR
let canTakeOrLoan customer =
    match customer with
    | { YearsOfHistory = 0 | 1
        Overdraft = { Approved = true } } -> true
    | { YearsOfHistory = 0 | 1 } -> false
    | _ -> true


// BINDING SYMBOLS
let bindingSymbols customer =
    match customer with
    | { YearsOfHistory = 0 | 1
        Overdraft = { Approved = true
                      CurrentAmount = amount } } ->
        printfn $"Loan Approved; current overdraft is {amount}"
        true
    | { YearsOfHistory = 0 | 1
        Overdraft = { Approved = false } as overdraftDetails } ->
        printfn $"Loan declined; overdraft details are {overdraftDetails}"
        true
    | _ -> false


printfn $"{bindingSymbols csmr}"
