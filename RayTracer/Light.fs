namespace RayTracer

type internal DirectionalLight =
    {
        Direction : UnitVector
        Colour : Colour
        L : float
    }

type Light =
    internal | Directional of DirectionalLight

[<RequireQualifiedAccess>]
module DirectionalLight =

    let internal direction (hr : HitRecord) (dl : DirectionalLight) : UnitVector =
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
            DirectionalLight.direction hr d

    let luminosity (l : Light) : Colour =
        match l with
        | Directional d ->
            DirectionalLight.luminosity d