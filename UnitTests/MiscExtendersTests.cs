using FluentAssertions;
using SetCodeHeaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class MiscExtendersTests
    {
        [TestMethod]
        [DataRow(FileKind.CS, "", "")]
        [DataRow(FileKind.CS, " ", "")]
        [DataRow(FileKind.CS, "aaa", "aaa")]
        [DataRow(FileKind.CS, "aaa ", "aaa ")]
        [DataRow(FileKind.CS, " aaa", "aaa")]
        [DataRow(FileKind.CS, "//aaa", "")]
        [DataRow(FileKind.CS, "// aaa", "")]
        [DataRow(FileKind.CS, " //aaa", "")]
        [DataRow(FileKind.CS, " //aaa//bbb", "")]
        [DataRow(FileKind.CS, " //aaa// bbb", "")]
        [DataRow(FileKind.CS, "//aaa\nbbb", "bbb")]
        [DataRow(FileKind.CS, "//aaa\nbbb\n//ccc", "bbb\n//ccc")]
        [DataRow(FileKind.XML, "<!--aaa-->", "")]
        [DataRow(FileKind.XML, "<!--aaa-->bbb", "bbb")]
        [DataRow(FileKind.XML, "<!--aaa-->\nbbb", "bbb")]
        [DataRow(FileKind.XML, "<!--aaa--><!--ccc-->", "<!--ccc-->")]
        [DataRow(FileKind.XML, "<!--aaa-->\n<!--ccc-->", "<!--ccc-->")]
        [DataRow(FileKind.XML, "<!--aaa-->\nbbb<!--ccc-->", "bbb<!--ccc-->")]
        [DataRow(FileKind.XML, "<!--aaa-->\nbbb\n<!--ccc-->", "bbb\n<!--ccc-->")]
        public void WithoutHeaderWithGoodArgs(
            FileKind fileKind, string oldText, string expected)
        {
            oldText.WithoutHeader(fileKind).Should().Be(expected);
        }
    }
}
