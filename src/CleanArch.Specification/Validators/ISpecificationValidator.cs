namespace CleanArchitecture.Specification;

public interface ISpecificationValidator
{
    bool IsValid<T>(T entity, ISpecification<T> specification);
}
