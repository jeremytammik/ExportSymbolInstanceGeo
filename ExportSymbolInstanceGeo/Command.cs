#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

      const string json_format_str = "\"{0}\" : \"{1}\"";
      const string json_format_arr = "\"{0}\" : [{1}]";

      List<string> lines = new List<string>( n );

      lines.Add( string.Format( json_format_str, 
        "element_uid", e.UniqueId ) );

      lines.Add( string.Format( json_format_arr,
        "coords", triangulator.VertexCoordinates ) );

      lines.Add( string.Format( json_format_arr, 
        "instance_triangles", triangulator.InstanceTriangleIndices ) );

      if( triangulator.HasSymbol )
      {
        Debug.Assert( e is FamilyInstance, 
          "expected only family instance to have symbol geometry" );

        lines.Add( string.Format( json_format_str, "symbol_uid", 
          (e as FamilyInstance).Symbol.UniqueId ) );

        lines.Add( string.Format( json_format_arr, 
          "symbol_rotation", triangulator.SymbolRotation ) );

        lines.Add( string.Format( json_format_arr, 
          "symbol_translation", triangulator.SymbolTranslation ) );

        lines.Add( string.Format( json_format_arr, 
          "symbol_triangle_indices", triangulator.SymbolTriangleIndices ) );
      }

      using( StreamWriter s = new StreamWriter( _filepath ) )
      {
        string a = "{\r\n" 
          + string.Join( ",\r\n", lines ) 
          + "\r\n}";

        s.Write( a );
        s.Close();
      }
      return Result.Succeeded;
    }
  }
}
