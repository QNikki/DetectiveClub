namespace DetectiveClub.Data;

public interface IEntityWithType<out T, TType>: IEntity<T>
{
    public TType TypeId { get; set; }
}