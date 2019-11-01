// Learn more about F# at http://fsharp.org

open System
open RayTracer

type PinholeCamera =
    {
        LowerLeftCorner : Vector
        Horizontal : Vector
        Vertical : Vector
        Origin : Vector
    }

let hackySphere = { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
let shapes =
    [
        Sphere { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
        Sphere { Center = { X = 0.; Y = -100.5; Z = -1. }; Radius = 100. }
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
    let x = 1920.
    let y = 1080.
    let antialiasingCount = 2
    let viewPlane =
        {
            LowerLeftCorner = { X = -2.; Y = -1.; Z = -1. }
            Horizontal = { X = 4.; Y = 0.; Z = 0. }
            Vertical = { X = 0.; Y = 2.; Z = 0. }
            Origin = { X = 0.; Y = 0.;  Z = 0. }
        }
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
                            |> Vector.sub viewPlane.Origin
                            |> Vector.unitVector
                        { Position = viewPlane.Origin; Direction = direction })
            )
    Array2D.map (rayCollides r shapes) rays
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
