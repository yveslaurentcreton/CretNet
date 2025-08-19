namespace CretNet.Platform.Data;

public interface IIdentity
{
    bool HasSameIdAs(IIdentity? other);
}

public interface IIdentity<TId> : IIdentity
    where TId : notnull
{
    
    TId Id { get; }
    
    bool IIdentity.HasSameIdAs(IIdentity? other)
    {
        if (other is not IIdentity<TId> otherGeneric)
            return false;

        return EqualityComparer<TId>.Default.Equals(Id, otherGeneric.Id);
    }
}