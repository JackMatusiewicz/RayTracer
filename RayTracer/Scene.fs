namespace RayTracer


type Scene =
    {
        Objects : SceneObject list
        Lights : Light list
        GetCameraRays : unit -> Ray[,]
    }


[<RequireQualifiedAccess>]
module Scene =

    let private findClosestCollision (shapes : SceneObject list) (ray : Ray) : CollisionRecord option =
        List.map (Shape.collides 0.001 ray) shapes
        |> List.choose id
        |> function
            | [] -> None
            | cs ->
                List.sortBy (fun c -> c.T) cs
                |> List.head
                |> Some

    let rec private shadeAtCollision
        (sceneObjects : SceneObject list)
        (cr : CollisionRecord)
        (r : Ray)
        (cont : Ray -> Colour)
        (l : Light)
        : Colour
        =
        let dir =
            Light.direction l
            |> UnitVector.toVector
            |> Vector.scalarMultiply -1.
            |> Vector.normalise
        let lightRay = { Origin = cr.CollisionPoint; Direction = dir }

        match findClosestCollision sceneObjects lightRay with
        | None ->
            cr.Material.Colour
                cr.Normal
                dir
                (r.Direction |> UnitVector.toVector |> Vector.scalarMultiply -1. |> Vector.normalise)
                (Light.luminosity l)
                cr.CollisionPoint
                cont
        | Some _ ->  { R = 0.; G = 0.; B = 0. }

    and private getColourForRay
        (shapes : SceneObject list)
        (lights : Light list)
        (r : Ray)
        : Colour
        =
        findClosestCollision shapes r
        |> Option.map
            (fun collision ->
                List.map (shadeAtCollision shapes collision  r (getColourForRay shapes lights)) lights
                |> List.fold (+) { R = 0.; G = 0.; B = 0.})
        |> Option.defaultValue { R = 0.; G = 0.; B = 0. }

    let toImage (scene : Scene) : Colour[,] =
        scene.GetCameraRays ()
        |> Array2D.map (getColourForRay scene.Objects scene.Lights)
