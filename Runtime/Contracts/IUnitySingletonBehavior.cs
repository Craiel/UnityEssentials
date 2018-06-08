namespace Craiel.UnityEssentials.Runtime.Contracts
{
    public interface IUnitySingletonBehavior
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
