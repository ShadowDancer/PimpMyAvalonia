using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PimpMyAvalonia.LanguageServer.Tests.UnitTests
{
    public class AddPositionTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 5)]
        [InlineData(1, 0)]
        [InlineData(1, 5)]
        public void EmptyStringDoesNotChangePosition(int line, int chars)
        {
            var start = new Position(line, chars);
            var newPos = Utils.AddPosition(start, "");
            Assert.Equal(start, newPos);
        }

        [Theory]
        [InlineData(0, 0, 3)]
        [InlineData(0, 5, 8)]
        [InlineData(1, 0, 3)]
        [InlineData(1, 5, 8)]
        public void SingleLineText(int line, int chars, int expectedChars)
        {
            var start = new Position(line, chars);
            var newPos = Utils.AddPosition(start, "abc");
            Assert.Equal(new Position(line, expectedChars), newPos);
        }

        [Theory]
        [InlineData(0, 0, 1, 1)]
        [InlineData(0, 5, 1, 1)]
        [InlineData(1, 0, 2, 1)]
        [InlineData(1, 5, 2, 1)]
        public void MultiLineText(int line, int chars, int expectedLine, int expectedChars)
        {
            string charStr3WithNewLine = "a\nb";
            var pos = Utils.AddPosition(new Position(line, chars), charStr3WithNewLine);
            Assert.Equal(new Position(expectedLine, expectedChars), pos);
        }
    }
}
