# ExportSymbolInstanceGeo

Export selected element symbol and instance geometry triangles.

## Task

Export element triangles.

Given the currently selected element, export exhaustive information about its geometry to JSON, e.g., to `element.json`, like this:

```
{ "element_id": "d7a856d0-2e09-4ca6-83b0-9e7a18885b28-0012c5cf-1",
  "solids": [ { "faces": [ { "triangles": [ {
  "/": "these are the 3 coordinates of the 3 vertices of the triangle. The order doesn't matter as long as its consistent",
  "coords": [10, 11, 12, 1, 2, 3, 21, 22, 23] }, { "coords": [14, 11, 12, 1, 2, 3, 21, 22, 23] } ] } ] } ] }
```

The same json needs to be exported from the `FamilySymbol` to another file, e.g., `family_symbol.json`, with some translation info, e.g., `translation.json`, like this:

```
{ "rotation_matrix": [1,0,0,0,1,0,0,0,1], "translation": [1,2,3], ... }
```

such that, given family_symbol.json and translation.json, we can recreate element.json.

Note that we need to support all types of faces (just triangulate them).

## Discussion

So, we implement the export of both element geometry and the transformed family definition geometry, all of the files mentioned above?

So, we export all types of faces, triangulated.

How about curves? Do they need to be supported at all, or can they just be ignored? &ndash; let's ignore them for now.

I would suggest some improvements to the above:

In elements.json, i would store the coordinates in a separate list, and reference that from the triangles. that saves repeating the same coordinates three or more times for different triangles.

Besides family_symbols.json and translation.json, we need a family_instances.json that refers to them both and says family instance element id X makes use of family symbol Y with translation Z.

In fact, I would store all the required information in one single file, containing:

a list of all XYZ points used
a list of all family symbols with their id and geometry definition, i.e., triangles.
a list of all family instances with their id, family symbol id and transform.

The entire solution needs to work only for the currently selected element.

Note that this entire task is only a proof of concept.

It will obviously need to be extended further to be used in production scenarios.

Saving the list of vertices separately is useful in two ways: it saves space, and, more importantly, helps ensure that the vertices and coordinates and triangles are really consistent.
It is actually easier to do that properly and minimally than to repeat the data unnecessarily.

Picking an individual element is a rather spacial case.
For production use, handling all elements in one go using
a [custom exporter](https://thebuildingcoder.typepad.com/blog/about-the-author.html#5.1) makes more sense.

For selected elements, I once implemented
the [tiny WebGL exporter](https://github.com/jeremytammik/TwglExport).

Picking an individual element and accessing its geometry is not the recommended approach to export its triangulated solids, as I point out in the discussions implementing TwglExport:

- [Exporting 3D Element Geometry to a WebGL Viewer](https://thebuildingcoder.typepad.com/blog/2015/04/exporting-3d-element-geometry-to-a-webgl-viewer.html)
- [Live Revit Element Rendering in Remote WebGL Viewer](https://thebuildingcoder.typepad.com/blog/2015/04/live-revit-element-rendering-in-remote-webgl-viewer.html)

For a production scenario, one should rather go for the custom exporter solution, not select an individual element.

I also ((over-?) optimisticaly) believe that the custom exporter helps automatically solve the issue of differentiating between family instances that require individual geometry versus those that can reuse the symbol geometry.

