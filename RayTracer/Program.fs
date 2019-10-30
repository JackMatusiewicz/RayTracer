// Learn more about F# at http://fsharp.org

open System
open RayTracer

let hackySphere = { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
let shapes =
    [
        Sphere { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
        Sphere { Center = { X = 0.; Y = -100.5; Z = -1. }; Radius = 100. }
    ]

let rayCollides (shapes : Shape list) (r : Ray) : Colour =
    let collisionPoints =
        List.map (Shape.collides { Min = 0.; Max = Double.MaxValue } r) shapes
        |> List.choose id
    match collisionPoints with
    | [] ->
        let dir = UnitVector.toVector r.Direction
        let t = 0.5 * (dir.Y + 1.)
        let a =
            {X = 1.; Y = 1.; Z = 1.}
            |> Vector.scalarMultiply (1. - t)
        let b =
            {X = 0.5; Y = 0.7; Z = 1.}
            |> Vector.scalarMultiply t
        Vector.add a b
    | vs ->
        let v =
            List.sortBy (fun hr -> hr.T) vs
            |> List.head
        v.Normal
        |> UnitVector.toVector
        |> (fun {X=x;Y=y;Z=z} -> { X = x+1.; Y = y+1.; Z = z+1. })
        |> Vector.scalarMultiply 0.5
    |> fun r ->
        {
            R = r.X * 255.99 |> uint8
            G = r.Y * 255.99 |> uint8
            B = r.Z * 255.99 |> uint8
        }

let hackyScene () =
    let x = 1920.
    let y = 1080.
    let llc = { X = -2.; Y = -1.; Z = -1. }
    let hori = { X = 4.; Y = 0.; Z = 0. }
    let vert = { X = 0.; Y = 2.; Z = 0. }
    let origin = { X = 0.; Y = 0.; Z = 0. }
    let rays =
        Array2D.init
            (int y)
            (int x)
            (fun v u ->
                let v = (int y) - v - 1
                let u = (float u) / x
                let v = (float v) / y
                let direction =
                    let a = Vector.scalarMultiply u hori
                    let b = Vector.scalarMultiply v vert
                    Vector.add a b
                    |> Vector.add llc
                    |> Vector.unitVector
                { Position = origin; Direction = direction }
            )
    Array2D.map (rayCollides shapes) rays
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
