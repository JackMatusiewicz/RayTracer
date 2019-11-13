namespace RayTracer

(*
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
            List.map (Shape.collides 0.001 r) shapes
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
            let mutable col = { R = 0.; G = 0.; B = 0. }
            for l in lights do
                let dir =
                    Light.direction l
                    |> UnitVector.toVector
                    |> Vector.scalarMultiply -1.
                    |> Vector.normalise
                let lightRay = { Ray.Position = v.CollisionPoint; Direction = dir }
                let collisionPoints =
                    List.map (Shape.collides 0.001 lightRay) shapes
                    |> List.choose id
                match collisionPoints with
                | [] ->
                    Material.colour
                        v.Normal
                        dir
                        (r.Direction |> UnitVector.toVector |> Vector.scalarMultiply -1. |> Vector.normalise)
                        (Light.luminosity l)
                        v.CollisionPoint
                        (getColourForRay shapes lights)
                        v.Material
                | _ ->
                    { R = 0.; G = 0.; B = 0. }
                |> fun c -> col <- col + c
                
            col
        
    let toImage (scene : Scene) : unit =
        Pinhole.getRays scene.Camera
        |> Array2D.map (getColourForRay scene.Objects scene.Lights)
        |> Ppm.toPpm
        |> Ppm.toDisk "testImage"

*)
module Image =
    let save (s : string) (_ : Colour[,]) = ()




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

    let private shadeAtCollision
        (sceneObjects : SceneObject list)
        (cr : CollisionRecord)
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
        | None -> Lambertian.colour cr.Normal dir cr.Material
        | Some _ ->  { R = 0.; G = 0.; B = 0. }
        |> (+) (0.1 .* cr.Material.Colour) //Ambient colour
            
        
    let rec private getColourForRay
        (shapes : SceneObject list)
        (lights : Light list)
        (r : Ray)
        : Colour
        =
        findClosestCollision shapes r
        |> Option.map
            (fun collision ->
                List.map (shadeAtCollision shapes collision) lights
                |> List.fold (+) { R = 0.; G = 0.; B = 0.})
        |> Option.defaultValue { R = 0.; G = 0.; B = 0. }
        
    let toImage (scene : Scene) : unit =
        scene.GetCameraRays ()
        |> Array2D.map (getColourForRay scene.Objects scene.Lights)
        |> Ppm.toPpm
        |> Ppm.toDisk "simpleImage"
