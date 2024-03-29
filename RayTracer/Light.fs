namespace RayTracer

type DirectionalLight =
    internal {
        Direction : UnitVector
        Colour : Colour
        Luminosity : float
    }

type Light =
    internal
    | Directional of DirectionalLight

[<RequireQualifiedAccess>]
module DirectionalLight =

    let internal direction (dl : DirectionalLight) : UnitVector =
        dl.Direction

    let internal luminosity (dl : DirectionalLight) : Colour =
        dl.Luminosity .* dl.Colour

    let make (direction : UnitVector) (colour : Colour) (luminosity : float) : Light =
        { Direction = direction; Colour = colour; Luminosity = luminosity }
        |> Directional

[<RequireQualifiedAccess>]
module Light =

    let direction (l : Light) : UnitVector =
        match l with
        | Directional d ->
            DirectionalLight.direction d

    let luminosity (l : Light) : Colour =
        match l with
        | Directional d ->
            DirectionalLight.luminosity d
