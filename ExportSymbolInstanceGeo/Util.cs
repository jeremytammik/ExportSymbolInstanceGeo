
using Autodesk.Revit.DB;
using System;

namespace ExportSymbolInstanceGeo
{
  class Util
  {
    #region Unit Handling
    const double _inchToMm = 25.4;
    const double _footToMm = 12 * _inchToMm;
    const double _footToMeter = _footToMm * 0.001;
    const double _sqfToSqm = _footToMeter * _footToMeter;
    const double _cubicFootToCubicMeter = _footToMeter * _sqfToSqm;

    /// <summary>
    /// Convert a given length in feet to millimetres.
    /// </summary>
    public static double FootToMm( double length )
    {
      return length * _footToMm;
    }

    /// <summary>
    /// Convert a given length in feet to millimetres,
    /// rounded to the closest millimetre.
    /// </summary>
    public static int FootToMmInt( double length )
    {
      //return (int) ( _feet_to_mm * d + 0.5 );
      return (int) Math.Round( _footToMm * length,
        MidpointRounding.AwayFromZero );
    }
    #endregion // Unit Handling

  }
}
