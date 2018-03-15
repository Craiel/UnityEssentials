namespace Craiel.UnityEssentials.Contracts
{
    public interface IUnitySingleton
    {
        bool IsInitialized { get; }
        void Initialize();

        void DestroySingleton();
    }
}
