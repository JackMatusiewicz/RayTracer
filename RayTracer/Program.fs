// Learn more about F# at http://fsharp.org

open System
open RayTracer

type ViewPlane =
    {
        LowerLeftCorner : Vector
        Horizontal : Vector
        Vertical : Vector
    }

let hackySphere = { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
let shapes =
    [
        Sphere { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
        Sphere { Center = { X = 0.; Y = -100.5; Z = -1. }; Radius = 100. }
    ]

let rayCollides (shapes : Shape list) (r : Ray list) : Colour =
    List.map
        (fun r -> 
            let collisionPoints =
                List.map (Shape.collides { Min = 0.; Max = Double.MaxValue } r) shapes
                |> List.choose id
            match collisionPoints with
            | [] ->
                { X = 0.; Y = 0.; Z = 0.}
            | vs ->
                let v =
                    List.sortBy (fun hr -> hr.T) vs
                    |> List.head
                v.Normal
                |> UnitVector.toVector
                |> (fun {X=x;Y=y;Z=z} -> { X = x+1.; Y = y+1.; Z = z+1. })
                |> Vector.scalarMultiply 0.5)
        r
        |> List.reduce Vector.add
        |> Vector.scalarDivide (float r.Length)
    |> fun r ->
        {
            R = r.X * 255.99 |> uint8
            G = r.Y * 255.99 |> uint8
            B = r.Z * 255.99 |> uint8
        }

let hackyScene () =
    let r = System.Random ()
    let x = 1920.
    let y = 1080.
    let antialiasingCount = 5
    let viewPlane =
        {
            LowerLeftCorner = { X = -2.; Y = -1.; Z = -1. }
            Horizontal = { X = 4.; Y = 0.; Z = 0. }
            Vertical = { X = 0.; Y = 2.; Z = 0. }
        }
    let origin = { X = 0.; Y = 0.; Z = 0. }
    let rays =
        Array2D.init
            (int y)
            (int x)
            (fun v u ->
                List.init antialiasingCount
                    (fun _ ->
                        let v = (int y) - v - 1
                        let u = (float u + r.NextDouble ()) / x
                        let v = (float v + r.NextDouble ()) / y
                        let direction =
                            let a = Vector.scalarMultiply u viewPlane.Horizontal
                            let b = Vector.scalarMultiply v viewPlane.Vertical
                            Vector.add a b
                            |> Vector.add viewPlane.LowerLeftCorner
                            |> Vector.unitVector
                        { Position = origin; Direction = direction })
            )
    Array2D.map (rayCollides shapes) rays
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
