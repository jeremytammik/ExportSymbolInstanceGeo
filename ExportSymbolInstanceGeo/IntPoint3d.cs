using System;
using Autodesk.Revit.DB;

namespace ExportSymbolInstanceGeo
{
  /// <summary>
  /// An integer-based 3D point class.
  /// </summary>
  class IntPoint3d : IComparable<IntPoint3d>, 
    IEquatable<IntPoint3d>
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    /// <summary>
    /// Initialise a 2D millimetre integer 
    /// point to the given values.
    /// </summary>
    public IntPoint3d( int x, int y, int z )
    {
      X = x;
      Y = y;
      Z = z;
    }

    /// <summary>
    /// Convert a 2D Revit UV to a 3D millimetre 
    /// integer point by scaling from feet to mm.
    /// </summary>
    public IntPoint3d( UV p )
    {
      X = Util.FootToMmInt( p.U );
      Y = Util.FootToMmInt( p.V );
      Z = 0;
    }

    /// <summary>
    /// Convert a 3D Revit XYZ to a 3D millimetre 
    /// integer point, scaling from feet to mm.
    /// </summary>
    public IntPoint3d( XYZ p )
    {
      X = Util.FootToMmInt( p.X );
      Y = Util.FootToMmInt( p.Y );
      Z = Util.FootToMmInt( p.Z );
    }

    /// <summary>
    /// Convert Revit coordinates XYZ to a 3D 
    /// millimetre integer point by scaling 
    /// from feet to mm.
    /// </summary>
    public IntPoint3d( double x, double y, double z )
    {
      X = Util.FootToMmInt( x );
      Y = Util.FootToMmInt( y );
      Z = Util.FootToMmInt( z );
    }

    /// <summary>
    /// Comparison with another point, important
    /// for dictionary lookup support, implements 
    /// IComparable<>.
    /// </summary>
    public int CompareTo( IntPoint3d a )
    {
      int d = X - a.X;

      if( 0 == d )
      {
        d = Y - a.Y;

        if( 0 == d )
        {
          d = Z - a.Z;
        }
      }
      return d;
    }

    /// <summary>
    /// Equality predicate, implements IEquatable<>.
    /// </summary>
    public bool Equals( IntPoint3d other )
    {
      return 0 == CompareTo( other );
    }

    /// <summary>
    /// Return hash code. 
    /// When overriding equality, you should always 
    /// have a matching Equals() and GetHashCode() 
    /// (i.e. for two values, if Equals() returns true
    /// they must return the same hash-code, but the 
    /// converse is not required).
    /// https://stackoverflow.com/questions/720177/default-implementation-for-object-gethashcode
    /// </summary>
    public override int GetHashCode()
    {
      // disable overflow, for the unlikely possibility 
      // that you are compiling with overflow-checking 
      // enabled

      unchecked
      {         
        int hash = 27;
        hash = (13 * hash) + X.GetHashCode();
        hash = (13 * hash) + Y.GetHashCode();
        hash = (13 * hash) + Z.GetHashCode();
        return hash;
      }
    }

    /// <summary>
    /// Return as a comma-separated string enclosed in
    /// square brackets, i.e., a valid JSON array.
    /// </summary>
    public string ToJsonCoords()
    {
      return string.Format( "[{0},{1},{2}]", X, Y, Z );
    }

    /// <summary>
    /// Display as a string.
    /// </summary>
    public override string ToString()
    {
      return string.Format( "({0},{1},{2})", X, Y, Z );
    }

    /// <summary>
    /// Display as a string.
    /// </summary>
    public string ToString(
      bool onlySpaceSeparator )
    {
      string format_string = onlySpaceSeparator
        ? "{0} {1} {2}"
        : "({0},{1},{2})";

      return string.Format( format_string, X, Y, Z );
    }

    /// <summary>
    /// Add two points, i.e. treat one of 
    /// them as a translation vector.
    /// </summary>
    public static IntPoint3d operator +(
      IntPoint3d a,
      IntPoint3d b )
    {
      return new IntPoint3d(
        a.X + b.X, a.Y + b.Y, a.Z + b.Z );
    }
  }
}
