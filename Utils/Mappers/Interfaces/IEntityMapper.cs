namespace AllinOne.Utils.Mappers.Interfaces
{
    public interface IEntityMapper<TEntity, TResponse, TCreate, TUpdate>
    {
        TEntity ToEntity(TCreate request);
        void UpdateEntity(TUpdate request, TEntity entity);
        TResponse ToResponse(TEntity entity);
    }

}
