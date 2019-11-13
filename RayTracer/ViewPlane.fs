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

type internal OrthonormalBasis =
    {
        U : UnitVector
        V : UnitVector
        W : UnitVector
    }

module internal OrthonormalBasis =

    let make
        (UnitVector up : UnitVector)
        ((UnitVector w) as wUnitVector : UnitVector) =
        let u = Vector.cross up w
        let v = Vector.cross w u
        { U = Vector.normalise u; V = Vector.normalise v; W = wUnitVector }

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

    let make (vp : ViewPlane) (location : Point) (distance : float) (up : UnitVector)
        ((UnitVector direction) : UnitVector)
        : Pinhole
        =
        {
            ViewPlane = vp
            CameraLocation = location
            Up = up
            CameraDistance = distance
            Onb =
                OrthonormalBasis.make
                    up
                    (-1. .* direction |> Vector.normalise)
        }

    let private convertToWorldCoordinates x y z (onb : OrthonormalBasis) =
        let u = x .* UnitVector.toVector onb.U
        let v = y .* UnitVector.toVector onb.V
        let w = z .* UnitVector.toVector onb.W
        Vector.normalise (u + v - w)

    let getRays (pinhole : Pinhole) : unit -> Ray[,] =
        fun () ->
            Array2D.init
                pinhole.ViewPlane.VerticalResolution
                pinhole.ViewPlane.HorizontalResolution
                (fun r c ->
                    let r = pinhole.ViewPlane.VerticalResolution - r - 1
                    let x,y =
                        ViewPlane.getXY r c pinhole.ViewPlane
                    let dir = convertToWorldCoordinates x y pinhole.CameraDistance pinhole.Onb
                    { Origin = pinhole.CameraLocation; Direction = dir }
                )
