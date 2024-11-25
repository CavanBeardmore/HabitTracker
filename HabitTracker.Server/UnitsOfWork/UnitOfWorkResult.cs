namespace HabitTracker.Server.UnitsOfWork
{
    public class UnitOfWorkResult
    {
        public bool Success { get; }
        public object Data { get; }
        public UnitOfWorkResult(bool success, object data) 
        {
            Success = success;
            Data = data;
        }
    }
}
