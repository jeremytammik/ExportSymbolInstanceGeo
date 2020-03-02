# ExportSymbolInstanceGeo

Revit C# .NET add-in that exports selected element symbol and instance geometry triangles.

Overall goal: export `FamilyInstance` geometry as `FamilySymbol` geometry plus translation.

- [Task &ndash; First Iteration](#1.1)
- [Discussion of First Iteration](#1.2)
- [No Nested Families](#1.3)
- [Handling Nested Families](#1.4)
- [Task &ndash; Second Iteration](#2)
- [Second Iteration Result](#2.1)
- [Second Iteration Explanation](#2.2)


## <a name="1.1"></a>Task &ndash; First Iteration

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

## <a name="1.2"></a>Discussion of First Iteration

So, we implement the export of both element geometry and the transformed family definition geometry, all of the files mentioned above?

So, we export all types of faces, triangulated.

How about curves? Do they need to be supported at all, or can they just be ignored? &ndash; let's ignore them for now.

I would suggest some improvements to the above:

In elements.json, i would store the coordinates in a separate list, and reference that from the triangles. that saves repeating the same coordinates three or more times for different triangles.

Besides family_symbols.json and translation.json, we need a family_instances.json that refers to them both and says family instance element id X makes use of family symbol Y with translation Z.

In fact, I would store all the required information in one single file, containing:

- a list of all XYZ points used
- a list of all family symbols with their id and geometry definition, i.e., triangles.
- a list of all family instances with their id, family symbol id and transform.

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


## <a name="1.3"></a>No Nested Families

The first version of the per-element exporter is completed and tested in release 2020.0.0.1.

It only supports symbols one level deep.

It has assertions built in that fire if you try to use it on more nested family definitions.

We will have to completely reconsider and rethink the situation if that case needs to be handled also.

## <a name="1.4"></a>Handling Nested Families

Here is an idea on how to handle nested family instances:

Only support symbol definitions one level deep.

Any symbols that are nested within a top-level symbol are subsumed into the top-level symbol.
No nested symbol geometry is managed.
The nested symbol geometry is transformed up to the top-level symbol.

One complication is that this may create several different versions of the top-level symbol, and these need to be told apart.

For instance, imagine we have a door family D equipped with different nested door handles H.
Further, assume D defines a type == symbol D1, and H defines two types H1 and H2.
Assume the project contains three instances: D1 with H1 and D1 with H2.
This will generate two symbol definitions for D1.
How do we identify them, how to tell them apart?


## <a name="2"></a>Task &ndash; Second Iteration

Let's make a new start based on the experience gathered from the first attempt.

The goal is to export the geometric information about a specific element.

The command is launched by selecting a specific FamilyInstance and clicking "export geometry".

Export 3 files (suggested format attached):

- [instance_geometry.json](example/instance_geometry.json) &ndash;
exhaustive information regarding the geometry of the instance. See the attached
[example_export.cs](example/example_export.cs) for suggested traversal code. 
- [symbol_geometry.json](example/symbol_geometry.json) &ndash;
exhaustive information regarding the general family symbol
- [instance_transform.json](example/instance_transform.json) &ndash;
information regarding the translation of the specific element to reach its

Existing files are appended to, not overwerittten.

Also implement code that can take `family_symbol_geometry` + `family_instance_translation_info` and create the `family_instance_geometry` file from it.

Notes:

- If the instance has nested sub-elements, skip it.
- Work only on the currently selected element (obviously, it will be modified to work on all elements later).
- Need to support all types of faces (just triangulate them using `face.Triangulate(LevelOfDetail)`).
- No need to support other types of geometries (PolyLine, Point, Curve, ...). Just Solid and Mesh.
- Please do not export the vertices separately from their indices. Although I understand the value of that, it makes the code less clear at this point.
- The code should work for elements in linked models as well.

## <a name="2.1"></a>Second Iteration Result

For a selected element, the external command now successfully exports three files:

- [instance_geometry.json](test/instance_geometry.json)
- [instance_transform.json](test/instance_transform.json)
- [symbol_geometry.json](test/symbol_geometry.json)

The latter two are only generated for `FamilyInstance` elements that make use of unmodified symbol geometry.

The latter-most is completely independent of the selected family instance.
Different family instances using the same symbol generate identical copies of `symbol_geometry.json`.

The JSON format has been simplified and streamlined in all three to represent all 3D points, vectors and matrix columns as proper JSON subarrays, and remove the superfluous dictionary structure within the solid list of faces and face list of triangles.
They are now just stored as simple lists, not dictionaries.

A Python test script reads the three output files and tests that the instance transform applied to the symbol geometry produces the exact same  instance geometry, within the integer-based millimetre coordinates' rounding precision:

- [transform_symbol_to_instance.py](test/transform_symbol_to_instance.py)

It makes use of
the [Pyrr 3D mathematical functions using NumPy](https://github.com/adamlwgriffiths/Pyrr) for
matrix multiplication to implement the required transformations.

## <a name="2.2"></a>Second Iteration Explanation

I am still confused. Here are some questions that come to mind that might help me understand better:

1. What is the difference between the following two?

``` 
    GeometryInstance geoInst;
    GeometryElement geoElem = geoInst.SymbolGeometry;
```

And:

``` 
    GeometryInstance geoInst;  
    GeometryElement geoElem = geoInst.GetSymbolGeometry();
```

2. I read the remarks in the documentation on
the [GeometryInstance class](https://www.revitapidocs.com/2020/fe25b14f-5866-ca0f-a660-c157484c3a56.htm);
does it mean that sometimes an element has an underlying `GeometryInstance`, but might anyway be represented only of Solids/Meshes/...?
 
3. When getting the `GeometryElement` of multiple different elements using `inst.SymbolGeometry`, how can I know that they are the same, and avoid exporting them both, but only export one of them, adding a translation for each element separately?
 
4. You don't use the `FamilyInstance.Symbol` field to reach the original symbol, just directly through geometries, so you don't even need to check whether it's a `FamilyInstance` or not. Do you think one method is strictly better than the other?
 
5. I can also traverse the geometry using `GetInstanceGeometry` instead of `SymbolGeometry`. Why don't you?

P.S - I think some of the questions have the same answers... yet, it's better to ask :)
 
Answers:

I think the remark you pointed out says it all:
 
> A GeometryInstance represents a set of geometry stored by Revit in a default configuration, and then transformed into the proper location as a result of the properties of the element. The most common situation where GeometryInstances are encountered is in Family instances. Revit uses GeometryInstances to allow it to store a single copy of the geometry for a given family and reuse it in multiple instances. Note that not all Family instances will include GeometryInstances. When Revit needs to make a unique copy of the family geometry for a given instance (because of the effect of local joins, intersections, and other factors related to the instance placement) no GeometryInstance will be encountered; instead the Solid geometry will be found at the top level of the hierarchy. A GeometryInstance offers the ability to read its geometry through the GetSymbolGeometry() and GetInstanceGeometry() methods. These methods return another Autodesk.Revit.DB.GeometryElement which can be parsed just like the first level return.
 
1: difference between SymbolGeometry and GetSymbolGeometry?
 
As far as I know, they are equivalent, except that the 'get' method enables you to specify a transform to the geometry retrieved.
 
2: can an element sometimes have underlying GeometryInstances, but might be structured of Solids/Meshes/... only?
 
I would not use that phrasing. More precisely, I would ask: can an element be a family instance yet not make use of any GeometryInstance for its representation?
 
Afaict the answer is yes. If you wish, I can create a test case to prove it.

3: When using inst.SymbolGeometry on several elements, how can I know that they are the same?
 
Afaik, and according to the description above, the symbol geometry is always the unmodified geometry specified by the family definition. If the element needs something different, a unique adaptation is created for it, cf. description above.
 
4: you don't use `FamilyInstance.Symbol` to reach the original symbol, just directly through geometries (so you don't even need to check whether it's a FamilyInstance or not). Do you think one method is strictly better than the other?
 
Yes, I think going straight through the geometries is better, because if I check for a symbol, that does not answer the question on whether the instance is actually making use of the original unmodified family symbol geometry or not; I still need to perform the check on the geometries anyway.
 
5: GetInstanceGeometry returns the geometry in model coordinates. SymbolGeometry returns them in symbol coordinates. If we wish to share the symbol geometry between multiple instances, we need the symbol geometry in symbol coordinates plus the transform into the model space. A different transform for each instance. Same symbol geometry for each instance. That's the point.
 

## <a name="author"></a>Author

Jeremy Tammik, [The Building Coder](http://thebuildingcoder.typepad.com), [ADN](http://www.autodesk.com/adn) [Open](http://www.autodesk.com/adnopen), [Autodesk Inc.](http://www.autodesk.com)


## <a name="license"></a>License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.

