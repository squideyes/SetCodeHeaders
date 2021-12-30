// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)

// This file is part of SetCodeHeaders

// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

// This file is part of SetCodeHeaders

// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************
namespace SetCodeHeaders
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string LicenseHeaderString = "5860a97e-ed6c-470f-acc8-d420ef831d7c";
        public static Guid SetCodeHeaders = new Guid(LicenseHeaderString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int MyMenuGroup = 0x0001;
        public const int MyCommand = 0x0100;
    }
}