open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Giraffe


module Db =
    let tryFindDeviceStatus (deviceId: int) =
        task {
            if deviceId < 50 then return Some "ACTIVE"
            elif deviceId < 100 then return Some "IDLE"
            else return None
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

let giraffeApp =
    choose
        [ GET >=> route "/" >=> text "homepage"
          GET >=> routef "/devide/status/%i" getDeviceStatusById
          GET >=> route "/device/status" >=> getDeviceStatus
          POST >=> route "device/execute" >=> text "execute a command!"
          POST >=> route "/device/status" >=> getDeviceStatusByJson

          ]



let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore

let app = builder.Build()
app.UseGiraffe giraffeApp


app.Run()
