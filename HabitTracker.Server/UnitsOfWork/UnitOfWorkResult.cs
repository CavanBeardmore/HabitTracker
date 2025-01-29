using HabitTracker.Server.Services;

namespace HabitTracker.Server.UnitsOfWork
{
    public class UnitOfWorkResult<T>
    {
        public bool Success { get; }
        public T Data { get; }
        public UnitOfWorkResult(bool success, T data) 
        {
            Success = success;
            Data = data;
        }
    }
}
