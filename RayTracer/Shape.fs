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

    let sphereCollides (s:Sphere) (r : Ray) : bool =
        let aV = r.Position
        let bV = UnitVector.toVector r.Direction
        let cV = s.Center
        let aMinusC = Vector.sub aV cV

        let a = Vector.dot bV bV
        let b =
            Vector.dot bV aMinusC
            |> fun v -> v * 2.
        let c =
            Vector.dot aMinusC aMinusC
            |> fun v -> v - (s.Radius * s.Radius)
        b * b - 4. * a * c > 0.

    let hackySphere = { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
    let hackySphereScene (r : Ray) : Colour =
        if sphereCollides hackySphere r then
            { R = 255uy; G = 0uy; B = 0uy }
        else
            let dir = UnitVector.toVector r.Direction
            let t = 0.5 * (dir.Y + 1.)
            let a =
                {X = 1.; Y = 1.; Z = 1.}
                |> Vector.scalarMultiply (1. - t)
            let b =
                {X = 0.5; Y = 0.7; Z = 1.}
                |> Vector.scalarMultiply t
            let r =
                Vector.add a b
            {
                R = r.X * 255.99 |> uint8
                G = r.Y * 255.99 |> uint8
                B = r.Z * 255.99 |> uint8
            }

