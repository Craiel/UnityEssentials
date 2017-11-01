namespace Assets.Scripts.Craiel.Essentials.Contracts
{
    public interface IUnitySingletonBehavior
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
