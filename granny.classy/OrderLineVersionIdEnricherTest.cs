using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace granny.classy
{
    public class OrderLineVersionIdEnricherTest
    {
        [Fact]
        public void GivenAPmsOverrunWithMultipleMatchingOrderLineVersionIds_WhenEnrich_ThenTheOverrunIsFailedBecauseMultipleOrderLineVersions()
        {
            var mockRepository = Substitute.For<IRepository>();
            mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new []{345, 456});
            var orderLineVersionIdEnricher = new OrderLineVersionIdEnricher(mockRepository);
            var pmsOverrun = new PmsOverrun
            {
                PhysicalZoneId = 123,
                AccessDeviceId = 234
            };
            var overrun = new Overrun();
            orderLineVersionIdEnricher.Enrich(pmsOverrun, overrun);

            overrun.Status.Should().Be(Status.Failed);
            overrun.ReasonForFailure.Should().Be(Reason.MultipleOrderLineVersions);
        }

        [Fact]
        public void GivenAPmsOverrunWithNoMatchingOrderLineVersionIds_WhenEnrich_ThenTheOverrunIsFailedBecauseOrderLineVersionNotFound()
        {
            var mockRepository = Substitute.For<IRepository>();
            mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new List<int>());
            var orderLineVersionIdEnricher = new OrderLineVersionIdEnricher(mockRepository);
            var pmsOverrun = new PmsOverrun
            {
                PhysicalZoneId = 123,
                AccessDeviceId = 234
            };
            var overrun = new Overrun();
            orderLineVersionIdEnricher.Enrich(pmsOverrun, overrun);

            overrun.Status.Should().Be(Status.Failed);
            overrun.ReasonForFailure.Should().Be(Reason.OrderLineVersionNotFound);
        }

        [Fact]
        public void GivenAPmsOverrunWithOneMatchingOrderLineVersionIds_WhenEnrich_ThenTheOrderLineVersionIdIsFiledIn()
        {
            var mockRepository = Substitute.For<IRepository>();
            mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new [] {345});
            var orderLineVersionIdEnricher = new OrderLineVersionIdEnricher(mockRepository);
            var pmsOverrun = new PmsOverrun
            {
                PhysicalZoneId = 123,
                AccessDeviceId = 234
            };
            var overrun = new Overrun();
            orderLineVersionIdEnricher.Enrich(pmsOverrun, overrun);

            overrun.OrderLineVersionId.Should().Be(345);
        }
    }
}