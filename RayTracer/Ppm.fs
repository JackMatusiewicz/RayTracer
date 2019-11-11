namespace RayTracer

open System.Text
open System.IO

[<RequireQualifiedAccess>]
module Image =

    let toFile (fileName : string) (pixels : Colour [,]) : unit =
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
        |> fun t -> File.WriteAllText(fileName + ".ppm", t)
