using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PimpMyAvalonia.LanguageServer.Tests.UnitTests
{
    public class OffsetToPositionTests
    {
        [Fact]
        public void EmptyString_Returns0()
        {
            var pos = Utils.OffsetToPosition(0, "");
            Assert.Equal(new Position(0, 0), pos);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(9, 9)]
        public void SingleLineTests(int offset, int exprectedChar)
        {
            string data = "0123456789";
            var pos = Utils.OffsetToPosition(offset, data);
            Assert.Equal(new Position(0, exprectedChar), pos);
        }

        [Theory]
        [InlineData("\r", 1, 0)]
        [InlineData("\n", 1, 0)]
        [InlineData("\rn", 1, 0)]
        [InlineData("\rn", 2, 1)]
        [InlineData("\r\nn", 1, 0)]
        [InlineData("\r\nn", 2, 0)]
        [InlineData("\r\nn", 3, 1)]
        [InlineData("\rstuff", 4, 3)]
        public void NewLineCharactersSupport(string data, int offset, int expectedChar)
        {
            var pos = Utils.OffsetToPosition(offset, data);
            Assert.Equal(new Position(1, expectedChar), pos);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(4, 0, 4)]
        [InlineData(5, 1, 0)]
        [InlineData(6, 1, 0)]
        [InlineData(7, 1, 1)]
        [InlineData(8, 2, 0)]
        [InlineData(9, 2, 0)]
        public void MultiLineTests(int offset, int expectedLine, int exprectedChar)
        {
            string data = "01234\r67\n9";
            var pos = Utils.OffsetToPosition(offset, data);
            Assert.Equal(new Position(expectedLine, exprectedChar), pos);
        }
    }
}
