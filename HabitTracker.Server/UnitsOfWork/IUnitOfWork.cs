namespace HabitTracker.Server.UnitsOfWork
{
    public interface IUnitOfWork<T>
    {
        UnitOfWorkResult<T> execute();
        UnitOfWorkResult<T> rollback();
    }
}
