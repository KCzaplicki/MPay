namespace MPay.Infrastructure.DAL.UnitOfWork;

internal interface IUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> executeExpression);
}