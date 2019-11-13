namespace RayTracer

open System

[<Struct>]
type Vector =
    {
        X : float
        Y : float
        Z : float
    } with

    static member (+) (lhs : Vector, rhs : Vector) : Vector =
        { X = lhs.X + rhs.X; Y = lhs.Y + rhs.Y; Z = lhs.Z + rhs.Z }

    static member (-) (lhs : Vector, rhs : Vector) : Vector =
        { X = lhs.X - rhs.X; Y = lhs.Y - rhs.Y; Z = lhs.Z - rhs.Z }

    static member (*) (a : Vector, b : Vector) : Vector =
        { X = a.X * b.X; Y = a.Y * b.Y; Z = a.Z * b.Z }

    static member (.*) (a : float, b : Vector) : Vector =
        { X = a * b.X; Y = a * b.Y; Z = a * b.Z }

[<Struct>]
type UnitVector = private UnitVector of Vector

[<RequireQualifiedAccess>]
module Vector =

    let scalarMultiply (s : float) (x : Vector) = s .* x

    let dot (x : Vector) (y : Vector) =
        x.X * y.X + x.Y * y.Y + x.Z * y.Z

    let cross (a : Vector) (b : Vector) : Vector =
        {
            X = a.Y * b.Z - a.Z * b.Y
            Y = a.Z * b.X - a.X * b.Z
            Z = a.X * b.Y - a.Y * b.X
        }

    let scalarDivide (s : float) (x : Vector) =
        { X = x.X / s; Y = x.Y / s; Z = x.Z / s }

    let squaredLength (x : Vector) =
        x.X * x.X + x.Y * x.Y + x.Z * x.Z

    let length (x : Vector) =
        squaredLength x |> Math.Sqrt

    let normalise (x : Vector) : UnitVector =
        let len = length x
        scalarDivide len x
        |> UnitVector

[<RequireQualifiedAccess>]
module UnitVector =

    let toVector (UnitVector v) = v