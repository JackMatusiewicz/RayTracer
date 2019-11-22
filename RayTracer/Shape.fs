namespace RayTracer
open System

[<Struct>]
type CollisionRecord =
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
        Shader : IMaterial
    }
    
[<RequireQualifiedAccess>]
module CollisionRecord =

    let tryMake
        (minT : float)
        (t : float)
        (r : Ray)
        (n : UnitVector)
        (c : IMaterial)
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

[<RequireQualifiedAccess>]
module internal Sphere =

    let rayIntersects
        (range : float)
        ({Direction = UnitVector rayDir} as r : Ray)
        (material : IMaterial)
        (s : Sphere)
        =
        let aMinusC = r.Origin - s.Center
        let a = Vector.dot rayDir rayDir
        let b = 2. * Vector.dot aMinusC rayDir
        let c = Vector.dot aMinusC aMinusC - (s.Radius * s.Radius)
        let discriminant = b * b - 4. * a * c
        if discriminant < 0. then
            None
        else
            let v = (-b - Math.Sqrt discriminant) / (2. * a)
            let v' = (-b + Math.Sqrt discriminant) / (2. * a)
            let firstNormal = (Ray.getPosition v r) - s.Center |> Vector.normalise
            let secondNormal = (Ray.getPosition v' r) - s.Center |> Vector.normalise
            CollisionRecord.tryMake range v r firstNormal material
            |> Option.orElse (CollisionRecord.tryMake range v' r secondNormal material)

[<RequireQualifiedAccess>]
module internal Plane =

    let rayIntersects
        (p : Plane)
        (pr : float)
        (c : IMaterial)
        (r : Ray)
        =
        let dDotN = (Vector.dot (UnitVector.toVector r.Direction) (UnitVector.toVector p.Normal))
        // If the ray is parallel to the plane, we definitely won't intersect.
        if dDotN = 0. then
            None
        else
            let t =
                // (a - o) . n / d . n
                (Vector.dot (p.Point - r.Origin) (UnitVector.toVector p.Normal)) / dDotN
            CollisionRecord.tryMake pr t r p.Normal c

[<RequireQualifiedAccess>]
module Shape =

    let collides (pr : float) (r : Ray) (s : SceneObject) =
        match s.Shape with
        | Sphere sp ->
            Sphere.rayIntersects pr r s.Shader sp
        | Plane p ->
            Plane.rayIntersects p pr s.Shader r