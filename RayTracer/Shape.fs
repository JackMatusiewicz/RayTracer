namespace RayTracer
open System

[<Struct>]
type Sphere =
    internal {
        Center : Vector
        Radius : float
    }

type Shape =
    | Sphere of Sphere

[<RequireQualifiedAccess>]
module Shape =

    let sphereCollides (s:Sphere) (r : Ray) : float =
        let aV = r.Position
        let bV = UnitVector.toVector r.Direction
        let cV = s.Center
        let aMinusC = Vector.sub cV aV

        let a = Vector.dot bV bV
        let b =
            Vector.dot aMinusC bV
            |> fun v -> v * 2.
        let c =
            Vector.dot aMinusC aMinusC
            |> fun v -> v - (s.Radius * s.Radius)
        let discriminant = b * b - 4. * a * c
        if discriminant < 0. then
            -1.
        else
            (-b - Math.Sqrt discriminant) / (2. * a)

    let hackySphere = { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
    let hackySphereScene (r : Ray) : Colour =
        let v = sphereCollides hackySphere r
        if v > 0. then
            Ray.getPosition v r
            |> Vector.sub hackySphere.Center
            |> Vector.unitVector
            |> UnitVector.toVector
            |> (fun {X=x;Y=y;Z=z} -> { X = x+1.; Y = y+1.; Z = z+1. })
            |> Vector.scalarMultiply 0.5
        else
            let dir = UnitVector.toVector r.Direction
            let t = 0.5 * (dir.Y + 1.)
            let a =
                {X = 1.; Y = 1.; Z = 1.}
                |> Vector.scalarMultiply (1. - t)
            let b =
                {X = 0.5; Y = 0.7; Z = 1.}
                |> Vector.scalarMultiply t
            Vector.add a b
        |> fun r ->
            {
                R = r.X * 255.99 |> uint8
                G = r.Y * 255.99 |> uint8
                B = r.Z * 255.99 |> uint8
            }

