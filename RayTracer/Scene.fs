namespace RayTracer

type Scene =
    {
        Camera : Pinhole
        Objects : SceneObject list
        Lights : Light list
    }

[<RequireQualifiedAccess>]
module Scene =

    let rec private getColourForRay
        (shapes : SceneObject list)
        (lights : Light list)
        (r : Ray)
        : Colour
        =
        let collisionPoints =
            List.map (Shape.collides { Min = 0.001 } r) shapes
            |> List.choose id
        match collisionPoints with
        | [] ->
            {
                R = 0.
                G = 0.
                B = 0.
            }
        | vs ->
            let v =
                List.sortBy (fun hr -> hr.T) vs
                |> List.head
            lights
            |> List.map
                (fun l ->
                    let dir =
                        Light.direction l
                        |> UnitVector.toVector
                        |> Vector.scalarMultiply -1.
                        |> Vector.normalise
                    let lightRay = { Ray.Origin = v.CollisionPoint; Direction = dir }
                    let collisionPoints =
                        List.map (Shape.collides { Min = 0.001 } lightRay) shapes
                        |> List.choose id
                    match collisionPoints with
                    | [] ->
                        v.Material.Colour
                            v.Normal
                            dir
                            (r.Direction |> UnitVector.toVector |> Vector.scalarMultiply -1. |> Vector.normalise)
                            (Light.luminosity l)
                            v.CollisionPoint
                            (getColourForRay shapes lights)
                    | _ ->
                        { R = 0.; G = 0.; B = 0. })
                |> List.reduce (+)
        
    let toImage (scene : Scene) : Colour[,] =
        Pinhole.getRays scene.Camera ()
        |> Array2D.map (getColourForRay scene.Objects scene.Lights)
