using System.Collections.Generic;
using System.Linq;

namespace ExportSymbolInstanceGeo
{
  /// <summary>
  /// A vertex lookup class to eliminate 
  /// duplicate vertex definitions, cf.
  /// https://softwareengineering.stackexchange.com/questions/16260/why-is-there-no-generic-implementation-of-ordereddictionary-in-net
  /// </summary> 
  class IntVertexLookup : Dictionary<IntPoint3d, int>
  {
    List<IntPoint3d> _vertex_list
      = new List<IntPoint3d>();

    /// <summary>
    /// Return the index of the given vertex,
    /// adding a new entry if required.
    /// </summary>
    public int Add( IntPoint3d p )
    {
      if( !ContainsKey(p))
      {
        _vertex_list.Add( p );
        this[ p ] = Count;
      }
      return this[ p ];
    }

    public IntPoint3d this[ int i ]
    {
      get
      {
        return _vertex_list[i];
      }
    }

    //public List<IntPoint3d> VerticesInOrder
    //{
    //  get { return _vertex_list; }
    //}

    public string Coordinates
    {
      get
      {
        return string.Join( " ", 
          _vertex_list.Select<IntPoint3d, string>( 
            p => p.ToString( true ) ) );
      }
    }
  }
}
