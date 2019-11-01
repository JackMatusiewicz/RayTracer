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
        {
            X = lhs.X + rhs.X
            Y = lhs.Y + rhs.Y
            Z = lhs.Z + rhs.Z
        }

    static member (-) (lhs : Vector, rhs : Vector) : Vector =
        {
            X = lhs.X - rhs.X
            Y = lhs.Y - rhs.Y
            Z = lhs.Z - rhs.Z            
        }

    static member (*) (a : Vector, b : Vector) =
        {
            X = a.X * b.X
            Y = a.Y * b.Y
            Z = a.Z * b.Z
        }


[<Struct>]
type UnitVector = private UnitVector of Vector

[<RequireQualifiedAccess>]
module Vector =

    let add (x : Vector) (y : Vector) = x + y

    let sub (rhs : Vector) (lhs : Vector) = lhs - rhs

    let multiply (x : Vector) (y : Vector) = x * y

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

    let squaredLength (x : Vector) =
        x.X * x.X + x.Y * x.Y + x.Z * x.Z

    let length (x : Vector) =
        squaredLength x
        |> Math.Sqrt

    let unitVector (x : Vector) : UnitVector =
        let len = length x
        scalarDivide len x
        |> UnitVector

[<RequireQualifiedAccess>]
module UnitVector =

    let toVector (UnitVector v) = v