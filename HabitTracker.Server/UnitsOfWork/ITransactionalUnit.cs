namespace HabitTracker.Server.UnitsOfWork
{
    public interface ITransactionalUnit<T, TParams>
    {
        public T? Execute(TParams args);
    }
}
