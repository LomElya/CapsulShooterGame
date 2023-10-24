namespace Interfaces
{
    public interface IModificationListener<T>
    {
        void OnModificationUpdate(T value);
    }

}
