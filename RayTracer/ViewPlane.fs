namespace RayTracer

open System

[<Struct>]
type ViewPlane =
    {
        HorizontalResolution : int
        VerticalResolution : int
        PixelSize : float
    }

[<RequireQualifiedAccess>]
module ViewPlane =

    let getXY (row : int) (col : int) (vp : ViewPlane) : float * float =
        let x = vp.PixelSize * ((float col) - 0.5 * (float vp.HorizontalResolution) + 0.5)
        let y = vp.PixelSize * ((float row) - 0.5 * (float vp.VerticalResolution) + 0.5)
        x,y

type OrthonormalBasis =
    {
        U : UnitVector
        V : UnitVector
        W : UnitVector
    }

type Pinhole =
    internal {
        ViewPlane : ViewPlane
        CameraLocation : Point
        CameraDistance : float
        Up : UnitVector
        Onb : OrthonormalBasis
    }

[<RequireQualifiedAccess>]
module Pinhole =

    let private makeOnb (up : UnitVector) (direction : UnitVector) =
        let w = -1. .* UnitVector.toVector direction    
        let u = Vector.cross (UnitVector.toVector up) w
        let v = Vector.cross w u

        {
            U = Vector.normalise u
            V = Vector.normalise v
            W = Vector.normalise w
        }

    let make
        (vp : ViewPlane)
        (location : Point)
        (distance :float)
        (up : UnitVector)
        (direction : UnitVector)
        : Pinhole
        =
        {
            ViewPlane = vp
            CameraLocation = location
            Up = up
            CameraDistance = distance
            Onb = makeOnb up direction
        }

    let private getRayDirection x y z (onb : OrthonormalBasis) =
        let u = x .* UnitVector.toVector onb.U
        let v = y .* UnitVector.toVector onb.V
        let w = z .* UnitVector.toVector onb.W
        Vector.normalise (u + v - w)

    let getRays (pinhole : Pinhole) : Ray[,] =
        Array2D.init
            pinhole.ViewPlane.VerticalResolution
            pinhole.ViewPlane.HorizontalResolution
            (fun r c ->
                let r = pinhole.ViewPlane.VerticalResolution - r - 1
                let x,y =
                    ViewPlane.getXY r c pinhole.ViewPlane
                let dir = getRayDirection x y pinhole.CameraDistance pinhole.Onb
                { Origin = pinhole.CameraLocation; Direction = dir }
            )
