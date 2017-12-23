using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Abstractions.Tests
{
    public abstract class NonRestartableEventStoreTestsBase
    {
        protected abstract IEventStore CreateInstance();

        protected static readonly Random Random = new Random();

        protected static ImmutableArray<byte> CreateTestData()
        {
            lock (Random)
            {
                return Enumerable
                    .Range(0, Random.Next(20, 50))
                    .Select(i => (byte)Random.Next(0, 256))
                    .ToImmutableArray();
            }
        }

        protected struct Sequence
        {
            public readonly string EntityTypeName;

            public readonly Guid EntityId;

            public readonly ImmutableArray<Step> Steps;

            public Sequence(string entityTypeName, Guid entityId, ImmutableArray<Step> steps)
            {
                EntityTypeName = entityTypeName;
                EntityId = entityId;
                Steps = steps;
            }
        }

        protected struct Step
        {
            public readonly bool IsSnapshot;

            public readonly ImmutableArray<byte> Data;

            public Step(bool isSnapshot, ImmutableArray<byte> data)
            {
                IsSnapshot = isSnapshot;
                Data = data;
            }
        }

        protected async Task InsertInterleaved(IEventStore eventStore, params Sequence[] sequences)
        {
            var maxSteps = sequences.Max(sequence => sequence.Steps.Length);
            for (var i = 0; i < maxSteps; i++) foreach (var sequence in sequences)
                {
                    var runFromSteps = (i * sequence.Steps.Length) / maxSteps;
                    var runToSteps = ((i + 1) * sequence.Steps.Length) / maxSteps;

                    foreach (var step in sequence.Steps
                        .Skip(runFromSteps)
                        .Take(runToSteps - runFromSteps))
                    {
                        if (step.IsSnapshot)
                        {
                            await eventStore.PersistSnapshot(sequence.EntityTypeName, sequence.EntityId, step.Data);
                        }
                        else
                        {
                            await eventStore.PersistEvent(sequence.EntityTypeName, sequence.EntityId, step.Data);
                        }
                    }
                }
        }

        protected readonly Sequence CompetingSequenceWithSameEntityTypeName = new Sequence("Test Entity Type Name", Guid.NewGuid(), ImmutableArray.Create
        (
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(true, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(true, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData())
        ));

        protected readonly Sequence CompetingSequenceWithSameEntityId = new Sequence("Test Other Entity Type Name", Guid.NewGuid(), ImmutableArray.Create
        (
            new Step(false, CreateTestData()),
            new Step(true, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(true, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(true, CreateTestData()),
            new Step(false, CreateTestData()),
            new Step(true, CreateTestData())
        ));

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false, 0)]
        [InlineData(true, false, false, 5)]
        [InlineData(true, true, false, 5)]
        [InlineData(true, true, true, 5)]
        [InlineData(true, false, true, 5)]
        public async Task GetStatisticsNumberOfPersistedEventsWithoutRestarting(bool includesEvents, bool includesSnapshots, bool endsWithSnapshot, int numberOfPersistedEvents)
        {
            using (var eventStore = CreateInstance())
            {
                var steps = new List<Step>();
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (endsWithSnapshot) steps.Add(new Step(true, CreateTestData()));
                await InsertInterleaved(eventStore, CompetingSequenceWithSameEntityTypeName, CompetingSequenceWithSameEntityId, new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, steps.ToImmutableArray()));

                var statistics = await eventStore.GetStatistics(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId);

                Assert.Equal(numberOfPersistedEvents, statistics.NumberOfPersistedEvents);
            }
        }

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false, 0)]
        [InlineData(true, false, false, 0)]
        [InlineData(true, true, false, 4)]
        [InlineData(true, true, true, 5)]
        [InlineData(true, false, true, 5)]
        public async Task GetStatisticsNumberOfPersistedEventsAtTimeOfLatestSnapshotWithoutRestarting(bool includesEvents, bool includesSnapshots, bool endsWithSnapshot, int numberOfPersistedEventsAtTimeOfLatestSnapshot)
        {
            using (var eventStore = CreateInstance())
            {
                var steps = new List<Step>();
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (endsWithSnapshot) steps.Add(new Step(true, CreateTestData()));
                await InsertInterleaved(eventStore, CompetingSequenceWithSameEntityTypeName, CompetingSequenceWithSameEntityId, new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, steps.ToImmutableArray()));

                var statistics = await eventStore.GetStatistics(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId);

                Assert.Equal(numberOfPersistedEventsAtTimeOfLatestSnapshot, statistics.NumberOfPersistedEventsAtTimeOfLatestSnapshot);
            }
        }

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        public async Task GetEventWithoutRestarting(bool includesEvents, bool includesSnapshots, bool endsWithSnapshot)
        {
            using (var eventStore = CreateInstance())
            {
                var steps = new List<Step>();
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (endsWithSnapshot) steps.Add(new Step(true, CreateTestData()));
                var expected = steps
                    .Where(step => !step.IsSnapshot)
                    .Select(step => step.Data)
                    .ToList();
                await InsertInterleaved(eventStore, CompetingSequenceWithSameEntityTypeName, CompetingSequenceWithSameEntityId, new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, steps.ToImmutableArray()));
                var actual = new List<ImmutableArray<byte>>();

                while (actual.Count < expected.Count()) actual.Add(await eventStore.GetEvent(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, actual.Count));

                Assert.Equal(expected, actual);
            }
        }

        [Theory, Trait("Type", "Integration")]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        public async Task GetSnapshotWithoutRestarting(bool includesEvents, bool includesSnapshots, bool endsWithSnapshot)
        {
            using (var eventStore = CreateInstance())
            {
                var steps = new List<Step>();
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (includesSnapshots) steps.Add(new Step(true, CreateTestData()));
                if (includesEvents) steps.Add(new Step(false, CreateTestData()));
                if (endsWithSnapshot) steps.Add(new Step(true, CreateTestData()));
                var expected = steps
                    .Where(step => step.IsSnapshot)
                    .Select(step => step.Data)
                    .ToList();
                await InsertInterleaved(eventStore, CompetingSequenceWithSameEntityTypeName, CompetingSequenceWithSameEntityId, new Sequence(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, steps.ToImmutableArray()));
                var actual = new List<ImmutableArray<byte>>();

                while (actual.Count < expected.Count()) actual.Add(await eventStore.GetSnapshot(CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId, steps.TakeWhile(step => step.Data != expected[actual.Count]).Count(step => !step.IsSnapshot)));

                Assert.Equal(expected, actual);
            }
        }

        [Fact, Trait("Type", "Fuzz")]
        public async Task FuzzTestWithoutRestarting()
        {
            using (var eventStore = CreateInstance())
            {
                var sequenceTemplates = Enumerable
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

                var sequenceResults = await Task.WhenAll(sequenceTemplates.Select(async sequenceTemplate =>
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
                    var eventIds = await eventStore.GetStatistics(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId);
                    Assert.Equal(sequenceTemplate.ExpectedNumberOfPersistedEventsId, eventIds.NumberOfPersistedEvents);
                    Assert.Equal(sequenceTemplate.ExpectedSnapshotEventId, eventIds.NumberOfPersistedEventsAtTimeOfLatestSnapshot);
                    var results = new List<ImmutableArray<byte>>();
                    foreach (var step in sequenceTemplate.Steps)
                    {
                        if (step.Snapshot)
                        {
                            results.Add(await eventStore.GetSnapshot(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId, step.EventId));
                        }
                        else
                        {
                            results.Add(await eventStore.GetEvent(sequenceTemplate.EntityTypeName, sequenceTemplate.EntityId, step.EventId));
                        }
                    }
                    return results;
                }));

                Assert.Equal(sequenceTemplates.Select(sequence => sequence.Steps.Select(step => step.Data)), sequenceResults);
            }
        }
    }
}
