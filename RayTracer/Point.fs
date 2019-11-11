namespace RayTracer

[<Struct>]
type Point =
    {
        X : float
        Y : float
        Z : float
    } with

    static member (-) (lhs : Point, rhs : Point) : Vector =
        {
            X = lhs.X - rhs.X
            Y = lhs.Y - rhs.Y
            Z = lhs.Z - rhs.Z
        }

    static member (+) (lhs : Vector, rhs : Point) : Point =
        {
            Point.X = lhs.X + rhs.X
            Y = lhs.Y + rhs.Y
            Z = lhs.Z + rhs.Z
        }

    static member (+) (lhs : Point, rhs : Vector) : Point =
        rhs + lhs
