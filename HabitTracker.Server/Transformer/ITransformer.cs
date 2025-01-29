namespace HabitTracker.Server.Transformer
{
    public interface ITransformer<T, T1>
    {
        T Transform(T1 input);
    }
}
