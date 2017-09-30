using System.Linq;

namespace granny.classy
{
    public class OrderLineVersionIdEnricher : IEnricher
    {
        private readonly IRepository _repository;

        public OrderLineVersionIdEnricher(IRepository repository)
        {
            _repository = repository;
        }

        public void Enrich(PmsOverrun pmsOverrun, Overrun overrun)
        {
            var orderLineVersionIds = _repository.GetMatchingOrderLineIds(pmsOverrun.PhysicalZoneId, pmsOverrun.AccessDeviceId);

            if (orderLineVersionIds.Count() > 1)
            {
                overrun.Status = Status.Failed;
                overrun.ReasonForFailure = Reason.MultipleOrderLineVersions;
                return;
            }
            if (orderLineVersionIds.Count() < 1)
            {
                overrun.Status = Status.Failed;
                overrun.ReasonForFailure = Reason.OrderLineVersionNotFound;
                return;
            }

            overrun.OrderLineVersionId = orderLineVersionIds.First();
        }
    }
}