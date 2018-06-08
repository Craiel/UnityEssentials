namespace Craiel.UnityEssentials.Runtime.Utils
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using Contracts;
    using Enums;
    using YamlDotNet.RepresentationModel;

    public class YamlFluentSerializer
    {
        private readonly YamlStream stream;
        private readonly YamlDocument document;
        private readonly YamlSequenceNode root;

        private readonly Stack<YamlNode> parentStack;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public YamlFluentSerializer()
        {
            this.root = new YamlSequenceNode();
            this.document = new YamlDocument(this.root);
            this.stream = new YamlStream(this.document);

            this.parentStack = new Stack<YamlNode>();
            this.parentStack.Push(this.root);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public YamlFluentSerializer Add(int key, IYamlSerializable value)
        {
            return this.Add(key.ToString(CultureInfo.InvariantCulture), value);
        }

        public YamlFluentSerializer Add(string key, IYamlSerializable value)
        {
            // Create a floating node and have the child serialize into it
            this.Begin(YamlContainerType.List, true);
            value.Serialize(this);
            YamlNode valueNode = this.parentStack.Peek();
            this.End();

            // Then add the floating node to the document
            this.AddNodeToParent(key, valueNode);

            return this;
        }
        
        public YamlFluentSerializer Add(int key, string value)
        {
            return this.Add(key.ToString(CultureInfo.InvariantCulture), value);
        }

        public YamlFluentSerializer Add(int key, ushort value)
        {
            return this.Add(key.ToString(CultureInfo.InvariantCulture), value.ToString(CultureInfo.InvariantCulture));
        }

        public YamlFluentSerializer Add(string key, ushort value)
        {
            return this.Add(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public YamlFluentSerializer Add(ushort value)
        {
            return this.Add(value.ToString(CultureInfo.InvariantCulture));
        }

        public YamlFluentSerializer Add(string key, int value)
        {
            return this.Add(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public YamlFluentSerializer Add(int value)
        {
            return this.Add(value.ToString(CultureInfo.InvariantCulture));
        }

        public YamlFluentSerializer Add(string key, string value)
        {
            this.AddNodeToParent(new YamlScalarNode(key), new YamlScalarNode(value));
            return this;
        }

        public YamlFluentSerializer Add(string value)
        {
            this.AddNodeToParent(new YamlScalarNode(value));
            return this;
        }

        public YamlFluentSerializer Add<T>(IEnumerable<T> objects)
            where T : IYamlSerializable
        {
            this.Begin(YamlContainerType.List);
            foreach (T obj in objects)
            {
                obj.Serialize(this);
            }

            this.End();
            return this;
        }

        public YamlFluentSerializer Add<T>(params T[] objects)
            where T : IYamlSerializable
        {
            this.Begin(YamlContainerType.List);
            foreach (T obj in objects)
            {
                obj.Serialize(this);
            }

            this.End();
            return this;
        }

        public YamlFluentSerializer Add(IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            var dict = new YamlMappingNode();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                dict.Add(pair.Key, pair.Value);
            }

            this.AddNodeToParent(dict);
            return this;
        }

        public YamlFluentSerializer Begin(YamlContainerType type, bool floatingNode = false)
        {
            switch (type)
            {
                    case YamlContainerType.Dictionary:
                    {
                        if (floatingNode)
                        {
                            this.parentStack.Push(new YamlMappingNode());
                        }
                        else
                        {
                            this.AddNodeToParent(new YamlMappingNode(), true);
                        }

                        break;
                    }

                    case YamlContainerType.List:
                    {
                        if (floatingNode)
                        {
                            this.parentStack.Push(new YamlSequenceNode());
                        }
                        else
                        {
                            this.AddNodeToParent(new YamlSequenceNode(), true);
                        }

                        break;
                    }
            }

            return this;
        }

        public YamlFluentSerializer End()
        {
            if (this.parentStack.Count <= 1)
            {
                throw new SerializationException("Parent stack was in unexpected state");
            }

            this.parentStack.Pop();
            return this;
        }

        public string Serialize()
        {
            if (this.parentStack.Count != 1)
            {
                throw new SerializationException("Parent stack was in unexpected state, did you forget to end a list or dictionary?");
            }

            using (var writer = new StringWriter())
            {
                this.stream.Save(writer);
                return writer.ToString();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AddNodeToParent(YamlNode key, YamlNode value)
        {
            YamlNode parent = this.parentStack.Peek();
            switch (parent.NodeType)
            {
                case YamlNodeType.Mapping:
                    {
                        ((YamlMappingNode)parent).Add(key, value);
                        break;
                    }

                default:
                    {
                        throw new SerializationException("Add Node called for unsupported parent: " + parent);
                    }
            }
        }

        private void AddNodeToParent(YamlNode key, bool pushToStack = false)
        {
            YamlNode parent = this.parentStack.Peek();
            switch (parent.NodeType)
            {
                case YamlNodeType.Sequence:
                    {
                        ((YamlSequenceNode)parent).Add(key);
                        break;
                    }

                case YamlNodeType.Mapping:
                    {
                        ((YamlMappingNode)parent).Add(key, string.Empty);
                        break;
                    }

                default:
                    {
                        throw new SerializationException("Add Node called for unsupported parent: " + parent);
                    }
            }

            if (pushToStack)
            {
                this.parentStack.Push(key);
            }
        }
    }
}