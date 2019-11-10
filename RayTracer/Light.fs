namespace RayTracer

type internal DirectionalLight =
    {
        Direction : UnitVector
        Colour : Colour
        L : float
    }

type Light =
    internal
    | Directional of DirectionalLight

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

    let direction (l : Light) : UnitVector =
        match l with
        | Directional d ->
            DirectionalLight.direction d

    let luminosity (l : Light) : Colour =
        match l with
        | Directional d ->
            DirectionalLight.luminosity d

    let getPosition (l : Light) : Point option =
        match l with
        | Directional _ -> None