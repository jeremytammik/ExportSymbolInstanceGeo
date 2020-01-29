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
[assembly: AssemblyCopyright( "Copyright 2019-2020 (C) Jeremy Tammik, Autodesk Inc." )]
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
// 2019-12-04 2020.0.0.1 split symbol transform into rotation and translation
// 2019-12-05 2020.0.0.2 added symbol unique id to json output
// 2019-12-11 2020.0.0.3 added regions and comments
// 2019-12-11 2020.0.0.3 suppress symbol triangles and transformation when nested
// 2019-12-11 2020.0.0.4 append to output file for testing purposes
// 2019-12-11 2020.0.0.4 implemented and tested nested families
// 2020-01-22 2020.0.1.0 started working on iteration number two
// 2020-01-22 2020.0.1.0 clarified output filenames
// 2020-01-29 2020.0.1.0 added call to ExporterIFCUtils.UsesInstanceGeometry
// 2020-01-29 2020.0.1.1 removed all line and curve stuff, rerwrote to retain solid-face-triangle hierarchy
// 2020-01-29 2020.0.1.1 implemented ToJsonCoords for points and triangles
// 2020-01-29 2020.0.1.1 renamed to JtTriangle
// 2020-01-29 2020.0.1.1 implemented InstanceMeshesJson and InstanceSolidsJson
// 2020-01-29 2020.0.1.1 implemented first test output with instance inline vertex coordinates of solid and mesh geometry
// 2020-01-29 2020.0.1.1 removed extraneous curly braces
// 2020-01-29 2020.0.1.1 instance geometry export looks good
// 2020-01-29 2020.0.1.2 implemented output of symbol geometry and transformation
// 2020-01-29 2020.0.1.2 successful test of family symbol export, fixed assertion
// 2020-01-29 2020.0.1.3 split symbol geometry and instance-symbol relationship into separate files
//
[assembly: AssemblyVersion( "2020.0.1.3" )]
[assembly: AssemblyFileVersion( "2020.0.1.3" )]
