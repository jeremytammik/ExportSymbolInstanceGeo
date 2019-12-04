using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "ExportSymbolInstanceGeo" )]
[assembly: AssemblyDescription( "Revit add-in to export selected element symbol and instance geometry triangles" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Autodesk Inc." )]
[assembly: AssemblyProduct( "ExportSymbolInstanceGeo Revit C# .NET Add-In" )]
[assembly: AssemblyCopyright( "Copyright 2019 (C) Jeremy Tammik, Autodesk Inc." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "321044f7-b0b2-4b1c-af18-e71a19252be0" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
//
// History:
//
// 2019-12-03 2020.0.0.0 initial skeleton 
// 2019-12-03 2020.0.0.0 started implementing TriangleCollector based on SDK ElementViewer
// 2019-12-04 2020.0.0.0 renamed IntVertexLookup
// 2019-12-04 2020.0.0.0 added IEquatable and GetHashCode
// 2019-12-04 2020.0.0.0 completed TriangleCollector
// 2019-12-04 2020.0.0.0 research on OrderedDictiuonary, added sorted _vertex_list
// 2019-12-04 2020.0.0.0 started implementing json output
// 2019-12-04 2020.0.0.0 implemented SymbolTransform output 
// 2019-12-04 2020.0.0.0 implemented json output 
// 2019-12-04 2020.0.0.0 implemented HasSymbol predicate
// 2019-12-04 2020.0.0.0 implemented InSymbol predicate
// 2019-12-04 2020.0.0.0 properly formatted json and successful first test
//
[assembly: AssemblyVersion( "2020.0.0.0" )]
[assembly: AssemblyFileVersion( "2020.0.0.0" )]
