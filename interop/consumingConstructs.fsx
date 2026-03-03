open System.IO
let fileInfo = FileInfo __SOURCE_FILE__
let directoryInfo = fileInfo.Directory
let files = directoryInfo.GetFiles()

open System.Linq

let scriptFiles =
    files
    |> Seq.where (fun f -> Path.GetExtension f.Name = ".fsx")
    |> Seq.map (fun f -> f.Name)

let scriptFilesLinq =
    files.Where(fun f -> Path.GetExtension f.Name = ".fsx").Select(fun f -> f.Name)
