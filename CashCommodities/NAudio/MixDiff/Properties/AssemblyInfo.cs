﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MixDiff")]
[assembly: AssemblyDescription("Mix Comparison Utility")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mark Heath")]
[assembly: AssemblyProduct("MixDiff")]
[assembly: AssemblyCopyright("Copyright © Mark Heath 2006")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("12d6167b-bcab-4b93-b7ae-460fee425dfa")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("0.1.7.0")]
[assembly: AssemblyFileVersion("0.1.7.0")]

// build 1 19 Oct 2006
// initial project created
// build 2 8 Mar 2007
// project added to codeplex
// basic ability to load and play a file
// can switch between files
// build 3 16 Mar 2007
// pausing ability
// fixed a bug with stop
// compare mode added
// initial slot clearing code
// build 4 19 Mar 2007
// more context menu actions
// properties form for offset and volume setting
// build 5 20 Mar 2007
// can save settings to a MixDiff file
// offset is now working
// build 6 22 Mar 2007
// can load from a MixDiff file
// help and about dialogs
// build 7 23 Mar 2007
// comparison file opening bug fixes & enhancements
// beginnings of a shuffle feature

// TODO list - key features for version 1
// 24 bit support
// select WaveOut output device
// configurable skip back amount
// Error handling
// Implement shuffle & reveal feature
// Keyboard support
// Options form to allow selection of WaveOut device
// update WaveOut to allow Init to be called multiple times
// fix the negative time thing
// repositioning drag bar
// status bar


//Extra features:
//MixDiff file should offer relative path support
//master volume
//32 bit file support
//mp3 support
//less important: 8 bit, 64 bit file support
//mismatched sample rate support
