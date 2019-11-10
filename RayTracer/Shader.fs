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
        (s : Shader)
        : Colour
        =
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
            Colour.scalarMultiply l.AlbedoCoefficient l.Colour
        | Glossy s ->
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

    let baseColour (s : Shader) : Colour =
        match s with
        | Diffuse l -> l.Colour
        | Glossy g -> g.Colour

type Matte =
    {
        Diffuse : Shader
    }

type Phong =
    {
        Diffuse : Shader
        Specular : Shader
    }

type Material =
    | Matte of Matte
    | Phong of Phong

[<RequireQualifiedAccess>]
module Material =
    let colour
        (contactPoint : Point)
        (normal : UnitVector)
        (inDirection : UnitVector)
        (outDirection : UnitVector)
        (l : Light)
        (getColour : Ray -> Colour)
        (m : Material)
        : Colour
        =
        
        match m with
        | Matte m ->
            let ambient = Shader.baseColour m.Diffuse * {R = 0.025; G = 0.025; B = 0.025}
            let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inDirection)
            if nDotIn < 0. then ambient
            else
                let col = Shader.colour contactPoint normal inDirection outDirection getColour m.Diffuse
                (Light.luminosity l) * col + ambient
                |> Colour.scalarMultiply nDotIn
        | Phong p ->
            let ambient = Shader.baseColour p.Diffuse * {R = 0.025; G = 0.025; B = 0.025}
            let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inDirection)
            if nDotIn < 0. then
                printfn "BONK"
                ambient
            else
                let diffCol = Shader.colour contactPoint normal inDirection outDirection getColour p.Diffuse
                let specCol = Shader.colour contactPoint normal inDirection outDirection getColour p.Specular
                (diffCol + specCol) * (Light.luminosity l)
                |> Colour.scalarMultiply nDotIn