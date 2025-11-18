namespace AllinOne.Services.Interfaces.PaginationStrategies
{
    public interface IPaginationStrategyFactory<T> where T : class
    {
        IPaginationStrategy<T, TOut> GetStrategy<TOut>(int totalRecords) where TOut : class;
    }
}
