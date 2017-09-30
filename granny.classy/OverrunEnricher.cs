namespace granny.classy
{
    public class OverrunEnricher
    {
        private readonly IEnricher _orderLineVersionIdEnricher;

        public OverrunEnricher(IEnricher orderLineVersionIdEnricher)
        {
            _orderLineVersionIdEnricher = orderLineVersionIdEnricher;
        }

        public Overrun Enrich(PmsOverrun pmsOverrun)
        {
            var overrun = new Overrun();

            _orderLineVersionIdEnricher.Enrich(pmsOverrun, overrun);

            if (overrun.Status == Status.Imported)
            {
                overrun.Status = Status.Valid;
            }

            return overrun;
        }
    }
}