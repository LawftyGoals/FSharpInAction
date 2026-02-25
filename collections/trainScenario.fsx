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



type Carriage =
    { Id: int
      CarriageType: CarriageType
      Seats: int
      CarriageFeatures: CarriageFeatures list }

let testCarriage =
    { Id = 1
      CarriageType = BuffetFood Hot
      Seats = 45
      CarriageFeatures = [ WIFI; Quiet; Washroom ] }

printfn $"{testCarriage.CarriageType}"
