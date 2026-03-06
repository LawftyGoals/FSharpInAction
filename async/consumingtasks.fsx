open System.IO
open System.Threading.Tasks

File.WriteAllText("foo.txt", "Hello, world!")
let text: string = File.ReadAllText "foo.txt"
let textAsync: Task<string> = File.ReadAllTextAsync "foo.txt"

let theText = textAsync.Result

//Exercise 12.1
let text2 = File.ReadAllText "bar.txt"
(*
System.IO.FileNotFoundException: Could not find file 'C:\Users\mymsn\Documents\FSharp\bar.txt'.
File name: 'C:\Users\mymsn\Documents\FSharp\bar.txt'
   at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.Strategies.OSFileStreamStrategy..ctor(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.Strategies.FileStreamHelpers.ChooseStrategyCore(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.StreamReader.ValidateArgsAndOpenPath(String path, Int32 bufferSize)
   at System.IO.File.ReadAllText(String path, Encoding encoding)
   at <StartupCode$FSI_0011>.$FSI_0011.main@() in c:\Users\mymsn\Documents\FSharp\InAction\async\consumingtasks.fsx:line 11
   at System.RuntimeMethodHandle.InvokeMethod(ObjectHandleOnStack target, Void** arguments, ObjectHandleOnStack sig, BOOL isConstructor, ObjectHandleOnStack result)
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
Stopped due to error
*)
let text2Async = File.ReadAllTextAsync "bar.txt"
(*  System.Threading.Tasks.Task`1[System.String]
    {AsyncState = null;
     CreationOptions = None;
     Exception = System.AggregateException: One or more errors occurred. (Could not find file 'C:\Users\mymsn\Documents\FSharp\bar.txt'.)
 ---> System.IO.FileNotFoundException: Could not find file 'C:\Users\mymsn\Documents\FSharp\bar.txt'.
File name: 'C:\Users\mymsn\Documents\FSharp\bar.txt'
   at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.Strategies.OSFileStreamStrategy..ctor(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.Strategies.FileStreamHelpers.ChooseStrategyCore(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share, Int32 bufferSize, FileOptions options, Int64 preallocationSize)
   at System.IO.File.AsyncStreamReader(String path, Encoding encoding)
   at System.IO.File.InternalReadAllTextAsync(String path, Encoding encoding, CancellationToken cancellationToken)
   --- End of inner exception stack trace ---;
     Id = 2;
     IsCanceled = false;
     IsCompleted = true;
     IsCompletedSuccessfully = false;
     IsFaulted = true;
     Status = Faulted;}
*)

//EXERCISE 12.2
let delaytext = Task.Delay(10000).ContinueWith(fun _ -> 100)

delaytext.Wait() // makes asynchronous synchronous again!


//TASK BLOCK - F# ASYNC AWAIT
let writeToFile fileName (data: string) =
    System.IO.File.AppendAllText(fileName, data)
    let data = System.IO.File.ReadAllText fileName
    data.Length

let total = writeToFile "sample.txt" "foo"

let writeToFileAsync fileName (data: string) =
    task {
        do! System.IO.File.AppendAllTextAsync(fileName, data)
        let! data = System.IO.File.ReadAllTextAsync fileName
        return data.Length
    }

// EXECUTE MULTIPLE TASKS
open System

let multiTask =
    task {
        let! allFiles =
            [ "file1.txt"; "file2.txt"; "file3.txt" ]
            |> List.map File.ReadAllTextAsync
            |> Task.WhenAll

        return allFiles |> Array.reduce (sprintf "%s %s")
    }


// EMBEDDING A TASK BLOCK WITH LARGER EXPRESSION

let writeToFileAsyncMix fileName (data: string) =
    printfn "1. This is happening synchronously!"
    Task.Delay(1000).Wait()
    printfn "2. Kicking off the background work!"

    let result =
        task {
            do! System.IO.File.AppendAllTextAsync(fileName, data)
            do! Task.Delay(1000)
            printfn "4. This is happening asynchronously!"
            let! data = System.IO.File.ReadAllTextAsync fileName
            return data.Length
        }

    printfn "3. Doing something more now let's run this task"
    result

writeToFileAsyncMix "bob.txt" "yabadabadooo"


//ASYNC BLOCK
let writeToFileAsyncAsync fileName (data: string) =
    async {
        do! System.IO.File.AppendAllTextAsync(fileName, data) |> Async.AwaitTask
        let! data = System.IO.File.ReadAllTextAsync(fileName) |> Async.AwaitTask
        return data.Length
    }


// Standard synchronous call chain
open System.Text.Json

let loadCustomerFromDb customerId = {| Name = "Isaac"; Balance = 0 |}

let tryGetCustomer customerId =
    let customer = loadCustomerFromDb customerId

    if customer.Balance <= 0 then
        Error "Customer is in debt!"
    else
        Ok customer

let handleRequest (json: string) =
    let request: {| CustomerId: int |} = JsonSerializer.Deserialize json
    let response = tryGetCustomer request.CustomerId

    match response with
    | Ok c -> {| CustomerName = c.Name.ToUpper() |}
    | Error msg -> failwith $"Bad request: {msg}"

//BECOMES
let loadCustomerFromDbTask customerId =
    task { return {| Name = "Isaac"; Balance = 0 |} }

let tryGetCustomerTask customerId =
    task {
        let! customer = loadCustomerFromDbTask customerId

        return
            if customer.Balance <= 0 then
                Error "Customer is in debt!"
            else
                Ok customer
    }

let handleRequestTask (json: string) =
    task {
        let request: {| CustomerId: int |} = JsonSerializer.Deserialize json
        let! response = tryGetCustomerTask request.CustomerId

        return
            match response with
            | Ok c -> {| CustomerName = c.Name.ToUpper() |}
            | Error msg -> failwith $"Bad request: {msg}"
    }
