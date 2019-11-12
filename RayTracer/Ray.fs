namespace RayTracer

[<Struct>]
type Ray =
    {
        Origin : Point
        Direction : UnitVector
    }

[<RequireQualifiedAccess>]
module Ray =

    let getPosition
        (t : float)
        ({Origin = x; Direction = UnitVector v } : Ray)
        : Point
        =
        t .* v + x
