open System
#r "nuget:FSToolkit.ErrorHandling, 2.13.0"
open FsToolkit.ErrorHandling

type PassengerClass =
    | FirstClass
    | SecondClass

type BuffetFood =
    | Hot
    | Cold
    | HotAndCold

type CarriageType =
    | Passenger of PassengerClass
    | BuffetFood of BuffetFood

type CarriageFeatures =
    | WIFI
    | Quiet
    | Washroom

type StopName = StopName of string

type Stop =
    { Name: StopName
      TimeOfArrival: DateTime }

type CarriageNumber = CarriageNumber of int

type Carriage =
    { CarriageNumber: CarriageNumber
      CarriageType: CarriageType
      Seats: int
      CarriageFeatures: CarriageFeatures Set }


type TrainId = TrainId of string

type Train =
    { Id: TrainId
      Origin: Stop
      Destination: Stop
      Stops: Stop list
      Exchange: Stop option
      Carriages: Carriage list }

let testCarriage =
    { CarriageNumber = CarriageNumber 1
      CarriageType = BuffetFood HotAndCold
      Seats = 45
      CarriageFeatures = Set [ WIFI; Quiet; Washroom ] }

let testTrain =
    { Id = TrainId "Train1"
      Origin =
        { Name = StopName "First"
          TimeOfArrival = DateTime.Now }
      Destination =
        { Name = StopName "Last"
          TimeOfArrival = DateTime.Now.AddHours(1) }
      Stops = []
      Exchange = None
      Carriages = [ testCarriage; testCarriage ] }

module TrainMethods =
    let totalNumberOfSeats train =
        (0, train.Carriages)
        ||> List.fold (fun (accu: int) carriage -> accu + carriage.Seats)

    let findStop stops (name: StopName) =
        stops |> List.tryFind (fun stop -> stop.Name = name)

    type TimeBetweenErrors =
        | SameStartAndDestination of StopName
        | NonExistentStop of StopName
        | DestinationEarlierThanOrigin of StopName * StopName

    let timeBetweenStops train start destination =
        result {
            if start = destination then
                return! Error(SameStartAndDestination start)

            let! starting = findStop train.Stops start |> Result.requireSome (NonExistentStop start)

            let! ending =
                findStop train.Stops destination
                |> Result.requireSome (NonExistentStop destination)

            if starting.TimeOfArrival > ending.TimeOfArrival then
                return! Error(DestinationEarlierThanOrigin(destination, start))

            return ending.TimeOfArrival - starting.TimeOfArrival

        }

    let findCarriagesWithFeature feature train =
        train.Carriages
        |> List.filter (fun carriage -> carriage.CarriageFeatures.Contains feature)
