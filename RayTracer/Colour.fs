namespace RayTracer

open System
open System.Drawing

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

    let toColor (c : Colour) : Color =
        let { R = r; G = g; B = b } = clampToOne c
        (r * 255., g * 255., b * 255.)
        |> (fun (r,g,b) -> Color.FromArgb (255,(int r),(int g),(int b)))