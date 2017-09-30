namespace granny.classy
{
    public enum Status
    {
        Imported,
        Valid,
        Failed
    }

    public enum Reason
    {
        MultipleOrderLineVersions,
        OrderLineVersionNotFound
    }

    public class Overrun
    {
        public Status Status { get; set; }
        public Reason ReasonForFailure { get; internal set; }
        public int? OrderLineVersionId { get; internal set; }
    }
}