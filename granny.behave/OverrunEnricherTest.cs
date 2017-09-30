using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace granny.classy
{
    public class OverrunEnricherTest
    {
        private readonly IRepository _mockRepository;
        private readonly IServiceProvider _serviceProvider;

        public OverrunEnricherTest()
        {
            _mockRepository = Substitute.For<IRepository>();
            _serviceProvider = new ServiceCollection()
            .AddSingleton<IRepository>(_mockRepository)
            .AddTransient<OverrunEnricher, OverrunEnricher>()
            .BuildServiceProvider();
        }

        [Fact]
        public void GivenAValidPmsOverrun_WhenEnrich_ThenTheOverrunIsValid()
        {
            _mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new[] { 345 });
            var overrunEnricher = _serviceProvider.GetService<OverrunEnricher>();
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
            _mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new[] { 345, 456 });
            var overrunEnricher = _serviceProvider.GetService<OverrunEnricher>();
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
            _mockRepository.GetMatchingOrderLineIds(123, 234).Returns(new List<int>());
            var overrunEnricher = _serviceProvider.GetService<OverrunEnricher>();
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
