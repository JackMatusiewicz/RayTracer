// Learn more about F# at http://fsharp.org

open System
open RayTracer

let shapes =
    [
        {
            Shape = Sphere { Center = { X = 0.; Y = 0.; Z = -1200. }; Radius = 600. }
            Colour = { R = 1.; G = 0.; B = 0. }
        }
        {
            Shape = Sphere { Center = { X = 0.; Y = 0.; Z = 0. }; Radius = 600. }
            Colour = { R = 0.; G = 1.; B = 0. }
        }
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
let rec rayCollides' (shapes : SceneObject list) (lights : Light list) (r : Ray) (kont : Colour -> 'k) : 'k =
    let collisionPoints =
        List.map (Shape.collides { Min = 0.001 } r) shapes
        |> List.choose id
    match collisionPoints with
    | [] ->
        {
            R = 0.
            G = 0.
            B = 0.
        }
        |> kont
    | vs ->
        let v =
            List.sortBy (fun hr -> hr.T) vs
            |> List.head
        let mutable col = { R = 0.; G = 0.; B = 0. }
        for l in lights do
            let dir =
                Light.direction v l
                |> UnitVector.toVector
                |> Vector.scalarMultiply -1.
                |> Vector.normalise
            let lightRay = { Ray.Position = v.CollisionPoint; Direction = dir }
            let collisionPoints =
                List.map (Shape.collides { Min = 0.001 } lightRay) shapes
                |> List.choose id
            match collisionPoints with
            | [] ->
                failwith "colour from light"
            | _ -> { R = 0.; G = 0.; B = 0. }
            |> fun c -> col <- col + c
            
        col |> kont

// Deals with all rays for a particular cell (anti aliasing)
let rec rayCollides (shapes : SceneObject list) (lights : Light list) (r : Ray list) : Colour =
    List.map
        (fun ray -> rayCollides' shapes lights ray id) r
    |> Colour.reduceAndAverage

let hackyScene () =
    let pinhole =
        Pinhole.make
            {
                HorizontalResolution = 1920
                VerticalResolution = 1080
                PixelSize = 1.
            }
            (*{ Point.X = 0.; Y = 0.; Z = -3200. }
            500.
            ({ Vector.X = 0.; Y = 1.; Z = 0. } |> Vector.normalise)
            ({ Vector.X = 0.; Y = 0.; Z = 1. } |> Vector.normalise)*)
            { Point.X = -2000.; Y = 0.; Z = -600. }
            500.
            ({ Vector.X = 0.; Y = 1.; Z = 0. } |> Vector.normalise)
            ({ Vector.X = 1.; Y = 0.; Z = 0. } |> Vector.normalise)

    let l = DirectionalLight.make (Vector.normalise { X = 0.; Y = 0.; Z = 1.; }) { R = 1.; G = 1.; B = 1. } 1.
        
    Pinhole.getRays pinhole
    |> Array2D.map (List.singleton >> rayCollides shapes [l])
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
