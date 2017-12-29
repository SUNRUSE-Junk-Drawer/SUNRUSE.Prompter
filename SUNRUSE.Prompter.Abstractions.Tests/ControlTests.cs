// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace SUNRUSE.Prompter.Abstractions.Tests
{
    public sealed class ControlTests
    {
        [Fact, Trait("Type", "Unit")]
        public void ButtonIdDefaultsToNull()
        {
            var control = new Control();

            Assert.Null(control.ButtonId);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void ButtonIdCanBeSetTo(string expected)
        {
            var control = new Control(buttonId: expected);

            var actual = control.ButtonId;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void ButtonIdRoundTripsSerializationWhen(string expected)
        {
            var control = new Control(buttonId: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.ButtonId);
        }

        [Fact, Trait("Type", "Unit")]
        public void LeftTextDefaultsToNull()
        {
            var control = new Control();

            Assert.Null(control.LeftText);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void LeftTextCanBeSetTo(string expected)
        {
            var control = new Control(leftText: expected);

            var actual = control.LeftText;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void LeftTextRoundTripsSerializationWhen(string expected)
        {
            var control = new Control(leftText: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.LeftText);
        }

        [Fact, Trait("Type", "Unit")]
        public void MiddleTextDefaultsToNull()
        {
            var control = new Control();

            Assert.Null(control.MiddleText);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void MiddleTextCanBeSetTo(string expected)
        {
            var control = new Control(middleText: expected);

            var actual = control.MiddleText;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void MiddleTextRoundTripsSerializationWhen(string expected)
        {
            var control = new Control(middleText: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.MiddleText);
        }

        [Fact, Trait("Type", "Unit")]
        public void RightTextDefaultsToNull()
        {
            var control = new Control();

            Assert.Null(control.RightText);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void RightTextCanBeSetTo(string expected)
        {
            var control = new Control(rightText: expected);

            var actual = control.RightText;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void RightTextRoundTripsSerializationWhen(string expected)
        {
            var control = new Control(rightText: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.RightText);
        }

        [Fact, Trait("Type", "Unit")]
        public void StylingFlagsDefaultsToEmpty()
        {
            var control = new Control();

            Assert.Empty(control.StylingFlags);
        }

        [Fact, Trait("Type", "Unit")]
        public void StylingFlagsCanBeSet()
        {
            var control = new Control(stylingFlags: ImmutableHashSet.Create("Test Styling Flag A", "Test Styling Flag B", "Test Styling Flag C"));

            var actual = control.StylingFlags;

            Assert.Equal(new[] { "Test Styling Flag A", "Test Styling Flag B", "Test Styling Flag C" }, actual.OrderBy(i => i));
        }

        [Fact, Trait("Type", "Unit")]
        public void StylingFlagsRoundTripsSerialization()
        {
            var control = new Control(stylingFlags: ImmutableHashSet.Create("Test Styling Flag A", "Test Styling Flag B", "Test Styling Flag C"));

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(new[] { "Test Styling Flag A", "Test Styling Flag B", "Test Styling Flag C" }, roundTripped.StylingFlags.OrderBy(i => i));
        }
    }
}
