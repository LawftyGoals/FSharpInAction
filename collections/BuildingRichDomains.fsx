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


//OPTION.MAP

type Customer = { Name: string; Age: int }



let getAge customer =
    match customer with
    | Some c -> Some c.Age
    | None -> None

let getAgeMap customer = customer |> Option.map (fun c -> c.Age)

let classifyAge age =
    age
    |> Option.map (fun a ->
        match a with
        | a when a < 18 -> "Child"
        | _ -> "Adult")
(*    match age with
    | age when age < 18 -> "Child"
    | age when age < 64 -> "Adult"
    | _ -> "Senior"*)

let optionalClassification optionalCustomer =
    optionalCustomer |> Option.map getAgeMap |> Option.map classifyAge

//Compose operator

let operatorClassification optionalCustomer =
    optionalCustomer |> Option.map (getAge >> classifyAge)

open System.IO

let tryGetFileContents fileName =
    if File.Exists fileName then
        Some(File.ReadAllText fileName)
    else
        printfn "aint no file here boss"
        None

let countWords (text: string) = text.Split " " |> Array.length

let countWordsInFile fileName =
    fileName |> tryGetFileContents |> Option.map countWords


let targetFile = Path.Combine(__SOURCE_DIRECTORY__, "boss.txt")
countWordsInFile targetFile




//vibetest

let getTextFromFile fileName =
    if File.Exists fileName then
        Some(File.ReadAllText fileName)
    else
        None

let wordCount (text: string) = text.Split " " |> Array.length

let getWordCountFromFile fileName =
    fileName |> getTextFromFile |> Option.map wordCount


getWordCountFromFile "boss.txt"


//OPTION BIND
type OptionalCustomer = { Name: string; Age: int option }
let bob = Some { Name = "Name"; Age = Some 42 }

let getAgeS b = b.Age


let ageToGroup: int option = bob |> Option.bind getAgeS

match ageToGroup with
| Some age -> age
| None -> 0


// OPTIONS AND COLLECTIONS

let col = [ 1; 2; 3 ]

let found = col |> List.find (fun t -> t < 2)
let otherFound = List.find (fun t -> t > 1) col

let tryFound = List.tryFind (fun t -> t > 2) col
let tryNotFound = List.tryFind (fun t -> t > 3) col

let extractFound =
    match tryFound with
    | Some nr -> nr
    | None -> 0

printfn $"{found} {otherFound} {extractFound} {tryNotFound}"

//List.pick
let pic = [ 1, 2, 3, 4 ]

let resultPick =
    List.pick
        (fun l ->
            match l with
            | n -> Some n
            | _ -> None)
        pic


//RESULTS
type RawCustomer =
    { CustomerId: string
      Name: string
      Street: string
      City: string
      Country: string
      AccountBalance: decimal }

type CustomerId = CustomerId of int
type Name = Name of string
type Street = Street of string
type City = City of string

type Country =
    | Domestic
    | Foreign of string

type AccountBalance = AccountBalance of decimal

type CookedCustomer =
    { Id: CustomerId
      Name: Name
      Address:
          {| Street: Street
             City: City
             Country: Country |}
      Balance: AccountBalance }


let validateCustomer (rawCustomer: RawCustomer) =
    let customerId =
        if rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error $"Invalid Customer Id '{rawCustomer.CustomerId}"

    let country =
        match rawCustomer.Country with
        | "" -> Error "No country supplied"
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    match customerId, country with
    | Ok customerId, Ok country ->
        Ok
            { Id = customerId
              Name = Name rawCustomer.Name
              Address =
                {| Street = Street rawCustomer.Street
                   City = City rawCustomer.City
                   Country = country |}
              Balance = AccountBalance rawCustomer.AccountBalance }
    | Error err, _
    | _, Error err -> Error err


// MULTIPLE ERROR RETURN
let multiResultValidateCustomer rawCustomer =
    let customerId =
        if rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error $"Invalid customer ID: {rawCustomer.CustomerId}"

    let country =
        match rawCustomer.Country with
        | "" -> Error "No country selected"
        | "Norway" -> Ok Domestic
        | other -> Ok(Foreign other)

    match customerId, country with
    | Ok customerId, Ok country ->
        Ok
            { Id = customerId
              Name = Name rawCustomer.Name
              Address =
                {| Street = Street rawCustomer.Street
                   City = City rawCustomer.City
                   Country = country |}
              Balance = AccountBalance rawCustomer.AccountBalance }

    | customerId, country ->
        Error
            [ match customerId with
              | Ok _ -> ()
              | Error x -> x
              match country with
              | Ok _ -> ()
              | Error x -> x ]


type CustomerValidationError =
    | InvalidCustomerId of string
    | InvalidName of string
    | InvalidCountry of string


let validateCustomerId (cId: string) =
    if cId.StartsWith "C" then
        Ok(CustomerId(int cId[1..]))
    else
        Error(InvalidCustomerId $"Invalid Customer Id: {cId}.")

let validateCustomerCountry (country: string) =
    match country with
    | "" -> Error(InvalidCountry "No country selected")
    | "Norway" -> Ok Domestic
    | other -> Ok(Foreign other)

let stronglyErroredValidateCustomer rawCustomer =
    let customerId = validateCustomerId rawCustomer.CustomerId
    let country = validateCustomerCountry rawCustomer.Country

    match customerId, country with
    | Ok customerId, Ok country ->
        Ok
            { Id = customerId
              Name = Name rawCustomer.Name
              Address =
                {| Street = Street rawCustomer.Street
                   City = City rawCustomer.City
                   Country = country |}
              Balance = AccountBalance rawCustomer.AccountBalance }
    | customerId, country ->
        Error
            [ match customerId with
              | Ok _ -> ()
              | Error x -> x
              match country with
              | Ok _ -> ()
              | Error x -> x ]

// HIERARCHY OF ERRORS
open System

module ValidationNested =
    type CustomerIdError =
        | EmptyId
        | InvalidIdFormat of string

    type NameError =
        | NoNameSupplied
        | TooManyParts

    type CountryError = | NoCountrySupplied

    type CustomerValidationError =
        | CustomerIdError of CustomerIdError
        | NameError of NameError
        | CountryError of CountryError

    let validateCustomerIdNested customerId =
        if String.IsNullOrWhiteSpace customerId then
            Error EmptyId
        elif customerId.StartsWith "C" then
            Ok(CustomerId(int customerId[1..]))
        else
            Error(InvalidIdFormat customerId)

    let validateCountry country =
        match country with
        | "" -> Error NoCountrySupplied
        | "Norway" -> Ok Domestic
        | other -> Ok(Foreign other)

    let validateName name =
        if String.IsNullOrWhiteSpace name then
            Error NoNameSupplied
        elif name.Split " " |> Array.length > 2 then
            Error TooManyParts
        else
            Ok(Name name)

type CustomerValidationErrorRoot =
    | CustomerIdError of ValidationNested.CustomerIdError
    | NameError of ValidationNested.NameError
    | CountryError of ValidationNested.CountryError

let validateCustomerNestedRaw rawCustomer =
    let customerId = ValidationNested.validateCustomerIdNested rawCustomer.CustomerId
    let country = ValidationNested.validateCountry rawCustomer.Country
    let name = ValidationNested.validateName rawCustomer.Name

    match customerId, country, name with
    | Ok customerId, Ok country, Ok name ->
        Ok
            { Id = customerId
              Name = name
              Address =
                {| Street = Street rawCustomer.Street
                   City = City rawCustomer.City
                   Country = country |}
              Balance = AccountBalance rawCustomer.AccountBalance }
    | customerId, country, name ->
        Error
            [ match customerId with
              | Ok _ -> ()
              | Error error -> CustomerIdError error
              match country with
              | Ok _ -> ()
              | Error error -> CountryError error
              match name with
              | Ok _ -> ()
              | Error err -> NameError err ]

let customerB =
    { CustomerId = "123" // Bad customerId
      Name = "    " // only whitespace name
      Street = "123 Hoved gate"
      City = "Anyvik"
      Country = "" //No country supplied
      AccountBalance = 123.45m }

let validatedC2Nested = validateCustomerNestedRaw customerB

// EXCEPTIONS
module Exceptions =
    try
        Some(1 / 0)
    with ex ->
        printfn $"Error: {ex.Message}"
        None

    let handleException func arg =
        try
            func arg |> Ok
        with ex ->
            Error ex

    let divide (a, b) = a / b
    let divideSafe = handleException divide
    let result = divideSafe (2, 0)


// COMPUTATIONAL EXPRESSIONS
open System

#r "nuget:FSToolkit.ErrorHandling, 2.13.0"
open FsToolkit.ErrorHandling

module ComputationalExpressions =
    let calc (a: int) (b: int) (c: int) : int = a + b * c

    let tryParseNumber (numberAsString: string) =
        match Int32.TryParse numberAsString with
        | true, number -> Some number
        | false, _ -> None

    let calcRest: int option =
        match tryParseNumber "1", tryParseNumber "2", tryParseNumber "3" with
        | Some firstNumber, Some secondNumber, Some thirdNumber -> Some(calc firstNumber secondNumber thirdNumber)
        | _ -> None

    let myMaybeData =
        option {
            let! numberOne = tryParseNumber "1"
            let! numberTwo = tryParseNumber "2"
            let! numberThree = tryParseNumber "3"
            return calc numberOne numberTwo numberThree
        }

//EXERCISE 9.2
module Validation =
    type CustomerValidationErrorRich =
        | EmptyCustomerId
        | InvalidCustomerIdFormat of string
        | NoNameSupplied
        | TooManyNameParts
        | NoCountrySupplied

    let validateCustomerId customerId =
        if String.IsNullOrWhiteSpace customerId then
            Error EmptyCustomerId
        elif customerId.StartsWith "C" then
            Ok(CustomerId(int customerId[1..]))
        else
            Error(InvalidCustomerIdFormat customerId)

    let validateCountry country =
        match country with
        | "" -> Error NoCountrySupplied
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    let validateName name =
        if String.IsNullOrWhiteSpace name then
            Error NoNameSupplied
        elif name.Split ' ' |> Array.length > 2 then
            Error TooManyNameParts
        else
            Ok(Name name)

let validateCustomerFull (rawCustomer: RawCustomer) =
    let customerId =
        if String.IsNullOrWhiteSpace rawCustomer.CustomerId then
            Error Validation.EmptyCustomerId
        elif rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error(Validation.InvalidCustomerIdFormat rawCustomer.CustomerId)

    let country =
        match rawCustomer.Country with
        | "" -> Error Validation.NoCountrySupplied
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    let name =
        if String.IsNullOrWhiteSpace rawCustomer.Name then
            Error Validation.NoNameSupplied
        elif rawCustomer.Name.Split ' ' |> Array.length > 2 then
            Error Validation.TooManyNameParts
        else
            Ok(Name rawCustomer.Name)

    match customerId, country, name with
    | Ok customerId, Ok country, Ok name ->
        Ok
            { Id = customerId
              Name = name
              Address =
                {| Street = Street rawCustomer.Street
                   City = City rawCustomer.City
                   Country = country |}
              Balance = AccountBalance rawCustomer.AccountBalance }
    | customerId, country, name ->
        Error
            [ match customerId with
              | Ok _ -> ()
              | Error err -> err
              match country with
              | Ok _ -> ()
              | Error err -> err
              match name with
              | Ok _ -> ()
              | Error err -> err ]

let validateCustomerCE rawCustomer =
    validation {
        let! customerId = Validation.validateCustomerId rawCustomer.CustomerId

        and! country = Validation.validateCountry rawCustomer.Country
        and! name = Validation.validateName rawCustomer.Name

        return
            { Id = customerId
              Name = name
              Address =
                {| Street = Street rawCustomer.Street
                   City = City rawCustomer.City
                   Country = country |}
              Balance = AccountBalance rawCustomer.AccountBalance }
    }
