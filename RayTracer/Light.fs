namespace RayTracer

type internal DirectionalLight = DL of Point * Colour * float

type Light =
    internal | Directional of DirectionalLight

[<RequireQualifiedAccess>]
module DirectionalLight =

    let internal direction (hr : HitRecord) (DL (pos, _,_) : DirectionalLight) : UnitVector =
        pos - hr.CollisionPoint
        |> Vector.normalise

    let internal luminosity (DL (_, col,ls) : DirectionalLight) : Colour =
        Colour.scalarMultiply ls col

    let make (position : Point) (colour : Colour) (luminosity : float) : Light =
        DL (position, colour, luminosity)
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