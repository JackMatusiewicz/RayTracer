namespace RayTracer

open System.Text

type Colour =
    {
        R : uint8
        G : uint8
        B : uint8
    }

type Vec =
    {
        X : int
        Y : int
        Z : int
    }

[<RequireQualifiedAccess>]
module Colour =

    let toString ({R = r; G = g; B = b} : Colour) =
        sprintf "%d %d %d" r g b

type PpmFile = internal Ppm of string

[<RequireQualifiedAccess>]
module Ppm =

    let toPpm (pixels : Colour [,]) : PpmFile =
        let rows = pixels.GetLength 0
        let cols = pixels.GetLength 1
        let mutable sb = StringBuilder ()
        sb <- sb.Append (sprintf "%d %d\n" rows cols)
        for i in 0 .. rows - 1 do
            let row =
                pixels.[i, *]
                |> Array.map Colour.toString
                |> Array.fold (fun s a -> sprintf "%s %s" a s) "\n"
            sb <- sb.Append row
            ()
        sb.ToString ()
        |> Ppm
    
