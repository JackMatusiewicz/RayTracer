namespace RayTracer

open System

type Lambertian =
    {
        Colour : Colour
        AlbedoCoefficient : float
    }

[<RequireQualifiedAccess>]
module Lambertian =

    let colour (normal : UnitVector) (inDirection : UnitVector) (l : Lambertian) =
        let normal = UnitVector.toVector normal
        let inDirection = UnitVector.toVector inDirection
        let angleBetweenVectors =
            Vector.dot normal inDirection
        if angleBetweenVectors < 0. then
            { R = 0.; G = 0.; B = 0. }
        else
        l.AlbedoCoefficient / Math.PI .* l.Colour

type Specular =
    {
        Colour : Colour
        AlbedoCoefficient : float
        Exponent : float
    }

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
            (-1. .* inDirection) + (2. * nDotIn .* normal)
            |> Vector.normalise
            |> UnitVector.toVector
        let rDotOut = Vector.dot r outDirection
        if rDotOut > 0. then
            s.Colour
            |> (.*) s.AlbedoCoefficient
            |> (.*) (Math.Pow (rDotOut, s.Exponent))
        else { R = 0.; G = 0.; B = 0. }    
