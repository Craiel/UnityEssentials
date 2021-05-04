namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    public interface IGameModule
    {
        void Initialize();
        
        void Update();

        void FixedUpdate();

        void Destroy();
    }
}