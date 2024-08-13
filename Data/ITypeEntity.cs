namespace DetectiveClub.Data;

public interface ITypeEntity<out T, TType>: IEntity<T>
{
    public TType TypeId { get; set; }
}