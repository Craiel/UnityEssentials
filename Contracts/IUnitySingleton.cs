namespace Assets.Scripts.Craiel.Essentials.Contracts
{
    public interface IUnitySingleton
    {
        bool IsInitialized { get; }
        void Initialize();

        void DestroySingleton();
    }
}
