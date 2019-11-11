namespace RayTracer

[<Struct>]
type Ray =
    {
        Position : Point
        Direction : UnitVector
    }

[<RequireQualifiedAccess>]
module Ray =

    let getPosition
        (t : float)
        ({Position = x; Direction = UnitVector v } : Ray)
        : Point
        =
        (Vector.scalarMultiply t v) + x

