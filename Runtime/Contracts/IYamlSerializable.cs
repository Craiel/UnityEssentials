using YamlFluentDeserializer = Craiel.UnityEssentials.Runtime.Utils.YamlFluentDeserializer;
using YamlFluentSerializer = Craiel.UnityEssentials.Runtime.Utils.YamlFluentSerializer;

namespace Craiel.UnityEssentials.Runtime.Contracts
{
    public interface IYamlSerializable
    {
        void Serialize(YamlFluentSerializer serializer);
        void Deserialize(YamlFluentDeserializer deserializer);
    }
}
