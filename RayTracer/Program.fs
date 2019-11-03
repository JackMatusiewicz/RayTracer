// Learn more about F# at http://fsharp.org

open System
open RayTracer

let shapes =
    [
        Sphere { Center = { X = 0.; Y = 0.; Z = 0. }; Radius = 200. }
        Plane { Point = {X = 0.; Y = 0.; Z = 0.}; Normal = {Vector.X = 0.; Y = 1.; Z = 0.} |> Vector.unitVector }
        //Sphere { Center = { X = 0.; Y = -100.5; Z = -1. }; Radius = 100. }
    ]

let rec randomPointInUnitSphere (r : Random) : Vector =
    let p : Vector =
        Vector.make (r.NextDouble ()) (r.NextDouble ()) (r.NextDouble ())
        |> Vector.scalarMultiply 2.
        |> Vector.sub { X = 1.; Y = 1.; Z = 1. }
    if Vector.squaredLength p < 1. then p
    else randomPointInUnitSphere r

let rec multiCont (xs : ((('a -> 'k) -> 'k) list)) (f : 'a list -> 'k) : 'k =
    match xs with
    | [] -> f []
    | h :: t ->
        h (fun a -> multiCont t (fun xs -> a :: xs |> f))

// Deals with the path of a single ray.
let rec rayCollides' (ran : Random) (shapes : Shape list) (r : Ray) (kont : Colour -> 'k) : 'k =
    let collisionPoints =
        List.map (Shape.collides { Min = 0.001 } r) shapes
        |> List.choose id
    match collisionPoints with
    | [] ->
        {
            R = 255uy
            G = 255uy
            B = 255uy
        }
        |> kont
    | vs ->
        let v =
            List.sortBy (fun hr -> hr.T) vs
            |> List.head
        v.Normal
        |> UnitVector.toVector
        |> Vector.add v.CollisionPoint
        |> Vector.add (randomPointInUnitSphere ran)
        |> fun vec ->
            rayCollides'
                ran
                shapes
                {
                    Position = v.CollisionPoint
                    Direction = Vector.sub v.CollisionPoint vec |> Vector.unitVector
                }
                (fun c -> Colour.scalarMultiply 0.5 c |> kont)

// Deals with all rays for a particular cell (anti aliasing)
let rec rayCollides (ran : Random) (shapes : Shape list) (r : Ray list) : Colour =
    List.map
        (fun ray -> rayCollides' ran shapes ray id) r
    |> Colour.reduceAndAverage

let hackyScene () =
    let r = System.Random ()
    let pinhole =
        Pinhole.make
            {
                HorizontalResolution = 1920
                VerticalResolution = 1080
                PixelSize = 1.
            }
            { Point.X = 0.; Y = 0.; Z = -2000. }
            10.
            ({ Vector.X = 0.; Y = 1.; Z = 0. } |> Vector.unitVector)
            ({ Vector.X = 0.; Y = 0.; Z = 1. } |> Vector.unitVector)
    
    Pinhole.getRays pinhole
    |> Array2D.map (List.singleton >> rayCollides r shapes)
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
