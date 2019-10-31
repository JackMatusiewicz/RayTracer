namespace RayTracer

open System.Text
open System.IO

[<Struct>]
type Colour =
    {
        R : uint8
        G : uint8
        B : uint8
    }

[<RequireQualifiedAccess>]
module Colour =

    let toString ({R = r; G = g; B = b} : Colour) =
        sprintf "%d %d %d" r g b

    let scalarMultiply (f : float) (c : Colour) =
        {
            R = (float c.R) * f |> uint8
            G = (float c.G) * f |> uint8
            B = (float c.B) * f |> uint8
        }

    let reduceAndAverage (cs : Colour list) : Colour =
        let rgbStore = 0,0,0
        List.fold
            (fun (r,g,b) c -> r + (int c.R), g + (int c.G), b + (int c.B))
            rgbStore
            cs
        |> (fun (r,g,b) ->
            {
                R = uint8 (r / cs.Length)
                G = uint8 (g / cs.Length)
                B = uint8 (b / cs.Length)
            })

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
        File.WriteAllText (fileName, data)
    
