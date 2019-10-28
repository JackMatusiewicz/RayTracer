namespace RayTracer

open System

[<Struct>]
type Vector =
    {
        X : float
        Y : float
        Z : float
    }

[<Struct>]
type UnitVector = private UnitVector of Vector

[<RequireQualifiedAccess>]
module Vector =

    let add (x : Vector) (y : Vector) =
        {
            X = x.X + y.X
            Y = x.Y + y.Y
            Z = x.Z + y.Z
        }

    let sub (rhs : Vector) (lhs : Vector) =
        {
            X = lhs.X - rhs.X
            Y = lhs.Y - rhs.Y
            Z = lhs.Z - rhs.Z
        }

    let multiply (x : Vector) (y : Vector) =
        {
            X = x.X * y.X
            Y = x.Y * y.Y
            Z = x.Z * y.Z
        }

    let divide (denominator : Vector) (numerator : Vector) =
        {
            X = numerator.X / denominator.X
            Y = numerator.Y / denominator.Y
            Z = numerator.Z / denominator.Z
        }

    let scalarMultiply (s : float) (x : Vector) =
        {
            X = x.X * s
            Y = x.Y * s
            Z = x.Z * s
        }

    let dot (x : Vector) (y : Vector) =
        x.X * y.X + x.Y * y.Y + x.Z * y.Z

    let scalarDivide (s : float) (x : Vector) =
        {
            X = x.X / s
            Y = x.Y / s
            Z = x.Z / s
        }

    let length (x : Vector) =
        Math.Sqrt (x.X * x.X + x.Y * x.Y + x.Z * x.Z)

    let unitVector (x : Vector) : UnitVector =
        let len = length x
        scalarDivide len x
        |> UnitVector

[<RequireQualifiedAccess>]
module UnitVector =

    let toVector (UnitVector v) = v