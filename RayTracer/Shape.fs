namespace RayTracer
open System

[<Struct>]
type HitRecord =
    {
        T : float
        CollisionPoint : Point
        Normal : UnitVector
        Colour : Colour
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
        Colour : Colour
    }

[<RequireQualifiedAccess>]
module internal Sphere =

    let rayIntersects
        (range : ParameterRange)
        (r : Ray)
        (c : Colour)
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
                        |> Vector.unitVector
                    Colour = c
                } |> Some
            else None

        let aV = r.Position
        let bV = UnitVector.toVector r.Direction
        let cV = s.Center
        let aMinusC = aV - cV

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

[<RequireQualifiedAccess>]
module internal Plane =

    let rayIntersects
        (p : Plane)
        (pr : ParameterRange)
        (c : Colour)
        (r : Ray)
        =
        let rp = { Point.X =r.Position.X; Y = r.Position.Y; Z = r.Position.Z }
        let t =
            Vector.dot
                (p.Point - rp)
                (UnitVector.toVector p.Normal)
            |> fun v -> v / (Vector.dot (UnitVector.toVector r.Direction) (UnitVector.toVector p.Normal))
        if ParameterRange.inRange pr t then
            let pt = Ray.getPosition t r
            {
                T = t
                CollisionPoint = pt
                Normal = p.Normal
                Colour = c
            } |> Some
        else None

[<RequireQualifiedAccess>]
module Shape =

    let collides (pr : ParameterRange) (r : Ray) (s : SceneObject) =
        match s.Shape with
        | Sphere sp ->
            Sphere.rayIntersects pr r s.Colour sp
        | Plane p ->
            Plane.rayIntersects p pr s.Colour r