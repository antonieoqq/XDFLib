namespace XDFLib
{
    public interface IPoolObject
    {
        void OnGetFromPool();
        void OnRecycleToPool();
    }
}
