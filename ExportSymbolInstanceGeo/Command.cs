#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace ExportSymbolInstanceGeo
{
  [Transaction( TransactionMode.ReadOnly )]
  public class Command : IExternalCommand
  {
    /// <summary>
    /// JSON output file path
    /// </summary>
    public const string _filepath
      = "C:/tmp/element_triangles.json";

    /// <summary>
    /// User interface usage prompt string
    /// </summary>
    const string _prompt = "Please select a single " 
      + "element to export its solid triangles.";

#if OBJ_EXPORTER_CODE
    /// <summary>
    /// Export all non-empty solids found for 
    /// the given element. Family instances may have 
    /// their own non-empty solids, in which case 
    /// those are used, otherwise the symbol geometry.
    /// The symbol geometry could keep track of the 
    /// instance transform to map it to the actual 
    /// project location. Instead, we ask for 
    /// transformed geometry to be returned, so the 
    /// resulting solids are already in place.
    /// From ObjExporter,
    /// https://thebuildingcoder.typepad.com/blog/2013/07/revit-2014-obj-exporter-and-new-sdk-samples.html
    /// </summary>
    int ExportSolids(
      IJtFaceEmitter emitter,
      Element e,
      Options opt,
      Color color,
      int shininess,
      int transparency )
    {
      int nSolids = 0;

      GeometryElement geo = e.get_Geometry( opt );

      Solid solid;

      if( null != geo )
      {
        Document doc = e.Document;

        if( e is FamilyInstance )
        {
          geo = geo.GetTransformed(
            Transform.Identity );
        }

        GeometryInstance inst = null;
        //Transform t = Transform.Identity;

        // Some columns have no solids, and we have to
        // retrieve the geometry from the symbol; 
        // others do have solids on the instance itself
        // and no contents in the instance geometry 
        // (e.g. in rst_basic_sample_project.rvt).

        foreach( GeometryObject obj in geo )
        {
          solid = obj as Solid;

          if( null != solid
            && 0 < solid.Faces.Size
            && ExportSolid( emitter, doc, solid,
              color, shininess, transparency ) )
          {
            ++nSolids;
          }

          inst = obj as GeometryInstance;
        }

        if( 0 == nSolids && null != inst )
        {
          geo = inst.GetSymbolGeometry();
          //t = inst.Transform;

          foreach( GeometryObject obj in geo )
          {
            solid = obj as Solid;

            if( null != solid
              && 0 < solid.Faces.Size
              && ExportSolid( emitter, doc, solid,
                color, shininess, transparency ) )
            {
              ++nSolids;
            }
          }
        }
      }
      return nSolids;
    }

    void ExportSymbolInstanceGeo( Element e )
    {

    }
#endif

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Application app = uiapp.Application;
      Document doc = uidoc.Document;
      Selection sel = uidoc.Selection;
      ICollection<ElementId> ids = sel.GetElementIds();
      int n = ids.Count;
      Element e = null;

      if( 1 < n )
      {
        message = _prompt;
        return Result.Failed;
      }
      else if( 1 == n )
      {
        e = doc.GetElement( ids.First() );
      }
      else
      {
        try
        {
          Reference r = sel.PickObject(
            ObjectType.Element, _prompt );

          e = doc.GetElement( r.ElementId );
        }
        catch( OperationCanceledException )
        {
          return Result.Cancelled;
        }
      }

      TriangleCollector triangulator 
        = new TriangleCollector();

      triangulator.DrawElement( e );

      //JavaScriptSerializer serializer
      //  = new JavaScriptSerializer();
      //lines.Add( serializer.Serialize( data ) );
      //File.AppendAllLines( FilePath, lines );

      List<string> lines = new List<string>( n );

      lines.Add( string.Format( "\"unique_id\" : \"{0}\"", e.UniqueId ) );
  
      using( StreamWriter s = new StreamWriter( 
        _filepath ) )
      {
        s.WriteLine( string.Format( "\"unique_id\" : \"{0}\"", e.UniqueId ) );

        string vertices = triangulator.VertexCoordinates;
        string instance_triangleIndices = triangulator.InstanceTriangleIndices;
        string symbol_triangleIndices = triangulator.SymbolTriangleIndices;

        //{ "element_id": "d7a856d0-2e09-4ca6-83b0-9e7a18885b28-0012c5cf-1",
        //  "solids": [ { 
        //    "faces": [ { 
        //      "triangles": [ { 
        //        "/": "these are the 3 coordinates of the 3 vertices of the triangle. The order doesn't matter as long as its consistent", 
        //        "coords": [10, 11, 12, 1, 2, 3, 21, 22, 23] }, 
        //      { "coords": [14, 11, 12, 1, 2, 3, 21, 22, 23] 
        //      } ] 
        //    } ] 
        //  } ] 
        //}

        //s.WriteLine( caption );

        //List<int> keys = new List<int>( loops.Keys );
        //keys.Sort();
        //foreach( int key in keys )
        //{
        //  ElementId id = new ElementId( key );
        //  Element e = doc.GetElement( id );

        //  s.WriteLine(
        //    "{{\"name\":\"{0}\", \"id\":\"{1}\", "
        //    + "\"uid\":\"{2}\", \"svg_path\":\"{3}\"}}",
        //    e.Name, e.Id, e.UniqueId,
        //    loops[ key ].SvgPath );
        //}
        s.Close();
      }


      return Result.Succeeded;
    }
  }
}
