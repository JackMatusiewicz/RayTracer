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
        l.AlbedoCoefficient .* l.Colour

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

type IMaterial =
    abstract Colour
        : normal:UnitVector
        -> inDirection:UnitVector
        -> outDirection:UnitVector
        -> lightLuminosity:Colour
        -> contactPoint:Point
        -> getColour:(Ray -> Colour)
        -> Colour

[<RequireQualifiedAccess>]
module Matte =

    let make (lam : Lambertian) : IMaterial =
        { new IMaterial with
            member __.Colour normal inD outD lightLum contactPoint getColour =
                let ambient = 0.1 .* lam.Colour
                let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inD)
                if nDotIn < 0. then ambient
                else
                    let col = Lambertian.colour normal inD lam
                    nDotIn .* (lightLum * col + ambient)
        }

[<RequireQualifiedAccess>]
module Phong =

    let make (lam : Lambertian) (s : Specular) : IMaterial =
        { new IMaterial with
            member __.Colour normal inD outD lightLum contactPoint getColour =
                let ambient = 0.1 .* lam.Colour
                let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inD)
                if nDotIn < 0. then
                    ambient
                else
                    let diffCol = Lambertian.colour normal inD lam
                    let specCol = Specular.colour normal inD outD s
                    nDotIn .* (diffCol + specCol) * lightLum
        }

[<RequireQualifiedAccess>]
module Mirror =

    let make (lam : Lambertian) (s : Specular) : IMaterial =
        let phong = Phong.make lam s
        { new IMaterial with
            member __.Colour normal inD outD lightLum contactPoint getColour =
                let underlyingColour = phong.Colour normal inD outD lightLum contactPoint getColour
                let normal = UnitVector.toVector normal
                let inDirection = UnitVector.toVector inD
                let nDotIn = Vector.dot normal inDirection
                let r =
                    (-1. .* inDirection) + (2. * nDotIn .* normal)
                    |> Vector.normalise
                let ray = { Ray.Position = contactPoint; Direction = r }
                Vector.dot normal inDirection .* getColour ray
                |> (+) underlyingColour
        }
