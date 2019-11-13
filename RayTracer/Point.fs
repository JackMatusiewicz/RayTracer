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
        { X = lhs.X + rhs.X; Y = lhs.Y + rhs.Y; Z = lhs.Z + rhs.Z }

    static member (+) (point : Point, vector : Vector) : Point = vector + point

    override this.ToString () =
        sprintf "%.3f, %.3f, %.3f" this.X this.Y this.Z

[<RequireQualifiedAccess>]
module Point =

    let make x y z : Point =
        {
            X = x; Y = y; Z = z
        }

    let add (v : Vector) (p : Point) : Point =
        {
            Point.X = p.X + v.X
            Y = p.Y + v.Y
            Z = p.Z + v.Z
        }

    let scalarAdd (v : float) (p : Point) : Point =
        {
            X = p.X + v
            Y = p.Y + v
            Z = p.Z + v
        }

    let scalarSub (v : float) (p : Point) : Point =
        {
            X = p.X - v
            Y = p.Y - v
            Z = p.Z - v
        }

    let scalarMultiply (v : float) (p : Point) : Point =
        {
            X = p.X * v
            Y = p.Y * v
            Z = p.Z * v
        }

    let lengthSquared (p : Point) : float =
        p.X * p.X + p.Y * p.Y + p.Z * p.Z

    let length (p : Point) : float =
        lengthSquared p
        |> System.Math.Sqrt

    let toString (v : Point) : string =
        sprintf "(%.3f, %.3f, %.3f)" v.X v.Y v.Z
