namespace RayTracer

[<Struct>]
type ViewPlane =
    {
        HorizontalResolution : int
        VerticalResolution : int
    }

[<RequireQualifiedAccess>]
module ViewPlane =

    let getXY (row : int) (col : int) (vp : ViewPlane) : float * float =
        let x = ((float col) - 0.5 * (float vp.HorizontalResolution) + 0.5)
        let y = ((float row) - 0.5 * (float vp.VerticalResolution) + 0.5)
        x,y

    let getRays (vp : ViewPlane) : Ray[,] =
        Array2D.init
            vp.VerticalResolution
            vp.HorizontalResolution
            (fun row col ->
                let row = vp.VerticalResolution - row - 1
                let x,y =
                    getXY row col vp
                {
                    Position = { X = x; Y = y; Z = 0. }
                    Direction = Vector.normalise {X = 0.; Y = 0.; Z = -1.}
                }
            )

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
                let x,y =
                    ViewPlane.getXY r c pinhole.ViewPlane
                let dir = getRayDirection x y pinhole
                { Position = pinhole.CameraLocation; Direction = dir }
            )
