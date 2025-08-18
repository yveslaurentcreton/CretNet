namespace CretNet.Platform.Data
{
    public interface IEntity<TType> : IIdentity<TType>
        where TType : notnull
    {
    }

    public interface IEntity : IEntity<int>
    {
    }
}