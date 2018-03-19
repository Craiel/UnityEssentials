using YamlFluentDeserializer = Craiel.UnityEssentials.Utils.YamlFluentDeserializer;
using YamlFluentSerializer = Craiel.UnityEssentials.Utils.YamlFluentSerializer;

namespace Craiel.UnityEssentials.Contracts
{
    public interface IYamlSerializable
    {
        void Serialize(YamlFluentSerializer serializer);
        void Deserialize(YamlFluentDeserializer deserializer);
    }
}
