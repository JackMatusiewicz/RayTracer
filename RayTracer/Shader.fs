namespace RayTracer

open System

type Specular =
    {
        Colour : Colour
        AlbedoCoefficient : float
        Exponent : float
    }

type Lambertian =
    {
        Colour : Colour
        AlbedoCoefficient : float
    }

module Lambertian =

    let colour (normal : UnitVector) (inDirection : UnitVector) (l : Lambertian) =
        let normal = UnitVector.toVector normal
        let inDirection = UnitVector.toVector inDirection
        let angleBetweenVectors =
            Vector.dot normal inDirection

        if angleBetweenVectors < 0. then
            { R = 0.; G = 0.; B = 0. }
        else
        (l.AlbedoCoefficient * angleBetweenVectors .* l.Colour)

[<RequireQualifiedAccess>]
module Specular =

    let colour
        (normal : UnitVector)
        (inDirection : UnitVector)
        (outDirection : UnitVector)
        (s : Specular)
        : Colour
        =
        let normal = UnitVector.toVector normal
        let inDirection = UnitVector.toVector inDirection
        let outDirection = UnitVector.toVector outDirection
        let nDotIn = Vector.dot normal inDirection
        let r =
            (Vector.scalarMultiply -1. inDirection) + (Vector.scalarMultiply (2. * nDotIn) normal)
            |> Vector.normalise
            |> UnitVector.toVector
        let rDotOut = Vector.dot r outDirection
        if rDotOut > 0. then
            s.Colour
            |> Colour.scalarMultiply s.AlbedoCoefficient
            |> Colour.scalarMultiply (Math.Pow (rDotOut, s.Exponent))
        else { R = 0.; G = 0.; B = 0. }    

type Matte =
    {
        Diffuse : Lambertian
    }

type Phong =
    {
        Diffuse : Lambertian
        Specular : Specular
    }

type Mirror =
    {
        Phong : Phong
    }

type Material =
    | Matte of Matte
    | Phong of Phong
    | Reflective of Mirror

[<RequireQualifiedAccess>]
module Material =

    let rec colour
        (normal : UnitVector)
        (inDirection : UnitVector)
        (outDirection : UnitVector)
        (lightLuminosity : Colour)
        (contactPoint : Point)
        (getColour : Ray -> Colour)
        (m : Material)
        : Colour
        =
        match m with
        | Matte m ->
            let ambient = Colour.scalarMultiply 0.1 m.Diffuse.Colour
            let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inDirection)
            if nDotIn < 0. then ambient
            else
                let col = Lambertian.colour normal inDirection m.Diffuse
                lightLuminosity * col + ambient
                |> Colour.scalarMultiply nDotIn
        | Phong p ->
            let ambient = Colour.scalarMultiply 0.1 p.Diffuse.Colour
            let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inDirection)
            if nDotIn < 0. then
                ambient
            else
                let diffCol = Lambertian.colour normal inDirection p.Diffuse
                let specCol = Specular.colour normal inDirection outDirection p.Specular
                (diffCol + specCol) * lightLuminosity
                |> Colour.scalarMultiply nDotIn
        | Reflective m ->
            let underlyingColour = colour normal inDirection outDirection lightLuminosity contactPoint getColour (Phong m.Phong)
            let normal = UnitVector.toVector normal
            let inDirection = UnitVector.toVector inDirection
            let nDotIn = Vector.dot normal inDirection
            let r =
                (Vector.scalarMultiply -1. inDirection) + (Vector.scalarMultiply (2. * nDotIn) normal)
                |> Vector.normalise
            let ray = { Ray.Origin = contactPoint; Direction = r }
            getColour ray
            |> Colour.scalarMultiply (Vector.dot normal inDirection)
            |> (+) underlyingColour