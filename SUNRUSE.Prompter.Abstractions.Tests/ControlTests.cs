// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Newtonsoft.Json;
using System;
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
        public void LeftIconDefaultsToNull()
        {
            var control = new Control();

            Assert.Null(control.LeftIcon);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void LeftIconCanBeSetTo(string expected)
        {
            var control = new Control(leftIcon: expected);

            var actual = control.LeftIcon;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void LeftIconRoundTripsSerializationWhen(string expected)
        {
            var control = new Control(leftIcon: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.LeftIcon);
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
        public void RightIconDefaultsToNull()
        {
            var control = new Control();

            Assert.Null(control.RightIcon);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void RightIconCanBeSetTo(string expected)
        {
            var control = new Control(rightIcon: expected);

            var actual = control.RightIcon;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(null)]
        [InlineData("Test Non-Null Value")]
        public void RightIconRoundTripsSerializationWhen(string expected)
        {
            var control = new Control(rightIcon: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.RightIcon);
        }

        [Fact, Trait("Type", "Unit")]
        public void IsFirstInGroupDefaultsToFalse()
        {
            var control = new Control();

            Assert.False(control.IsFirstInGroup);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(false)]
        [InlineData(true)]
        public void IsFirstInGroupCanBeSetTo(bool expected)
        {
            var control = new Control(isFirstInGroup: expected);

            var actual = control.IsFirstInGroup;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(false)]
        [InlineData(true)]
        public void IsFirstInGroupRoundTripsSerializationWhen(bool expected)
        {
            var control = new Control(isFirstInGroup: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.IsFirstInGroup);
        }

        [Fact, Trait("Type", "Unit")]
        public void IsLastInGroupDefaultsToFalse()
        {
            var control = new Control();

            Assert.False(control.IsLastInGroup);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(false)]
        [InlineData(true)]
        public void IsLastInGroupCanBeSetTo(bool expected)
        {
            var control = new Control(isLastInGroup: expected);

            var actual = control.IsLastInGroup;

            Assert.Equal(expected, actual);
        }

        [Theory, Trait("Type", "Unit")]
        [InlineData(false)]
        [InlineData(true)]
        public void IsLastInGroupRoundTripsSerializationWhen(bool expected)
        {
            var control = new Control(isLastInGroup: expected);

            var roundTripped = JsonConvert.DeserializeObject<Control>(JsonConvert.SerializeObject(control));

            Assert.Equal(expected, roundTripped.IsLastInGroup);
        }
    }
}
