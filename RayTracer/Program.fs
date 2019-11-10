open System
open RayTracer

let shapes =
    [
        {
            Shape = Sphere { Center = { X = 0.; Y = 0.; Z = -1200. }; Radius = 600. }
            Shader =
                {
                    Colour = { R = 1.; G = 0.; B = 0. }
                    AlbedoCoefficient = 0.5
                    Exponent = 5.
                }
                |> Glossy
                |> fun s -> { Matte.Diffuse = s } |> Matte
        }
        {
            Shape = Sphere { Center = { X = 0.; Y = 0.; Z = 0. }; Radius = 600. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 1.; B = 0. }
                    AlbedoCoefficient = 0.5
                }
                |> Diffuse
                |> fun s -> { Matte.Diffuse = s } |> Matte
        }
        {
            Shape = Plane { Point = { X = 0.; Y = -600.; Z = 0. }; Normal = Vector.normalise { Vector.X = 0.; Y = 1.; Z = 0. } }
            Shader =
                {
                    Lambertian.Colour = { R = 0.5; G = 0.5; B = 0.25 }
                    AlbedoCoefficient = 0.5
                    //Exponent = 1.
                }
                |> Diffuse
                |> fun s -> { Matte.Diffuse = s } |> Matte
        }
    ]

let rec multiCont (xs : ((('a -> 'k) -> 'k) list)) (f : 'a list -> 'k) : 'k =
    match xs with
    | [] -> f []
    | h :: t ->
        h (fun a -> multiCont t (fun xs -> a :: xs |> f))

// Deals with the path of a single ray.
let rec rayCollides'
    (shapes : SceneObject list)
    (lights : Light list)
    (r : Ray)
    : Colour
    =
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
    | vs ->
        let v =
            List.sortBy (fun hr -> hr.T) vs
            |> List.head
        let mutable col = { R = 0.; G = 0.; B = 0. }
        for l in lights do
            let dir =
                Light.direction l
                |> UnitVector.toVector
                |> Vector.scalarMultiply -1.
                |> Vector.normalise
            let lightRay = { Ray.Position = v.CollisionPoint; Direction = dir }
            let collisionPoints =
                List.map (Shape.collides { Min = 0.001 } lightRay) shapes
                |> List.choose id
            match collisionPoints with
            | [] ->
                Material.colour
                    v.CollisionPoint
                    v.Normal
                    dir
                    (r.Direction |> UnitVector.toVector |> Vector.scalarMultiply -1. |> Vector.normalise)
                    l
                    (rayCollides' shapes lights)
                    v.Material
            | _ ->
                { R = 0.; G = 0.; B = 0. }
            |> fun c -> col <- col + c
            
        col

// Deals with all rays for a particular cell (anti aliasing)
let rec rayCollides (shapes : SceneObject list) (lights : Light list) (r : Ray list) : Colour =
    List.map (rayCollides' shapes lights) r
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
            { Point.X = -2000.; Y = 1000.; Z = -600. }
            500.
            ({ Vector.X = 0.; Y = 1.; Z = 0. } |> Vector.normalise)
            ({ Vector.X = 2000.; Y = -1000.; Z = 600. } |> Vector.normalise)

    let l = DirectionalLight.make (Vector.normalise { X = 0.; Y = -1.; Z = 1.; }) { R = 1.; G = 1.; B = 1. } 1.
        
    Pinhole.getRays (System.Random ()) pinhole
    |> Array2D.map (rayCollides shapes [l])
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
