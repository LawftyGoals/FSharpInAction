open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.ViewEngine
open FsToolkit.ErrorHandling



module Db =
    let tryFindDeviceStatus (deviceId: int) =
        task {
            if deviceId < 50 then return Some "ACTIVE"
            elif deviceId < 100 then return Some "IDLE"
            else return None
        }


type DeviceStatusError =
    | NoDeviceIdSupplied
    | InvalidDeviceId of string
    | NoSuchDeviceId of int

    member this.Description =
        match this with
        | NoDeviceIdSupplied -> "No Device Id was provided"
        | InvalidDeviceId text -> $"'{text} is not valid Device Id"
        | NoSuchDeviceId deviceId -> $"Device Id {deviceId} does not exist"

type DeviceStatusResponse = { DeviceId: int; DeviceStatus: string }

let tryGetDeviceStatus maybeDeviceId =
    taskResult {
        let! (rawDeviceId: string) = maybeDeviceId |> Result.requireSome NoDeviceIdSupplied
        let! deviceId = Option.tryParse rawDeviceId |> Result.requireSome (InvalidDeviceId rawDeviceId)
        let! deviceStatus = Db.tryFindDeviceStatus deviceId
        let! deviceStatus = deviceStatus |> Result.requireSome (NoSuchDeviceId deviceId)

        return
            { DeviceId = deviceId
              DeviceStatus = deviceStatus }
    }

let warehouseApi next (ctx: HttpContext) =
    task {
        let maybeDeviceId = ctx.TryGetQueryStringValue "deviceId"
        let! deviceStatus = tryGetDeviceStatus maybeDeviceId

        match deviceStatus with
        | Error errorCode -> return! RequestErrors.BAD_REQUEST errorCode.Description next ctx
        | Ok deviceInfo -> return! json deviceInfo next ctx

    }

let getDeviceStatus next (ctx: HttpContext) =
    task {
        let deviceId =
            ctx.TryGetQueryStringValue "deviceId"
            |> Option.defaultWith (fun _ -> failwith "Missing device id")
            |> int

        let! deviceStatus = Db.tryFindDeviceStatus deviceId

        match deviceStatus with
        | None -> return! RequestErrors.NOT_FOUND "No device id found" next ctx
        | Some status ->
            return!
                json
                    {| DeviceId = deviceId
                       Status = status |}
                    next
                    ctx
    }

let getDeviceStatusById (deviceId: int) next (ctx: HttpContext) =
    task {
        let! deviceStatus = Db.tryFindDeviceStatus deviceId

        match deviceStatus with
        | None -> return! RequestErrors.NOT_FOUND "No device id found" next ctx
        | Some status ->
            return!
                json
                    {| DeviceId = deviceId
                       Status = status |}
                    next
                    ctx
    }


let getDeviceStatusByJson next (ctx: HttpContext) =
    task {
        let! request = ctx.BindModelAsync<{| DeviceId: int |}>()
        printf $"{request.DeviceId}"
        let! deviceStatus = Db.tryFindDeviceStatus request.DeviceId

        match deviceStatus with
        | None -> return! RequestErrors.NOT_FOUND "No device id found" next ctx
        | Some status ->
            return!
                json
                    {| DeviceId = request.DeviceId
                       Status = status |}
                    next
                    ctx
    }

let simpleView next (ctx: HttpContext) =
    task {
        let maybeDeviceId = ctx.TryGetQueryStringValue "deviceId"
        let! deviceStatus = tryGetDeviceStatus maybeDeviceId

        let view =
            html
                []
                [ head
                      []
                      [ style [] [ str "h1 {background-color:rebeccapurple}" ]

                        link [ _rel "stylesheet"; _href "/css/style.css" ] ]
                  body
                      []
                      [ h1 [] [ str "Device report" ]
                        match deviceStatus with
                        | Error errorCode -> h2 [] [ str $"Error: {errorCode.Description}" ]
                        | Ok deviceInfo ->
                            p
                                [ _class "is-success" ]
                                [ str $"Succcess: {deviceInfo.DeviceId} has status {deviceInfo.DeviceStatus}" ] ] ]

        return! htmlView view next ctx
    }

let giraffeApp =
    choose
        [ GET >=> route "/" >=> text "homepage"
          GET >=> routef "/devide/status/%i" getDeviceStatusById
          GET >=> route "/device/status" >=> simpleView //warehouseApi
          POST >=> route "device/execute" >=> text "execute a command!"
          POST >=> route "/device/status" >=> getDeviceStatusByJson

          ]






let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore

let app = builder.Build()
app.UseStaticFiles() |> ignore
app.UseGiraffe giraffeApp


app.Run()
