namespace Craiel.UnityEssentials.Contracts
{
    public interface IUnitySingletonBehavior
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
