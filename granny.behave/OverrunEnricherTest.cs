using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace granny.classy
{
    public class OverrunEnricherTest
    {
        [Fact]
        public void GivenAValidPmsOverrun_WhenEnrich_ThenTheOverrunIsValid()
        {
            var mockRepository = Substitute.For<IRepository>();
            mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new [] {345});
            var overrunEnricher = new OverrunEnricher(mockRepository);
            var pmsOverrun = new PmsOverrun
            {
                PhysicalZoneId = 123,
                AccessDeviceId = 234
            };
            var enrichedOverrun = overrunEnricher.Enrich(pmsOverrun);

            enrichedOverrun.Status.Should().Be(Status.Valid);
            enrichedOverrun.OrderLineVersionId.Should().Be(345);
        }

        [Fact]
        public void GivenAPmsOverrunWithMultipleMatchingOrderLineVersionIds_WhenEnrich_ThenTheOverrunIsFailedBecauseMultipleOrderLineVersions()
        {
            var mockRepository = Substitute.For<IRepository>();
            mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new []{345, 456});
            var overrunEnricher = new OverrunEnricher(mockRepository);
            var pmsOverrun = new PmsOverrun
            {
                PhysicalZoneId = 123,
                AccessDeviceId = 234
            };
            var enrichedOverrun = overrunEnricher.Enrich(pmsOverrun);

            enrichedOverrun.Status.Should().Be(Status.Failed);
            enrichedOverrun.ReasonForFailure.Should().Be(Reason.MultipleOrderLineVersions);
        }

        [Fact]
        public void GivenAPmsOverrunWithNoMatchingOrderLineVersionIds_WhenEnrich_ThenTheOverrunIsFailedBecauseOrderLineVersionNotFound()
        {
            var mockRepository = Substitute.For<IRepository>();
            mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new List<int>());
            var overrunEnricher = new OverrunEnricher(mockRepository);
            var pmsOverrun = new PmsOverrun
            {
                PhysicalZoneId = 123,
                AccessDeviceId = 234
            };
            var enrichedOverrun = overrunEnricher.Enrich(pmsOverrun);

            enrichedOverrun.Status.Should().Be(Status.Failed);
            enrichedOverrun.ReasonForFailure.Should().Be(Reason.OrderLineVersionNotFound);
        }
    }
}
