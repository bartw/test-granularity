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
            var mockOrderLineVersionIdEnricher = Substitute.For<IEnricher>();
            var overrunEnricher = new OverrunEnricher(mockOrderLineVersionIdEnricher);
            var enrichedOverrun = overrunEnricher.Enrich(new PmsOverrun());

            enrichedOverrun.Status.Should().Be(Status.Valid);
        }
    }
}
