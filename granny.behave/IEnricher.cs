namespace granny.classy
{
    public interface IEnricher
    {
        void Enrich(PmsOverrun pmsOverrun, Overrun overrun);
    }
}