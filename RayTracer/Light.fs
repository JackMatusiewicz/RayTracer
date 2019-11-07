namespace RayTracer

type internal DirectionalLight =
    {
        Direction : UnitVector
        Colour : Colour
        L : float
    }

type internal PointLight =
    {
        Position : Point
        Colour : Colour
        L : float
        Radius : float
    }

type Light =
    internal
    | Directional of DirectionalLight
    | Point of PointLight

[<RequireQualifiedAccess>]
module PointLight =

    let internal direction (hr : HitRecord) (pl : PointLight) : UnitVector =
        pl.Position - hr.CollisionPoint
        |> Vector.normalise

    let internal luminosity (pl : PointLight) : Colour =
        Colour.scalarMultiply pl.L pl.Colour

    let make (pos : Point) (colour : Colour) (luminosity : float) (radius : float) : Light =
        {
            Position = pos
            Colour = colour
            L = luminosity
            Radius = radius
        } |> Point

[<RequireQualifiedAccess>]
module DirectionalLight =

    let internal direction (dl : DirectionalLight) : UnitVector =
        dl.Direction

    let internal luminosity (dl : DirectionalLight) : Colour =
        Colour.scalarMultiply dl.L dl.Colour

    let make (direction : UnitVector) (colour : Colour) (luminosity : float) : Light =
        { Direction = direction; Colour = colour; L = luminosity }
        |> Directional

[<RequireQualifiedAccess>]
module Light =

    let direction (hr : HitRecord) (l : Light) : UnitVector =
        match l with
        | Directional d ->
            DirectionalLight.direction d
        | Point p ->
            PointLight.direction hr p

    let luminosity (l : Light) : Colour =
        match l with
        | Directional d ->
            DirectionalLight.luminosity d
        | Point p ->
            PointLight.luminosity p