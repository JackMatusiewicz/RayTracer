namespace RayTracer

open System.Text
open System.IO

type PpmFile = internal Ppm of string

[<RequireQualifiedAccess>]
module Ppm =

    let toPpm (pixels : Colour [,]) : PpmFile =
        let rows = pixels.GetLength 0
        let cols = pixels.GetLength 1
        let mutable sb = StringBuilder ()
        sb<- sb.AppendLine ("P3")
        sb <- sb.Append (sprintf "%d %d\n" cols rows)
        sb <- sb.AppendLine ("255")
        for i in 0 .. rows - 1 do
            let row =
                pixels.[i, *]
                |> Array.map Colour.toString
            for c in row do
                sb <- sb.Append (sprintf "%s\n" c)
        sb.ToString ()
        |> Ppm

    let toDisk (fileName : string) (Ppm data) : unit =
        File.WriteAllText (fileName + ".ppm", data)
    
