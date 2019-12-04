using System.Collections.Generic;

namespace ExportSymbolInstanceGeo
{
  /// <summary>
  /// A vertex lookup class to eliminate 
  /// duplicate vertex definitions.
  /// </summary> 
  class IntVertexLookup : Dictionary<IntPoint3d, int>
  {
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
