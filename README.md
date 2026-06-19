# Godot Geometry Primitives and Intersection Tests

This is a small Godot project implementing a set of 2D and 3D interesection tests from simple point-in-box to more complex triangle-AABB tests. The more later tests are built up by reusing the solutions for the earlier tests where possibles, to make them more understandable.

## Structure

```
Debug : debug line drawing
Geo2D : 2D primitives
Geo3D : 3D primitives
  Debug : intersection test debugging scripts
  Draw : primitive drawing helper scripts
```

## Intersection tests

_2D:_

- point : rectangle
- rectangle : rectangle
- edge : rectangle
- point : triangle
- triangle : rectangle

_3D:_

- point : aabb
- aabb : aabb
- ray : aabb
- edge : aabb
- ray : triangle
- edge : triangle
- triangle : aabb (see below)
- plane : aabb
- edge : plane

The focus of these implementations has been on algorithmic optimisations and keeping them as clear and readable as I can, rather than tight code optimisations. It is certainly possible to improve the performance of the tests by writing them in a language which provides more control of code generation, batching multiple tests together or exploiting redundancy in successive hierarchical tests.

I've included implementations of some of the standard algorithms, such as the Schwarz-Seidel triangle:AABB test and the Moller-Trumbore ray:triangle test.

### Triangle:AABB Intersection Test

In addition I've implemented a new triangle:AABB solution which takes a different approach to the Schwarz-Seidel test. It has four stages:

1. Check for intersection between the AABB and the bounds of the triangle. Exit early if disjoint.
2. Test each of the three triangle edges against the AABB. Exit early with an intersection.
3. Check for intersection between the plane of triangle and the AABB. Exit if disjoint.
4. Test each of the four internal diagonal axes of the AABB against the triangle, for the cases where the box intersects the face of the triangle without touching any of its edges.

The Schwarz-Seidel algorithm can only exit early in situations where the shapes are disjoint, so finding intersecting shapes requires all the individual tests to be checked. The new approach inverts this logic; after an initial broad-phase rejection using bounding boxes, it can return as soon as one of the potential intersection cases is met.

This means that it is significantly more efficient if the domain is mostly made up of intersecting shapes, which is often the case when performing a series of hierarchial tests such as with the generation of Sparse Voxel Octrees from triangle meshes.

If the domain is predominantly disjoint shape queries then performance of the two solutions is very similar, since the bounding box check is very effective in filtering these out early.

### Primitive Rendering

Scripts are supplied for rendering the primitives in the viewport, using the debug line renderer described below.

### Intersection Debugging

In order to test the correctness of the intersection tests I've included a set of test scripts to visualise the results.  This makes it easy to position the primitives in corner cases such as near-parallel alignments or glancing angles.

## Debug Line Rendering

Godot doesn't have any debug line rendering available in the editor so I've included a singleton class which fills this gap.  It was originally based on the work https://github.com/GeneralProtectionFault/Godot-3D-Lines-CSharp  But where that implementation only works in a running game, this version works in the editor directly.  It also clips the lines correctly when they cross the near clip plane of the camera and to the borders of the subviewport.

I've kept the clipping code for the debug line rendering separate from the other primitive tests so that there aren't any circular dependencies with the rest of th code.  These classes can be used independently.
