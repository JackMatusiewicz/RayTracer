namespace RayTracer
open System

[<Struct>]
type HitRecord =
    {
        T : float
        CollisionPoint : Vector
        Normal : UnitVector
    }

[<Struct>]
type ParameterRange =
    {
        Min : float
        Max: float
    }

[<RequireQualifiedAccess>]
module ParameterRange =

    let inRange (pr : ParameterRange) (v : float) =
        v >= pr.Min && v <= pr.Max

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

    let sphereCollides
        (range : ParameterRange)
        (r : Ray)
        (s : Sphere)
        : HitRecord option
        =
        let tryCreateHitRecord v =
            if ParameterRange.inRange range v then
                let p = Ray.getPosition v r
                {
                    T = v
                    CollisionPoint = p
                    Normal =
                        Vector.sub s.Center p
                        |> Vector.scalarDivide s.Radius
                        |> Vector.unitVector
                } |> Some
            else None
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
            None
        else
            let v = (-b - Math.Sqrt discriminant) / (2. * a)
            let v' = (-b + Math.Sqrt discriminant) / (2. * a)
            tryCreateHitRecord v
            |> Option.orElse (tryCreateHitRecord v')

    let collides (pr : ParameterRange) (r : Ray) (s : Shape) =
        match s with
        | Sphere s ->
            sphereCollides pr r s