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
    /// JSON output file directory and paths
    /// </summary>
    const string _ext = ".json";
    const string _dir = "C:/tmp/";
    const string _fn_instance_geo = _dir + "instance_geometry" + _ext;
    const string _fn_symbol_geo = _dir + "symbol_geometry" + _ext;
    const string _fn_instance_xform = _dir + "instance_transform" + _ext;

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
        = new TriangleCollector( e );

      const string json_format_str = "\"{0}\" : \"{1}\"";
      //const string json_format_arr = "\"{0}\" : [{1}]";

      List<string> lines = new List<string>();

      // Export instance solid face and triangle coordinates

      lines.Add( string.Format( json_format_str, 
        "element_uid", e.UniqueId ) );

      lines.Add( triangulator.InstanceMeshesJson );
      lines.Add( triangulator.InstanceSolidsJson );

      using( StreamWriter s = new StreamWriter(
        _fn_instance_geo, true ) )
      {
        string a = "{\r\n"
          + string.Join( ",\r\n", lines )
          + "\r\n}";

        s.Write( a );
        s.Close();
      }

      /*
            if( triangulator.HasSymbol )
            {
              Debug.Assert( e is FamilyInstance, 
                "expected only family instance to have symbol geometry" );

              lines.Add( string.Format( json_format_str, "symbol_uid", 
                (e as FamilyInstance).Symbol.UniqueId ) );

              if( triangulator.IsNested )
              {
                lines.Add( string.Format( json_format_str,
                "symbol_is_nested", "true" ) );
              }
              else
              {
                lines.Add( string.Format( json_format_arr,
                "symbol_rotation", triangulator.SymbolRotation ) );

                lines.Add( string.Format( json_format_arr,
                  "symbol_translation", 
                  triangulator.SymbolTranslation ) );

                lines.Add( string.Format( json_format_arr,
                  "symbol_triangle_indices", 
                  triangulator.SymbolTriangleIndices ) );
              }
            }
            */

      return Result.Succeeded;
    }
  }
}
