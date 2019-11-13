open RayTracer
open System.Drawing.Imaging

let shapes =
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
                Phong.make diffuse specular
        }
        {
            Shape = Sphere { Center = { X = 150.; Y = 0.; Z = -600. }; Radius = 300. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 1.; B = 0. }
                    AlbedoCoefficient = 0.5
                }
                |> Matte.make
        }
        {
            Shape = Sphere { Center = { X = -150.; Y = 0.; Z = 0. }; Radius = 300. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 0.; B = 1. }
                    AlbedoCoefficient = 0.5
                }
                |> Matte.make
        }
        {
            Shape = Sphere { Center = { X = 0.; Y = 400.; Z = 600. }; Radius = 300. }
            Shader =
                {
                    Lambertian.Colour = { R = 0.; G = 1.; B = 1. }
                    AlbedoCoefficient = 0.5
                }
                |> Matte.make
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
                Phong.make diffuse specular
                |> fun p -> Mirror.make p
        }
    ]

let testScene () =
    let pinhole =
        Pinhole.make
            {
                HorizontalResolution = 1920
                VerticalResolution = 1080
                PixelSize = 1.
            }
            { Point.X = 0.; Y = 0.; Z = -1800. }
            500.
            ({ Vector.X = 0.; Y = 1.; Z = 0. } |> Vector.normalise)
            ({ Vector.X = 0.; Y = 0.; Z = 600. } |> Vector.normalise)

    let l =
        DirectionalLight.make
            (Vector.normalise { X = 0.; Y = -1.; Z = 1.; })
            { R = 1.; G = 1.; B = 1. }
            2.5
        
    {Camera = pinhole; Objects = shapes; Lights = [l]}
    |> Scene.toImage

[<EntryPoint>]
let main _ =
    testScene ()
    |> Image.toFile "testImage" ImageFormat.Png
    0
