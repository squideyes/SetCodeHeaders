// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SetCodeHeaders
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SetCodeHeaders;

namespace UnitTests
{
    [TestClass]
    public class MiscExtendersTests
    {
        [TestMethod]
        [DataRow("", "")]
        [DataRow(" ", "")]
        [DataRow("a", "a")]
        [DataRow(" a", " a")]
        [DataRow("a ", "a ")]
        [DataRow("//a", "")]
        [DataRow("//a\nb", "b")]
        [DataRow("\n//a\nb", "b")]
        [DataRow("\n//a\n\nb", "b")]
        [DataRow("//a\nb\n//c", "b\n//c")]
        [DataRow("\n//a\nb\n//c", "b\n//c")]
        [DataRow("//a\n\n\nb\n//c", "b\n//c")]
        [DataRow("//a\n\n\nb\n\n//c", "b\n\n//c")]
        public void WithoutHeaderWithGoodArgs(string oldText, string expected)
        {
            var oldLines = oldText.ToLines();
            var newLines = oldLines.WithoutHeader();

            newLines.Should().BeEquivalentTo(expected.ToLines());            
        }
    }
}