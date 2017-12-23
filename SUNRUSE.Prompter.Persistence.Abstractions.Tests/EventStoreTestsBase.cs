﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Abstractions.Tests
{
    public abstract class EventStoreTestsBase : NonRestartableEventStoreTestsBase
    {
        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false, false, false, false, false, false, 0)]
        [InlineData(true, false, false, false, false, false, false, false, 0)]
        [InlineData(true, true, false, false, false, false, false, false, 5)]
        [InlineData(true, true, true, false, false, false, false, false, 5)]
        [InlineData(true, true, false, true, false, false, false, false, 5)]
        [InlineData(true, true, true, true, false, false, false, false, 5)]
        [InlineData(true, true, false, false, true, false, false, false, 5)]
        [InlineData(true, true, true, false, true, false, false, false, 5)]
        [InlineData(true, false, false, false, false, true, false, false, 6)]
        [InlineData(true, false, false, false, false, true, true, false, 6)]
        [InlineData(true, false, false, false, false, true, false, true, 6)]
        [InlineData(true, false, false, false, false, true, true, true, 6)]
        [InlineData(true, true, false, false, false, true, false, false, 11)]
        [InlineData(true, true, false, false, false, true, true, false, 11)]
        [InlineData(true, true, false, false, false, true, false, true, 11)]
        [InlineData(true, true, false, false, false, true, true, true, 11)]
        [InlineData(true, true, true, false, false, true, false, false, 11)]
        [InlineData(true, true, true, false, false, true, true, false, 11)]
        [InlineData(true, true, true, false, false, true, false, true, 11)]
        [InlineData(true, true, true, false, false, true, true, true, 11)]
        [InlineData(true, true, false, true, false, true, false, false, 11)]
        [InlineData(true, true, false, true, false, true, true, false, 11)]
        [InlineData(true, true, false, true, false, true, false, true, 11)]
        [InlineData(true, true, false, true, false, true, true, true, 11)]
        [InlineData(true, true, true, true, false, true, false, false, 11)]
        [InlineData(true, true, true, true, false, true, true, false, 11)]
        [InlineData(true, true, true, true, false, true, false, true, 11)]
        [InlineData(true, true, true, true, false, true, true, true, 11)]
        [InlineData(true, true, false, false, true, true, false, false, 11)]
        [InlineData(true, true, false, false, true, true, true, false, 11)]
        [InlineData(true, true, false, false, true, true, false, true, 11)]
        [InlineData(true, true, false, false, true, true, true, true, 11)]
        [InlineData(true, true, true, false, true, true, false, false, 11)]
        [InlineData(true, true, true, false, true, true, true, false, 11)]
        [InlineData(true, true, true, false, true, true, false, true, 11)]
        [InlineData(true, true, true, false, true, true, true, true, 11)]
        public async Task GetStatisticsNumberOfPersistedEventsWithRestart(bool includesCompetingSequences, bool previousSessionIncludesEvents, bool previousSessionIncludesSnapshots, bool previousSessionEndsWithSnapshot, bool thisSessionStartsWithSnapshot, bool thisSessionIncludesEvents, bool thisSessionIncludesSnapshots, bool thisSessionEndsWithSnapshot, int numberOfPersistedEvents)
        {
            var previousSessionSteps = new List<Step>();
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionEndsWithSnapshot) previousSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, previousSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());
            }
            var thisSessionSteps = new List<Step>();
            if (thisSessionStartsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionEndsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, thisSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());

                var statistics = await eventStore.GetStatistics(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId);

                Assert.Equal(numberOfPersistedEvents, statistics.NumberOfPersistedEvents);
            }
        }

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false, false, false, false, false, false, 0)]
        [InlineData(true, false, false, false, false, false, false, false, 0)]
        [InlineData(true, true, false, false, false, false, false, false, 0)]
        [InlineData(true, true, true, false, false, false, false, false, 4)]
        [InlineData(true, true, false, true, false, false, false, false, 5)]
        [InlineData(true, true, true, true, false, false, false, false, 5)]
        [InlineData(true, true, false, false, true, false, false, false, 5)]
        [InlineData(true, true, true, false, true, false, false, false, 5)]
        [InlineData(true, false, false, false, false, true, false, false, 0)]
        [InlineData(true, false, false, false, false, true, true, false, 4)]
        [InlineData(true, false, false, false, false, true, false, true, 6)]
        [InlineData(true, false, false, false, false, true, true, true, 6)]
        [InlineData(true, true, false, false, false, true, false, false, 0)]
        [InlineData(true, true, false, false, false, true, true, false, 9)]
        [InlineData(true, true, false, false, false, true, false, true, 11)]
        [InlineData(true, true, false, false, false, true, true, true, 11)]
        [InlineData(true, true, true, false, false, true, false, false, 4)]
        [InlineData(true, true, true, false, false, true, true, false, 9)]
        [InlineData(true, true, true, false, false, true, false, true, 11)]
        [InlineData(true, true, true, false, false, true, true, true, 11)]
        [InlineData(true, true, false, true, false, true, false, false, 5)]
        [InlineData(true, true, false, true, false, true, true, false, 9)]
        [InlineData(true, true, false, true, false, true, false, true, 11)]
        [InlineData(true, true, false, true, false, true, true, true, 11)]
        [InlineData(true, true, true, true, false, true, false, false, 5)]
        [InlineData(true, true, true, true, false, true, true, false, 9)]
        [InlineData(true, true, true, true, false, true, false, true, 11)]
        [InlineData(true, true, true, true, false, true, true, true, 11)]
        [InlineData(true, true, false, false, true, true, false, false, 5)]
        [InlineData(true, true, false, false, true, true, true, false, 9)]
        [InlineData(true, true, false, false, true, true, false, true, 11)]
        [InlineData(true, true, false, false, true, true, true, true, 11)]
        [InlineData(true, true, true, false, true, true, false, false, 5)]
        [InlineData(true, true, true, false, true, true, true, false, 9)]
        [InlineData(true, true, true, false, true, true, false, true, 11)]
        [InlineData(true, true, true, false, true, true, true, true, 11)]
        public async Task GetStatisticsNumberOfPersistedEventsAtTimeOfLatestSnapshotWithRestart(bool includesCompetingSequences, bool previousSessionIncludesEvents, bool previousSessionIncludesSnapshots, bool previousSessionEndsWithSnapshot, bool thisSessionStartsWithSnapshot, bool thisSessionIncludesEvents, bool thisSessionIncludesSnapshots, bool thisSessionEndsWithSnapshot, int numberOfPersistedEventsAtTimeOfLatestSnapshot)
        {
            var previousSessionSteps = new List<Step>();
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionEndsWithSnapshot) previousSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, previousSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());
            }
            var thisSessionSteps = new List<Step>();
            if (thisSessionStartsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionEndsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, thisSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());

                var statistics = await eventStore.GetStatistics(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId);

                Assert.Equal(numberOfPersistedEventsAtTimeOfLatestSnapshot, statistics.NumberOfPersistedEventsAtTimeOfLatestSnapshot);
            }
        }

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false, false, false, false, false, false)]
        [InlineData(true, false, false, false, false, false, false, false)]
        [InlineData(true, true, false, false, false, false, false, false)]
        [InlineData(true, true, true, false, false, false, false, false)]
        [InlineData(true, true, false, true, false, false, false, false)]
        [InlineData(true, true, true, true, false, false, false, false)]
        [InlineData(true, true, false, false, true, false, false, false)]
        [InlineData(true, true, true, false, true, false, false, false)]
        [InlineData(true, false, false, false, false, true, false, false)]
        [InlineData(true, false, false, false, false, true, true, false)]
        [InlineData(true, false, false, false, false, true, false, true)]
        [InlineData(true, false, false, false, false, true, true, true)]
        [InlineData(true, true, false, false, false, true, false, false)]
        [InlineData(true, true, false, false, false, true, true, false)]
        [InlineData(true, true, false, false, false, true, false, true)]
        [InlineData(true, true, false, false, false, true, true, true)]
        [InlineData(true, true, true, false, false, true, false, false)]
        [InlineData(true, true, true, false, false, true, true, false)]
        [InlineData(true, true, true, false, false, true, false, true)]
        [InlineData(true, true, true, false, false, true, true, true)]
        [InlineData(true, true, false, true, false, true, false, false)]
        [InlineData(true, true, false, true, false, true, true, false)]
        [InlineData(true, true, false, true, false, true, false, true)]
        [InlineData(true, true, false, true, false, true, true, true)]
        [InlineData(true, true, true, true, false, true, false, false)]
        [InlineData(true, true, true, true, false, true, true, false)]
        [InlineData(true, true, true, true, false, true, false, true)]
        [InlineData(true, true, true, true, false, true, true, true)]
        [InlineData(true, true, false, false, true, true, false, false)]
        [InlineData(true, true, false, false, true, true, true, false)]
        [InlineData(true, true, false, false, true, true, false, true)]
        [InlineData(true, true, false, false, true, true, true, true)]
        [InlineData(true, true, true, false, true, true, false, false)]
        [InlineData(true, true, true, false, true, true, true, false)]
        [InlineData(true, true, true, false, true, true, false, true)]
        [InlineData(true, true, true, false, true, true, true, true)]
        public async Task GetEventWithRestart(bool includesCompetingSequences, bool previousSessionIncludesEvents, bool previousSessionIncludesSnapshots, bool previousSessionEndsWithSnapshot, bool thisSessionStartsWithSnapshot, bool thisSessionIncludesEvents, bool thisSessionIncludesSnapshots, bool thisSessionEndsWithSnapshot)
        {
            var previousSessionSteps = new List<Step>();
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionEndsWithSnapshot) previousSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, previousSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());
            }
            var thisSessionSteps = new List<Step>();
            if (thisSessionStartsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionEndsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, thisSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());
                var expected = previousSessionSteps
                    .Concat(thisSessionSteps)
                    .Where(step => !step.IsSnapshot)
                    .Select(step => step.Data)
                    .ToList();
                var actual = new List<ImmutableArray<byte>>();

                while (actual.Count < expected.Count()) actual.Add(await eventStore.GetEvent(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, actual.Count));

                Assert.Equal(expected, actual);
            }
        }

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false, false, false, false, false, false)]
        [InlineData(true, false, false, false, false, false, false, false)]
        [InlineData(true, true, false, false, false, false, false, false)]
        [InlineData(true, true, true, false, false, false, false, false)]
        [InlineData(true, true, false, true, false, false, false, false)]
        [InlineData(true, true, true, true, false, false, false, false)]
        [InlineData(true, true, false, false, true, false, false, false)]
        [InlineData(true, true, true, false, true, false, false, false)]
        [InlineData(true, false, false, false, false, true, false, false)]
        [InlineData(true, false, false, false, false, true, true, false)]
        [InlineData(true, false, false, false, false, true, false, true)]
        [InlineData(true, false, false, false, false, true, true, true)]
        [InlineData(true, true, false, false, false, true, false, false)]
        [InlineData(true, true, false, false, false, true, true, false)]
        [InlineData(true, true, false, false, false, true, false, true)]
        [InlineData(true, true, false, false, false, true, true, true)]
        [InlineData(true, true, true, false, false, true, false, false)]
        [InlineData(true, true, true, false, false, true, true, false)]
        [InlineData(true, true, true, false, false, true, false, true)]
        [InlineData(true, true, true, false, false, true, true, true)]
        [InlineData(true, true, false, true, false, true, false, false)]
        [InlineData(true, true, false, true, false, true, true, false)]
        [InlineData(true, true, false, true, false, true, false, true)]
        [InlineData(true, true, false, true, false, true, true, true)]
        [InlineData(true, true, true, true, false, true, false, false)]
        [InlineData(true, true, true, true, false, true, true, false)]
        [InlineData(true, true, true, true, false, true, false, true)]
        [InlineData(true, true, true, true, false, true, true, true)]
        [InlineData(true, true, false, false, true, true, false, false)]
        [InlineData(true, true, false, false, true, true, true, false)]
        [InlineData(true, true, false, false, true, true, false, true)]
        [InlineData(true, true, false, false, true, true, true, true)]
        [InlineData(true, true, true, false, true, true, false, false)]
        [InlineData(true, true, true, false, true, true, true, false)]
        [InlineData(true, true, true, false, true, true, false, true)]
        [InlineData(true, true, true, false, true, true, true, true)]
        public async Task GetSnapshotWithRestart(bool includesCompetingSequences, bool previousSessionIncludesEvents, bool previousSessionIncludesSnapshots, bool previousSessionEndsWithSnapshot, bool thisSessionStartsWithSnapshot, bool thisSessionIncludesEvents, bool thisSessionIncludesSnapshots, bool thisSessionEndsWithSnapshot)
        {
            var previousSessionSteps = new List<Step>();
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionIncludesSnapshots) previousSessionSteps.Add(new Step(true, CreateTestData()));
            if (previousSessionIncludesEvents) previousSessionSteps.Add(new Step(false, CreateTestData()));
            if (previousSessionEndsWithSnapshot) previousSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, previousSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());
            }
            var thisSessionSteps = new List<Step>();
            if (thisSessionStartsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesSnapshots) thisSessionSteps.Add(new Step(true, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionIncludesEvents) thisSessionSteps.Add(new Step(false, CreateTestData()));
            if (thisSessionEndsWithSnapshot) thisSessionSteps.Add(new Step(true, CreateTestData()));
            using (var eventStore = CreateInstance())
            {
                var expected = previousSessionSteps
                    .Concat(thisSessionSteps)
                    .Where(step => step.IsSnapshot)
                    .Select(step => step.Data)
                    .ToList();
                var sequences = new List<Sequence> { new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, thisSessionSteps.ToImmutableArray()) };
                if (includesCompetingSequences)
                {
                    sequences.Add(CompetingSequenceWithSameEntityTypeName);
                    sequences.Add(CompetingSequenceWithSameEntityId);
                }
                await InsertInterleaved(eventStore, sequences.ToImmutableArray());
                var actual = new List<ImmutableArray<byte>>();

                while (actual.Count < expected.Count()) actual.Add(await eventStore.GetSnapshot(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, previousSessionSteps.Concat(thisSessionSteps).TakeWhile(step => step.Data != expected[actual.Count]).Count(step => !step.IsSnapshot)));

                Assert.Equal(expected, actual);
            }
        }

        [Fact, Trait("Type", "Fuzz")]
        public async Task FuzzTest()
        {
            using (var eventStore = CreateInstance())
            {
                var previousSequenceTemplates = Enumerable
                    .Range(0, 150)
                    .Select(i =>
                    {
                        var greatestEventId = 0;
                        var greatestSnapshotEventId = 0;

                        var steps = Enumerable
                            .Range(0, Random.Next(25, 50))
                            .Select(step =>
                            {
                                var isSnapshot = step % 2 == 1 && Random.Next(0, 2) == 0;
                                if (isSnapshot) greatestSnapshotEventId = greatestEventId;

                                var output = new
                                {
                                    Snapshot = isSnapshot,
                                    Data = CreateTestData(),
                                    EventId = greatestEventId
                                };

                                if (!isSnapshot) greatestEventId++;

                                return output;
                            })
                            .ToList();

                        var lastSnapshot = steps.LastOrDefault(step => step.Snapshot);

                        return new
                        {
                            EntityTypeName = $"Test Entity Type Name {Random.Next(0, 4)}",
                            EntityId = Guid.NewGuid(),
                            ExpectedNumberOfPersistedEventsId = greatestEventId,
                            ExpectedSnapshotEventId = greatestSnapshotEventId,
                            Steps = steps
                        };
                    })
                    .ToList();
                await Task.WhenAll(previousSequenceTemplates.Select(async sequenceTemplate =>
                 {
                     foreach (var step in sequenceTemplate.Steps)
                     {
                         if (step.Snapshot)
                         {
                             await eventStore.PersistSnapshot(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId, step.Data);
                         }
                         else
                         {
                             await eventStore.PersistEvent(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId, step.Data);
                         }
                     }
                     await eventStore.GetStatistics(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId);
                     foreach (var step in sequenceTemplate.Steps)
                     {
                         if (step.Snapshot)
                         {
                             await eventStore.GetSnapshot(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId, step.EventId);
                         }
                         else
                         {
                             await eventStore.GetEvent(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId, step.EventId);
                         }
                     }
                 }));
                var thisSequenceTemplates = previousSequenceTemplates
                    .Select(previousSequenceTemplate =>
                    {


                        var greatestEventId = previousSequenceTemplate.ExpectedNumberOfPersistedEventsId;
                        var greatestSnapshotEventId = previousSequenceTemplate.ExpectedSnapshotEventId;

                        var steps = Enumerable
                            .Range(0, Random.Next(25, 50))
                            .Select(step =>
                            {
                                var isSnapshot = greatestEventId != greatestSnapshotEventId && Random.Next(0, 2) == 0;
                                if (isSnapshot) greatestSnapshotEventId = greatestEventId;

                                var output = new
                                {
                                    Snapshot = isSnapshot,
                                    Data = CreateTestData(),
                                    EventId = greatestEventId
                                };

                                if (!isSnapshot) greatestEventId++;

                                return output;
                            })
                            .ToList();

                        var lastSnapshot = steps.LastOrDefault(step => step.Snapshot);

                        return new
                        {
                            PreviousTemplate = previousSequenceTemplate,
                            ExpectedNumberOfPersistedEventsId = greatestEventId,
                            ExpectedSnapshotEventId = greatestSnapshotEventId,
                            Steps = steps
                        };
                    })
                    .ToList();

                var sequenceResults = await Task.WhenAll(thisSequenceTemplates.Select(async thisSequenceTemplate =>
                {
                    foreach (var step in thisSequenceTemplate.Steps)
                    {
                        if (step.Snapshot)
                        {
                            await eventStore.PersistSnapshot(thisSequenceTemplate.PreviousTemplate.EntityTypeName, thisSequenceTemplate.PreviousTemplate.EntityId, step.Data);
                        }
                        else
                        {
                            await eventStore.PersistEvent(thisSequenceTemplate.PreviousTemplate.EntityTypeName, thisSequenceTemplate.PreviousTemplate.EntityId, step.Data);
                        }
                    }
                    var eventIds = await eventStore.GetStatistics(thisSequenceTemplate.PreviousTemplate.EntityTypeName, thisSequenceTemplate.PreviousTemplate.EntityId);
                    Assert.Equal(thisSequenceTemplate.ExpectedNumberOfPersistedEventsId, eventIds.NumberOfPersistedEvents);
                    Assert.Equal(thisSequenceTemplate.ExpectedSnapshotEventId, eventIds.NumberOfPersistedEventsAtTimeOfLatestSnapshot);
                    var results = new List<ImmutableArray<byte>>();
                    foreach (var step in thisSequenceTemplate.PreviousTemplate.Steps.Concat(thisSequenceTemplate.Steps))
                    {
                        if (step.Snapshot)
                        {
                            results.Add(await eventStore.GetSnapshot(thisSequenceTemplate.PreviousTemplate.EntityTypeName, thisSequenceTemplate.PreviousTemplate.EntityId, step.EventId));
                        }
                        else
                        {
                            results.Add(await eventStore.GetEvent(thisSequenceTemplate.PreviousTemplate.EntityTypeName, thisSequenceTemplate.PreviousTemplate.EntityId, step.EventId));
                        }
                    }
                    return results;
                }));

                Assert.Equal(thisSequenceTemplates.Select(sequence => sequence.PreviousTemplate.Steps.Select(step => step.Data).Concat(sequence.Steps.Select(step => step.Data))), sequenceResults);
            }
        }
    }
}
