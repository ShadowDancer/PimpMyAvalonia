using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PimpMyAvalonia.LanguageServer.Tests.UnitTests
{
    public class PositionToOffsetTests
    {
        [Fact]
        public void EmptyString_Returns0()
        {
            var pos = Utils.PositionToOffset(new Position(0, 0), "");
            Assert.Equal(0, pos);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(9, 9)]
        public void SingleLineTests(int character, int expectedPos)
        {
            string data = "0123456789";
            var pos = Utils.PositionToOffset(new Position(0, character), data);
            Assert.Equal(expectedPos, pos);
        }

        [Theory]
        [InlineData("\r", 1)]
        [InlineData("\n", 1)]
        [InlineData("\rn", 1)]
        [InlineData("\rstuff", 1)]
        public void NewLineCharactersSupport(string data, int expectedPos)
        {
            var pos = Utils.PositionToOffset(new Position(1, 0), data);
            Assert.Equal(expectedPos, pos);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        public void ConsecutiveNewLines(int line, int expectedPos)
        {
            string data = "\n\n\r\n";
            var pos = Utils.PositionToOffset(new Position(line, 0), data);
            Assert.Equal(expectedPos, pos);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 6, 6)]
        [InlineData(1, 0, 7)]
        [InlineData(1, 4, 11)]
        [InlineData(2, 0, 13)]
        [InlineData(2, 1, 14)]
        public void NewLinesWithText(int line, int character, int expectedPos)
        {
            string data = "tomato\nsauce\nis\r\nawesome";
            var pos = Utils.PositionToOffset(new Position(line, character), data);
            Assert.Equal(expectedPos, pos);
        }
    }
}
