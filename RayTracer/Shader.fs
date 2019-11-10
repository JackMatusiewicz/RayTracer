namespace RayTracer

open System

type Lambertian =
    {
        Colour : Colour
        AlbedoCoefficient : float
    }

type Specular =
    {
        Colour : Colour
        AlbedoCoefficient : float
        Exponent : float
    }

type Shader =
    | Diffuse of Lambertian
    | Glossy of Specular

[<RequireQualifiedAccess>]
module Shader =

    let colour
        (contactPoint : Point)
        (normal : UnitVector)
        (inDirection : UnitVector)
        (outDirection : UnitVector)
        (getColour : Ray -> Colour)
        (s : Shader) =
        let normal = UnitVector.toVector normal
        let inDirection = UnitVector.toVector inDirection
        let outDirection = UnitVector.toVector outDirection
        let angleBetweenVectors =
            Vector.dot normal inDirection
        match s with
        | Diffuse l ->
            if angleBetweenVectors < 0. then
                { R = 0.; G = 0.; B = 0. }
            else
            Colour.scalarMultiply
                (l.AlbedoCoefficient * Math.Max(0., angleBetweenVectors))
                l.Colour
        | Glossy s ->
            let diffColour =
                if angleBetweenVectors < 0. then
                    { R = 0.; G = 0.; B = 0. }
                else
                Colour.scalarMultiply
                    (s.AlbedoCoefficient * Math.Max(0., angleBetweenVectors))
                    s.Colour
            let h = inDirection + outDirection |> Vector.normalise
            let nDotH = Vector.dot normal (UnitVector.toVector h)
            if nDotH < 0. then
                { R = 0.; G = 0.; B = 0. }
            else
            s.Colour
            |> Colour.scalarMultiply s.AlbedoCoefficient
            |> (+) diffColour
            |> Colour.scalarMultiply (Math.Pow (nDotH, s.Exponent))

    let baseColour (s : Shader) : Colour =
        match s with
        | Diffuse l -> l.Colour
        | Glossy g -> g.Colour