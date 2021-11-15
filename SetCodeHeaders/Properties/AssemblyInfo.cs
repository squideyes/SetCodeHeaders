// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SetCodeHeaders
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SetCodeHeaders;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SetCodeHeaders")]
[assembly: AssemblyDescription("Adds/updates headers for source-code files")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Louis S. Berman")]
[assembly: AssemblyProduct("SetCodeHeaders")]
[assembly: AssemblyCopyright("Copyright 2021 by Louis S. Berman")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion(Vsix.Version)]
[assembly: AssemblyFileVersion(Vsix.Version)]

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}