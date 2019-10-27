namespace RayTracer

type Ray =
    {
        Position : Vector
        Direction : UnitVector
    }

[<RequireQualifiedAccess>]
module Ray =

    let getPosition
        (t : float)
        ({Position = x; Direction = UnitVector v } : Ray)
        : Vector
        =
        Vector.add x (Vector.scalarMultiply t v)

