namespace RayTracer

open System

[<Struct>]
type Colour =
    {
        R : float
        G : float
        B : float
    } with

    static member (+) (l : Colour, r : Colour) : Colour =
        {
            R = l.R + r.R
            G = l.G + r.G
            B = l.B + r.B
        }

    static member (*) (l : Colour, r : Colour) : Colour =
        {
            R = l.R * r.R
            G = l.G * r.G
            B = l.B * r.B
        }

    static member (.*) (l : float, r : Colour) : Colour =
        {
            R = l * r.R
            G = l * r.G
            B = l * r.B
        }

[<RequireQualifiedAccess>]
module Colour =

    let private clampToOne (c : Colour) : Colour =
        let maxValue = Math.Max (c.R, Math.Max (c.G, c.B))
        if maxValue > 1. then
            {
                R = c.R / maxValue
                G = c.G / maxValue
                B = c.B / maxValue
            }
        else c

    let toString (c : Colour) =
        let r,g,b =
            let { R = r; G = g; B = b } = clampToOne c
            r * 255., g * 255., b * 255.
        sprintf "%d %d %d" (uint8 r) (uint8 g) (uint8 b)

    let scalarDivide (f : float) (c : Colour) =
        {
            R = (float c.R) / f
            G = (float c.G) / f
            B = (float c.B) / f
        }

    let reduceAndAverage (cs : Colour list) : Colour =
        let rgbStore = 0.,0.,0.
        List.fold
            (fun (r,g,b) c -> r + c.R, g + c.G, b + c.B)
            rgbStore
            cs
        |> (fun (r,g,b) ->
            {
                R = r / (float cs.Length)
                G = g / (float cs.Length)
                B = b / (float cs.Length)
            })