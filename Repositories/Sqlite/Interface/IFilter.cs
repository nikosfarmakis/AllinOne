namespace AllinOne.Repositories.Sqlite.Interface
{
    public interface IFilter<T>
    {
        IQueryable<T> Apply(IQueryable<T> source);
    }
}
