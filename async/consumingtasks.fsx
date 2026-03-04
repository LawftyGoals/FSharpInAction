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
