using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DevSpatium.IO.TextReader.Tests
{
    public class TextReaderWrapperTests
    {
        [Theory]
        [InlineData("Qux", @"S.R.", new[] { "u" })]
        [InlineData("Qux", @"S[1]R[1]", new[] { "u" })]
        public async Task ReadAsync_OneChar_OutputContainsCorrectResult(
            string input,
            string pattern,
            string[] expectedOutput)
        {
            // Arrange
            var stringReader = new StringReader(input);
            var output = new List<string>();

            // Act
            using (var textReaderWrapper = new TextReaderWrapper(stringReader))
            {
                await textReaderWrapper.ReadAsync(pattern, output);
            }

            // Assert
            output.Should().Equal(expectedOutput);
        }

        [Theory]
        [InlineData("FooBar", @"S.{3}R.{3}", new[] { "B", "a", "r" })]
        [InlineData("FooBar", @"S[3]R[3]", new[] { "Bar" })]
        public async Task ReadAsync_SeveralChars_OutputContainsCorrectResult(
            string input,
            string pattern,
            string[] expectedOutput)
        {
            // Arrange
            var stringReader = new StringReader(input);
            var output = new List<string>();

            // Act
            using (var textReaderWrapper = new TextReaderWrapper(stringReader))
            {
                await textReaderWrapper.ReadAsync(pattern, output);
            }

            // Assert
            output.Should().Equal(expectedOutput);
        }

        [Theory]
        [InlineData("Foo\r\nBar", @"S>R>", new[] { "Bar" })]
        [InlineData("Foo\r\n\r\nBar", @"S>R>", new[] { "" })]
        [InlineData("Foo\rBar", @"S>R>", new[] { "Bar" })]
        [InlineData("Foo\r\rBar", @"S>R>", new[] { "" })]
        [InlineData("Foo\nBar", @"S>R>", new[] { "Bar" })]
        [InlineData("Foo\n\nBar", @"S>R>", new[] { "" })]
        public async Task ReadAsync_Line_OutputContainsCorrectResult(
            string input,
            string pattern,
            string[] expectedOutput)
        {
            // Arrange
            var stringReader = new StringReader(input);
            var output = new List<string>();

            // Act
            using (var textReaderWrapper = new TextReaderWrapper(stringReader))
            {
                await textReaderWrapper.ReadAsync(pattern, output);
            }

            // Assert
            output.Should().Equal(expectedOutput);
        }

        [Theory]
        [InlineData("FooBarBaz", @"S|'Bar' R+'Bar'", new[] { "Bar" })]
        [InlineData("FooBarBaz", @"R|'Bar' S+'Bar' R>", new[] { "Foo", "Baz" })]
        [InlineData("FooBarBaz", @"S|'Bar' {&R}", new[] { "Bar" })]
        [InlineData("FooBarBaz", @"R|'Bar' {&S} R>", new[] { "Foo", "Baz" })]
        public async Task ReadAsync_MatchingCaseSensitiveString_OutputContainsCorrectResult(
            string input,
            string pattern,
            string[] expectedOutput)
        {
            // Arrange
            var stringReader = new StringReader(input);
            var output = new List<string>();

            // Act
            using (var textReaderWrapper = new TextReaderWrapper(stringReader))
            {
                await textReaderWrapper.ReadAsync(pattern, output);
            }

            // Assert
            output.Should().Equal(expectedOutput);
        }

        [Theory]
        [InlineData("FooBarBaz", @"S|~bar~ R+~bar~", new[] { "Bar" })]
        [InlineData("FooBarBaz", @"R|~bar~ S+~bar~ R>", new[] { "Foo", "Baz" })]
        [InlineData("FooBarBaz", @"S|~bar~ {&R}", new[] { "Bar" })]
        [InlineData("FooBarBaz", @"R|~bar~ {&S} R>", new[] { "Foo", "Baz" })]
        public async Task ReadAsync_MatchingCaseInsensitiveString_OutputContainsCorrectResult(
            string input,
            string pattern,
            string[] expectedOutput)
        {
            // Arrange
            var stringReader = new StringReader(input);
            var output = new List<string>();

            // Act
            using (var textReaderWrapper = new TextReaderWrapper(stringReader))
            {
                await textReaderWrapper.ReadAsync(pattern, output);
            }

            // Assert
            output.Should().Equal(expectedOutput);
        }

        [Theory]
        [InlineData("FooBarBaz", @"S|/[Bb]a./ R+/[Bb]a./", new[] { "Bar" })]
        [InlineData("FooBarBaz", @"R|/[Bb]a./ S+/[Bb]a./ R>", new[] { "Foo", "Baz" })]
        [InlineData("FooBarBaz", @"S|/[Bb]a./ {&R}", new[] { "Bar" })]
        [InlineData("FooBarBaz", @"R|/[Bb]a./ {&S} R>", new[] { "Foo", "Baz" })]
        public async Task ReadAsync_StringMatchingRegex_OutputContainsCorrectResult(
            string input,
            string pattern,
            string[] expectedOutput)
        {
            // Arrange
            var stringReader = new StringReader(input);
            var output = new List<string>();

            // Act
            using (var textReaderWrapper = new TextReaderWrapper(stringReader))
            {
                await textReaderWrapper.ReadAsync(pattern, output);
            }

            // Assert
            output.Should().Equal(expectedOutput);
        }

        //[Theory]
        //[InlineData("FooBarBaz", @"S|['Qux'?'Bar'.........]", new[] {"Bar"})]
        //public async Task ReadAsync_FirstMatchingStringInSequence_OutputContainsCorrectResult(
        //    string input,
        //    string pattern,
        //    string[] expectedOutput)
        //{
        //    // Arrange
        //    var stringReader = new StringReader(input);
        //    var output = new List<string>();

        //    // Act
        //    using (var textReaderWrapper = new TextReaderWrapper(stringReader))
        //    {
        //        await textReaderWrapper.ReadAsync(pattern, output);
        //    }

        //    // Assert
        //    output.Should().Equal(expectedOutput);
        //}
    }
}