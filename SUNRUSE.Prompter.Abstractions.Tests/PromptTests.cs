// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace SUNRUSE.Prompter.Abstractions.Tests
{
    public sealed class PromptTests
    {
        [Fact, Trait("Type", "Unit")]
        public void PromptIdSet()
        {
            var promptId = Guid.NewGuid();

            var prompt = new Prompt(promptId, ImmutableArray<Control>.Empty);

            Assert.Equal(promptId, prompt.PromptId);
        }

        [Fact, Trait("Type", "Unit")]
        public void PromptIdRoundTripsSerialization()
        {
            var promptId = Guid.NewGuid();
            var prompt = new Prompt(promptId, ImmutableArray<Control>.Empty);

            prompt = JsonConvert.DeserializeObject<Prompt>(JsonConvert.SerializeObject(prompt));

            Assert.Equal(promptId, prompt.PromptId);
        }

        [Fact, Trait("Type", "Unit")]
        public void ControlsSet()
        {
            var prompt = new Prompt(Guid.NewGuid(), ImmutableArray.Create
            (
                new Control("Test ButtonId A", "Test LeftText A", "Test MiddleText A", "Test RightText A", ImmutableHashSet.Create("Test Styling Flag A A", "Test Styling Flag A B")),
                new Control("Test ButtonId B", "Test LeftText B", "Test MiddleText B", "Test RightText B", ImmutableHashSet<string>.Empty),
                new Control("Test ButtonId C", "Test LeftText C", "Test MiddleText C", "Test RightText C", ImmutableHashSet.Create("Test Styling Flag C A", "Test Styling Flag C B", "Test Styling Flag C C"))
            ));

            Assert.Equal(3, prompt.Controls.Length);
            Assert.Equal("Test ButtonId A", prompt.Controls[0].ButtonId);
            Assert.Equal("Test LeftText A", prompt.Controls[0].LeftText);
            Assert.Equal("Test MiddleText A", prompt.Controls[0].MiddleText);
            Assert.Equal("Test RightText A", prompt.Controls[0].RightText);
            Assert.Equal(new[] { "Test Styling Flag A A", "Test Styling Flag A B" }, prompt.Controls[0].StylingFlags.OrderBy(i => i));
            Assert.Equal("Test ButtonId B", prompt.Controls[1].ButtonId);
            Assert.Equal("Test LeftText B", prompt.Controls[1].LeftText);
            Assert.Equal("Test MiddleText B", prompt.Controls[1].MiddleText);
            Assert.Equal("Test RightText B", prompt.Controls[1].RightText);
            Assert.Empty(prompt.Controls[1].StylingFlags);
            Assert.Equal("Test ButtonId C", prompt.Controls[2].ButtonId);
            Assert.Equal("Test LeftText C", prompt.Controls[2].LeftText);
            Assert.Equal("Test MiddleText C", prompt.Controls[2].MiddleText);
            Assert.Equal("Test RightText C", prompt.Controls[2].RightText);
            Assert.Equal(new[] { "Test Styling Flag C A", "Test Styling Flag C B", "Test Styling Flag C C" }, prompt.Controls[2].StylingFlags.OrderBy(i => i));
        }

        [Fact, Trait("Type", "Unit")]
        public void ControlsRoundTripsSerialization()
        {
            var prompt = new Prompt(Guid.NewGuid(), ImmutableArray.Create
            (
                new Control("Test ButtonId A", "Test LeftText A", "Test MiddleText A", "Test RightText A", ImmutableHashSet.Create("Test Styling Flag A A", "Test Styling Flag A B")),
                new Control("Test ButtonId B", "Test LeftText B", "Test MiddleText B", "Test RightText B", ImmutableHashSet<string>.Empty),
                new Control("Test ButtonId C", "Test LeftText C", "Test MiddleText C", "Test RightText C", ImmutableHashSet.Create("Test Styling Flag C A", "Test Styling Flag C B", "Test Styling Flag C C"))
            ));

            prompt = JsonConvert.DeserializeObject<Prompt>(JsonConvert.SerializeObject(prompt));

            Assert.Equal(3, prompt.Controls.Length);
            Assert.Equal("Test ButtonId A", prompt.Controls[0].ButtonId);
            Assert.Equal("Test LeftText A", prompt.Controls[0].LeftText);
            Assert.Equal("Test MiddleText A", prompt.Controls[0].MiddleText);
            Assert.Equal("Test RightText A", prompt.Controls[0].RightText);
            Assert.Equal(new[] { "Test Styling Flag A A", "Test Styling Flag A B" }, prompt.Controls[0].StylingFlags.OrderBy(i => i));
            Assert.Equal("Test ButtonId B", prompt.Controls[1].ButtonId);
            Assert.Equal("Test LeftText B", prompt.Controls[1].LeftText);
            Assert.Equal("Test MiddleText B", prompt.Controls[1].MiddleText);
            Assert.Equal("Test RightText B", prompt.Controls[1].RightText);
            Assert.Empty(prompt.Controls[1].StylingFlags);
            Assert.Equal("Test ButtonId C", prompt.Controls[2].ButtonId);
            Assert.Equal("Test LeftText C", prompt.Controls[2].LeftText);
            Assert.Equal("Test MiddleText C", prompt.Controls[2].MiddleText);
            Assert.Equal("Test RightText C", prompt.Controls[2].RightText);
            Assert.Equal(new[] { "Test Styling Flag C A", "Test Styling Flag C B", "Test Styling Flag C C" }, prompt.Controls[2].StylingFlags.OrderBy(i => i));
        }

        [Fact, Trait("Type", "Unit")]
        public void BackgroundLayersDefaultsToEmpty()
        {
            var prompt = new Prompt(Guid.NewGuid(), ImmutableArray<Control>.Empty);

            Assert.Empty(prompt.BackgroundLayers);
        }

        [Fact, Trait("Type", "Unit")]
        public void BackgroundLayersSet()
        {
            var prompt = new Prompt(Guid.NewGuid(), ImmutableArray<Control>.Empty, ImmutableArray.Create("Test Background Layer A", "Test Background Layer B", "Test Background Layer C"));

            Assert.Equal(new[] { "Test Background Layer A", "Test Background Layer B", "Test Background Layer C" }, prompt.BackgroundLayers);
        }

        [Fact, Trait("Type", "Unit")]
        public void BackgroundLayersRoundTripsSerialization()
        {
            var prompt = new Prompt(Guid.NewGuid(), ImmutableArray<Control>.Empty, ImmutableArray.Create("Test Background Layer A", "Test Background Layer B", "Test Background Layer C"));

            prompt = JsonConvert.DeserializeObject<Prompt>(JsonConvert.SerializeObject(prompt));

            Assert.Equal(new[] { "Test Background Layer A", "Test Background Layer B", "Test Background Layer C" }, prompt.BackgroundLayers);
        }
    }
}
