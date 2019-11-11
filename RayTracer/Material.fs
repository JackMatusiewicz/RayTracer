namespace RayTracer

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
                    let col = Lambertian.colour lam
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
                    let diffCol = Lambertian.colour lam
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
                nDotIn .* reflectionTrace ray
                |> (+) underlyingColour
        }
