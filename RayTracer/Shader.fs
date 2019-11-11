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
        -> reflectionTrace:(Ray -> Colour)
        -> Colour

type Matte = inherit IMaterial
type Phong = inherit IMaterial
type Mirror = inherit IMaterial

[<RequireQualifiedAccess>]
module Matte =

    let make (lam : Lambertian) : Matte =
        { new Matte with
            member __.Colour normal inD _ lightLum _ _ =
                let ambient = 0.1 .* lam.Colour
                let nDotIn = Vector.dot (UnitVector.toVector normal) (UnitVector.toVector inD)
                if nDotIn < 0. then ambient
                else
                    let col = Lambertian.colour normal inD lam
                    nDotIn .* (lightLum * col + ambient)
        }

[<RequireQualifiedAccess>]
module Phong =

    let make (lam : Lambertian) (s : Specular) : Phong =
        { new Phong with
            member __.Colour normal inD outD lightLum _ _ =
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

    let make (phong : Phong) : Mirror =
        { new Mirror with
            member __.Colour normal inD outD lightLum contactPoint reflectionTrace =
                let underlyingColour = phong.Colour normal inD outD lightLum contactPoint reflectionTrace
                let normal = UnitVector.toVector normal
                let inDirection = UnitVector.toVector inD
                let nDotIn = Vector.dot normal inDirection
                let r =
                    (-1. .* inDirection) + (2. * nDotIn .* normal)
                    |> Vector.normalise
                let ray = { Ray.Position = contactPoint; Direction = r }
                Vector.dot normal inDirection .* reflectionTrace ray
                |> (+) underlyingColour
        }