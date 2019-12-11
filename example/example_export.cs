Options opt = new Options { IncludeNonVisibleObjects = true, DetailLevel = ViewDetailLevel.Fine };
GeometryElement geomElem = element.get_Geometry(opt);
foreach (GeometryObject geomObj in geomElem)
{
    switch (geomObj)
    {
        case GeometryInstance geoInst:
        {
            using (GeometryElement geoElem2 = geoInst.GetInstanceGeometry())
            {
                foreach (GeometryObject geoObj2 in geoElem2)
                {
                    switch (geoObj2)
                    {
                        case Solid solid:
                            handleSolid(solid);
                            break;
                        case Mesh m:
                            handleMesh(m);
                            break;
                        // other cases... Curve, Point, PolyLine, ...
                    }
                }
            }
            break;
        }
        // QUESTION: How can these cases happen? why aren't these geometries part of the InstanceGeometry above?
        // Will they be part of the SymbolGeometry if I use GetSymbolGeometry instead of GetInstanceGeometry
        case Solid geomSolid:
            handleSolid(geomSolid);
            break;
        case Mesh m:
            handleMesh(m)
            break
        // other cases... Curve, Point, PolyLine, ...
    }
}