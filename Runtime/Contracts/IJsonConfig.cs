using ManagedFile = Craiel.UnityEssentials.Runtime.IO.ManagedFile;

namespace Craiel.UnityEssentials.Runtime.Contracts
{
    public interface IJsonConfig<T>
    {
        T Current { get; set; }

        bool Load(ManagedFile file);
        bool Save(ManagedFile file = null);

        void Reset();
    }
}
