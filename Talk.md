Writing a simple ray-tracer in an hour.

First slide:
    Talk about what a ray tracer actually is:
    "Generate an image by tracing the path of light as pixels
    in an image plane and simulating effects of its
    encounters with objects"

    Mention that the aim of this is to write a simple
    ray tracer to get something that provides pleasant
    results whilst showing the benefits of a FP approach.

Next slide:

    Talk about how we will display an image: we'll be using the PPM format because it's incredibly simple

    Talk about how it is laid out. Introduce the tiny type that will encapsulate the type.

Next slide:

    The foundations of graphics applications is always a vector, it'll be used for positions, directions and offsets.

    Show the basic code for vec and why we structure code in this way: time to introduce the pipe operator

Next slide:
    Next we need to move onto the Ray. It's obvious why this is required (it's called a RAY tracer)

    So, a ray is just a line. We can represent it with a position, A, and a direction, B. We can then find a position on the ray with the following function:
    p(t) = A + t * B

Next slide:
    Now we're looking to actually start making something
    We'll go with a 1920 * 1080 image (something that isn't square to avoid missing errors due to a square image)

    So, we're going to use the right hand coordinate system (favoured by OpenGL). This means that we have:
        x is positive going to the right
        y is positive going "up"
        z is positive coming out of the screen

    We then need to place our camera, to begin with we'll put it at 0,0,0 - the origin.
    We then, for each pixel in the image, calculate the direction vector from the center of the pixel to the camera, we can then use this to do a pretty image.

    How do we calculate the direction vector?
        In the example we have the lower left corner, the height and the length of the square.
        We can calculate the ray direction by:
            lowerLeft + (i * horizontal) + (j * vertical)
        The origin is the position of the ray.

Next slide:
    Show the simple output of this tracer

Next slide:
    Now, we want to add objects!
    We'll start with a sphere, then we'll add cuboids.

    Adding a sphere is simple, the equation is:
        x^2 + y^2 + z^2 = r^2
    This equation is when the sphere is centered around the origin, if you want to center the sphere around a point(a,b,c) then you need to use:
        (x-a)^2 + (y-b)^2 + (z-c)^2 = r^2

    Then we obviously want to talk about this in terms of vectors: c is the vector of the center of the so we get:
    dot (p-c, p-c)=r^2
    Then, of course, we want to replace P with our ray, as we can then get an equation to check if our ray hits the sphere:

    dot(p-c, p-c) where p = A + t*B

    So, what can we turn this into:
    p-c = A + t*B - C

    t^2 * dot(B,B) + 2t * dot(B, A-C) + dot(A-C, A-C) - R*R = 0

    So, this is a quadratic equation:
    ax^2 + bx + c = 0
    Then we can say:
    b^2 - 4ac
        If the above value is less than 0 then there is no valid collision.
        If the value is zero then there is a single point of intersection
        If the value is greater than 1 then there are 2 points of intersection.

    So, we show the basic Shapr type, that contains a sphere as a single case DU. We will currently have the hardcoded 

Next slide:
    Show the simple red sphere output.

Next slide
    We want this to look less...shit. So we need to introduce surface normals.

    Surface normals are vectors that are perpendicular to the surface that point out. For a sphere this is the direction ofthe hitpoint subtracting the center. We will enforce that the normal vectors are indeed unit vectors.

Next slide:
Once we're done with that, we need to talk about how we deal with multiple objects.
    We want to keep it simple for now, so we need to store a value of minimum and maximum valid "t" values, then we just pick the one closest to the minimum, that is what we hit.

    However, this requires a slight tweak to the way we deal with collisions - we need to keep track of this information rather than just returning a value that we splat into our colour code (obvious this was never going to last).

    What we do is create the HitRecord type, and have our function return a HitRecord option result.
        Now is a good time to talk about why we're doing it in this way: In a more traditional OO way you'd be looking at a bool + ref combo (ugly and requires defaults) or using null (which is very dangerous and requires the programmer to be incredibly defensive - it also isn't clear that the function can return null - sad!)

        So, the option type is a very simple type that allows you to explicitly show that you're potentially not going to return anything.

Next: Talk about the camera (todo - move this way up to the top!)