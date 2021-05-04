namespace Craiel.UnityEssentials.Runtime.Contracts
{
    public interface IUnitySingleton
    {
        bool IsInitialized { get; }
        void Initialize();

        void DestroySingleton();
    }
}
