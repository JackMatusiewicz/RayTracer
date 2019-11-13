namespace RayTracer
open System

[<Struct>]
type HitRecord =
    {
        T : float
        CollisionPoint : Point
        Normal : UnitVector
        Material : IMaterial
    }

[<Struct>]
type ParameterRange =
    {
        Min : float
    }

[<RequireQualifiedAccess>]
module ParameterRange =

    let inRange (pr : ParameterRange) (v : float) =
        v >= pr.Min

[<Struct>]
type Sphere =
    internal {
        Center : Point
        Radius : float
    }

[<Struct>]
type Plane =
    internal {
        Point : Point
        Normal : UnitVector
    }

type Shape =
    | Sphere of Sphere
    | Plane of Plane

type SceneObject =
    {
        Shape : Shape
        Shader : IMaterial
    }

[<RequireQualifiedAccess>]
module internal Sphere =

    let rayIntersects
        (range : ParameterRange)
        (r : Ray)
        colour
        (s : Sphere)
        =
        let tryCreateHitRecord v =
            if ParameterRange.inRange range v then
                let p = Ray.getPosition v r
                {
                    T = v
                    CollisionPoint = p
                    Normal =
                        p - s.Center
                        |> Vector.scalarDivide s.Radius
                        |> Vector.normalise
                    Material = colour
                } |> Some
            else None

        let aV = r.Origin
        let bV = UnitVector.toVector r.Direction
        let cV = s.Center
        let aMinusC = aV - cV

        let a = Vector.dot bV bV
        let b = 2. * Vector.dot aMinusC bV
        let c = Vector.dot aMinusC aMinusC - (s.Radius* s.Radius)
        let discriminant = b * b - 4. * a * c
        if discriminant < 0. then
            None
        else
            let v = (-b - Math.Sqrt discriminant) / (2. * a)
            let v' = (-b + Math.Sqrt discriminant) / (2. * a)
            tryCreateHitRecord v
            |> Option.orElse (tryCreateHitRecord v')

[<RequireQualifiedAccess>]
module internal Plane =

    let rayIntersects
        (p : Plane)
        (pr : ParameterRange)
        colour
        (r : Ray)
        =
        let rayDirection = UnitVector.toVector r.Direction
        let (UnitVector normal) = p.Normal
        let t =
            Vector.dot (p.Point - r.Origin) (UnitVector.toVector p.Normal)
            |> fun v -> v / (Vector.dot rayDirection normal)
        if ParameterRange.inRange pr t then
            let pt = Ray.getPosition t r
            {
                T = t
                CollisionPoint = pt
                Normal = p.Normal
                Material = colour
            } |> Some
        else None

[<RequireQualifiedAccess>]
module Shape =

    let collides (pr : ParameterRange) (r : Ray) (s : SceneObject) =
        match s.Shape with
        | Sphere sp ->
            Sphere.rayIntersects pr r s.Shader sp
        | Plane p ->
            Plane.rayIntersects p pr s.Shader r