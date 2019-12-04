using System.Collections.Generic;

namespace ExportSymbolInstanceGeo
{
  /// <summary>
  /// A vertex lookup class to eliminate 
  /// duplicate vertex definitions.
  /// </summary> 
  class IntVertexLookup : Dictionary<IntPoint3d, int>
  {
    #region IntPoint3dEqualityComparer
    /// <summary>
    /// Define equality for IntPoint3d.
    /// </summary>
    class IntPoint3dEqualityComparer : IEqualityComparer<IntPoint3d>
    {
      public bool Equals( IntPoint3d p, IntPoint3d q )
      {
        return 0 == p.CompareTo( q );
      }

      public int GetHashCode( IntPoint3d p )
      {
        return (p.X.ToString()
          + "," + p.Y.ToString()
          + "," + p.Z.ToString())
          .GetHashCode();
      }
    }
    #endregion // IntPoint3dEqualityComparer

    public IntVertexLookup()
      : base( new IntPoint3dEqualityComparer() )
    {
    }

    /// <summary>
    /// Return the index of the given vertex,
    /// adding a new entry if required.
    /// </summary>
    public int AddVertex( IntPoint3d p )
    {
      return ContainsKey( p )
        ? this[ p ]
        : this[ p ] = Count;
    }
  }
}
