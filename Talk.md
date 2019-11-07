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
    Talk about how orthographic projection isn't enough, we want to be able to move around the scene to get cool angles!
    Talk about what a virtual pinhole camera is. How it differs from an actual pinhole camera and how we implement it
    Show some simple pictures of why we did this!

Next: Lighting
    So, what we currently have can be achieved in paint...hurray?

    Time to go to the next level: we need lights and shading!

So, how do we deal with lighting? Well, we need to compute the light that is reflected off our surfaces towards
the camera. What do we need for this?
	Camera direction
	Light direction (for each light in the scene)
	Surface normal
	Material parameters (colour, shininess etc)

To start with, we'll just support a diffuse material (lambertian)
	Light will be scattered uniformly in all directions
	Relies on lambert's cosine law: light per unit area is proportional to the angle between the incident light
	ray and the surface normal at that point (cos theta)

So, as was said previously, shading is independent of viewing direction of lambertian shading:
	L_d = k_d * I * max(0, Vector.dot n i)

	L_d is the reflected light colour
	k_d is the diffuse coefficient
	I is the illumination from the light
	n,i are the vectors for the surface normal and the incident light ray, respectively.

What are the two types of light we'll support?
	Local light (spot light) - will have a position
	Directional light - has no position but a direction

Remember: Our surface is only illuminated _if_ nothing blocks the light from our surface!
So, we need to trace the light ray to confirm it hits!
So, our light ray is going to have a point of the surface hit, and a direction of (lightPos - point).
At this point, it works if you enable the small delta for checking the t parameter, or it's fucked.
This is due to FP errors

Now, what we'll notice is that the shadows look unrealistic, nothing is ever _that_ black unless it's complete darkness.

The solution is to add an ambient light, to make everything a little bit softer, then we're done!

What about a more reflective material?
Phong is the specular shader that we'll implement

h = (v + l) / ||v + l|| (l in incident light, v is exitant)
L_s = k_s * I * max (0, n.h)^n (n is a parameter we can control on the material)

L_a = k_a * I_a (this is ambient light, makes the black shadows look more real)

L_a is once per intersection hit, L_d and L_s are once per light per intersection point (assuming nothing is blocked)

Checking if a light ray is blocked: Fire in the direction to the light source (will either be calculated for non-directional lights or just the reverse of the directional light) and see if it collides with _anything_

Super simple!

Replace I with I / r2 where r is the radius of the circle the collision point makes with the centre of the light (this defines light falloff) - only for point lights!
    We'll call these spot lights, they're a position in the world, with the I is divided by r^2.
    
    