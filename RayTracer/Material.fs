namespace RayTracer

(*
Materials are tough!

First, some definitions:

Note:
    w represents a particular direction

Radiant Energy (Q):
Measured in joules (J)
The radiant energy carried by each photon is: Q = hc/l where
    h = 6.626 x 10 ^ -34 (Js)
    c = 2.998 x 10 ^ 8 (m/s)
    l is the wavelength of the photon in meters

Radiant Flux (X):
    Measured in (J/s) AKA (W -> watts)
    Amount of radiant energy that passes through a surface or region of space per second:
    X = dQ / dt
    It is a restriction of radiant energy with respect to time

Radiant Flux Density (Y):
    Radiant flux per unit surface area:
    Y = dX / dA
    where dA is the differential area
    Measured in (W/m^2)
    
    A restriction of radiant energy with respect to time and space

Irradiance (E):
    The flux density that arrives at a surface
    E = dX / dA

Radiant exitance:
    Flux density that leaves a surface
    Same as above

Radiant intensity (I):
    Flux density per unit solid angle.
    I = dX / dw
    It has units of (W / (sr*m^2))
    sr is steradians, the solid angle equivalent of radians

    Only matters for point light sources (so will probably completely skip)
    Restriction of Q with respect to time and direction

Radiance (L):
    Flux per unit projected area per unit solid angle.
    Radiance measures the flux at a point in space, coming from a particular direction.
    It is measured on an imaginary surface that is perpendicular to this direction.

    L = d^2 X / (dA dw)

    We care about the limit as both dw and dA tend to 0 (since this represents an infinitly thin line which is our ray)

Going back to irradiance, since we'll be caring about when it hits a surface we may as well talk about the radiance wrt
this surface rather than the surface of the ray:
    dA = dA' / cos(theta)
    dA' = the area of the ray
    Theta = the angle between the normal and the ray
    Now we can substitute this into our radiance equation:

    L = d^2 X / (dA * cos(theta) * dw)
*)

