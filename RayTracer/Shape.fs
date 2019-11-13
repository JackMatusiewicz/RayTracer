namespace RayTracer
open System

[<Struct>]
type CollisionRecord =
    {
        /// The parameter to plug into the ray calculation
        T : float
        /// The collision point on the surface of the object
        CollisionPoint : Point
        /// The normal of the object at the collision point
        Normal : UnitVector
        /// The colour of the object
        Material : Colour
    }

module CollisionRecord =

    let tryMake
        (minT : float)
        (t : float)
        (r : Ray)
        (n : UnitVector)
        (c : Colour)
        : CollisionRecord option
        =
        if t > minT then
            {
                T = t
                CollisionPoint = Ray.getPosition t r
                Normal = n
                Material = c
            } |> Some
        else None

[<Struct>]
type Sphere =
    {
        Center : Point
        Radius : float
    }

type Plane =
    {
        Point : Point
        Normal : UnitVector
    }

type Shape =
    | Sphere of Sphere
    | Plane of Plane

type SceneObject =
    {
        Shape : Shape
        Material : Colour
    }

[<RequireQualifiedAccess>]
module internal Sphere =

    let rayIntersects
        (minT : float)
        (r : Ray)
        (colour : Colour)
        (s : Sphere)
        =

        let bV = UnitVector.toVector r.Direction
        let aMinusC = r.Position - s.Center

        // Calculating the terms for the quadratic equation formula
        let a = Vector.dot bV bV
        let b = 2. * Vector.dot aMinusC bV
        let c = Vector.dot aMinusC aMinusC - (s.Radius * s.Radius)
        let discriminant = b * b - 4. * a * c
        if discriminant < 0. then
            None
        else
            let firstT = (-b - Math.Sqrt discriminant) / (2. * a)
            let secondT = (-b + Math.Sqrt discriminant) / (2. * a)
            let firstNormal = (Ray.getPosition firstT r) - s.Center |> Vector.normalise
            let secondNormal = (Ray.getPosition secondT r) - s.Center |> Vector.normalise

            CollisionRecord.tryMake minT firstT r firstNormal colour
            |> Option.orElse (CollisionRecord.tryMake minT secondT r secondNormal colour)

[<RequireQualifiedAccess>]
module internal Plane =

    let rayIntersects
        (p : Plane)
        (minT : float)
        (c : Colour)
        (r : Ray)
        : CollisionRecord option
        =
        let dDotN = (Vector.dot (UnitVector.toVector r.Direction) (UnitVector.toVector p.Normal))
        // If the ray is parallel to the plane, we definitely won't intersect.
        if dDotN = 0. then
            None
        else
            let t =
                // (a - o) . n / d . n
                (Vector.dot (p.Point - r.Position) (UnitVector.toVector p.Normal)) / dDotN
            CollisionRecord.tryMake minT t r p.Normal c

[<RequireQualifiedAccess>]
module Shape =

    let collides (minT : float) (r : Ray) (s : SceneObject) =
        match s.Shape with
        | Sphere sp ->
            Sphere.rayIntersects minT r s.Material sp
        | Plane p ->
            Plane.rayIntersects p minT s.Material r