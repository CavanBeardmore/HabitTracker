namespace HabitTracker.Server.UnitsOfWork
{
    public interface IUnitOfWork
    {
        Task<UnitOfWorkResult> execute();
        Task<UnitOfWorkResult> rollback();
    }
}
