open RayTracer

(*
let shapes' =
    [
        {
            Shape = Sphere { Center = { X = 450.; Y = 0.; Z = -1200. }; Radius = 150. }
            Shader =
                let specular =
                    {
                        Colour = { R = 1.; G = 1.; B = 0.5 }
                        AlbedoCoefficient = 0.5
                        Exponent = 7.
                    }
                let diffuse =
                    {
                        Colour = { R = 1.; G = 1.; B = 0.5 }
                        AlbedoCoefficient = 0.5
                    }
                { Phong.Diffuse = diffuse; Specular = specular } |> Phong
        }
        {
            Shape = Sphere { Center = { X = 150.; Y = 0.; Z = -600. }; Radius = 300. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 1.; B = 0. }
                    AlbedoCoefficient = 0.5
                }
                |> fun s -> { Matte.Diffuse = s } |> Matte
        }
        {
            Shape = Sphere { Center = { X = -150.; Y = 0.; Z = 0. }; Radius = 300. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 0.; B = 1. }
                    AlbedoCoefficient = 0.5
                }
                |> fun s -> { Matte.Diffuse = s } |> Matte
        }
        {
            Shape = Sphere { Center = { X = 0.; Y = 400.; Z = 600. }; Radius = 300. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 1.; B = 1. }
                    AlbedoCoefficient = 0.5
                }
                |> fun s -> { Matte.Diffuse = s } |> Matte
        }
        {
            Shape = Plane { Point = { X = 0.; Y = -600.; Z = 0. }; Normal = Vector.normalise { Vector.X = 0.; Y = 1.; Z = 0. } }
            Shader =
                let specular =
                    {
                        Colour = { R = 0.5; G = 0.5; B = 0.25 }
                        AlbedoCoefficient = 0.5
                        Exponent = 7.
                    }
                let diffuse =
                    {
                        Colour = { R = 0.5; G = 0.5; B = 0.25 }
                        AlbedoCoefficient = 0.5
                    }
                { Mirror.Phong = { Phong.Diffuse = diffuse; Specular = specular }} |> Reflective
        }
    ]
*)
let shapes =
    [
        {
            Shape = Sphere { Center = { X = 0.; Y = 0.; Z = -1350. }; Radius = 300. }
            Material = { R = 1.; G = 0.; B = 0. }
        }
        {
            Shape = Sphere { Center = { X = 0.; Y = 0.; Z = -900. }; Radius = 300. }
            Material = { R = 1.; G = 0.; B = 0. }
        }
        {
            Shape = Plane { Point = { X = 0.; Y = -600.; Z = 0. }; Normal = Vector.normalise { Vector.X = 0.; Y = 1.; Z = 0. } }
            Material = { R = 1.; G = 0.; B = 0. }
        }
    ]

let testScene () =
    {ViewPlane = { HorizontalResolution = 1920; VerticalResolution = 1080 }; Objects = shapes;}
    |> Scene.toImage

[<EntryPoint>]
let main _ =
    testScene ()
    0
