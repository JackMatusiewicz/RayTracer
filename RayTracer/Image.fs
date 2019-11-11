namespace RayTracer

open System.Text
open System.IO

open System.Drawing
open System.Drawing.Imaging

[<RequireQualifiedAccess>]
module Image =

    let toFile (fileName : string) (pixels : Colour [,]) : unit =
        let rows = pixels.GetLength 0
        let cols = pixels.GetLength 1
        let image = new Bitmap(cols, rows)
        Array2D.iteri
            (fun y x v ->
                let col = Colour.toColor v
                image.SetPixel (x, y, col))
            pixels
        image.Save(fileName + ".png", ImageFormat.Png)
