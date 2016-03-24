namespace FluentProtobufNet.Specification
{
    public interface ISpecification<in T>
    {
        bool IsSatisfied(T item);
    }
}