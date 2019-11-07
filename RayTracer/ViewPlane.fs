namespace RayTracer

[<Struct>]
type ViewPlane =
    {
        HorizontalResolution : int
        VerticalResolution : int
        PixelSize : float
    }

type OrthonormalBasis =
    {
        U : UnitVector
        V : UnitVector
        W : UnitVector
    }

[<RequireQualifiedAccess>]
module ViewPlane =

    let getXY (row : int) (col : int) (vp : ViewPlane) : float * float =
        let x = vp.PixelSize * ((float col) - 0.5 * (float vp.HorizontalResolution) + 0.5)
        let y = vp.PixelSize * ((float row) - 0.5 * (float vp.VerticalResolution) + 0.5)
        x,y

type Pinhole =
    internal {
        ViewPlane : ViewPlane
        CameraLocation : Point
        ViewDirection : UnitVector
        CameraDistance : float
        Up : UnitVector
        Onb : OrthonormalBasis
    }

[<RequireQualifiedAccess>]
module Pinhole =

    let private makeOnb (up : UnitVector) (direction : UnitVector) =
        let w =
            UnitVector.toVector direction
            |> Vector.scalarMultiply -1.
            
        let u =
            UnitVector.toVector up
            |> fun up -> Vector.cross up w
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
            ViewDirection = direction
            Up = up
            CameraDistance = distance
            Onb = makeOnb up direction
        }

    let private getRayDirection x y (pinhole : Pinhole) =
        let u =
            UnitVector.toVector pinhole.Onb.U
            |> Vector.scalarMultiply x
        let v =
            UnitVector.toVector pinhole.Onb.V
            |> Vector.scalarMultiply y
        let w =
            UnitVector.toVector pinhole.Onb.W
            |> Vector.scalarMultiply pinhole.CameraDistance
        let ret =
            u + v - w
            |> Vector.normalise
        ret

    let getRays (pinhole : Pinhole) : Ray[,] =
        Array2D.init
            pinhole.ViewPlane.VerticalResolution
            pinhole.ViewPlane.HorizontalResolution
            (fun r c ->
                let r = pinhole.ViewPlane.VerticalResolution - r - 1
                let x,y = ViewPlane.getXY r c pinhole.ViewPlane
                let dir = getRayDirection x y pinhole
                { Position = pinhole.CameraLocation; Direction = dir })
