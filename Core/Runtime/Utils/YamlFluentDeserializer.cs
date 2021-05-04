namespace Craiel.UnityEssentials.Runtime.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using Contracts;
    using YamlDotNet.RepresentationModel;

    public class YamlFluentDeserializer
    {
        private readonly YamlStream stream;

        private readonly Stack<YamlNode> parentStack;
        private readonly Stack<int> parentIndexStack;

        private YamlSequenceNode rootNode;
        private int nextRootNodeIndex;

        private YamlDocument activeDocument;
        private int activeDocumentIndex;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public YamlFluentDeserializer(string data)
        {
            this.stream = new YamlStream();
            this.parentStack = new Stack<YamlNode>();
            this.parentIndexStack = new Stack<int>();

            using (var reader = new StringReader(data))
            {
                this.stream.Load(reader);
            }

            if (this.stream.Documents.Count <= 0)
            {
                throw new SerializationException("Yaml Stream has no documents!");
            }

            this.SetDocument(0);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public YamlFluentDeserializer SetDocument(int index)
        {
            if (this.stream.Documents.Count < index)
            {
                throw new SerializationException("Stream does not have a document at index " + index);
            }

            this.activeDocument = this.stream.Documents[index];
            this.activeDocumentIndex = index;

            if (this.activeDocument.RootNode == null)
            {
                throw new SerializationException("Document has no root node");
            }

            this.ClearParentStack();
            this.rootNode = (YamlSequenceNode)this.activeDocument.RootNode;
            
            return this;
        }

        public YamlFluentDeserializer NextDocument()
        {
            this.SetDocument(this.activeDocumentIndex++);
            return this;
        }
        
        public YamlFluentDeserializer Read(int key, out string value)
        {
            value = this.DoReadString(key);
            return this;
        }
        
        public YamlFluentDeserializer Read(string key, out string value)
        {
            value = this.DoReadString(key);
            return this;
        }
        
        public YamlFluentDeserializer Read(int key, out int value)
        {
            value = this.DoReadInt(key);
            return this;
        }
        
        public YamlFluentDeserializer Read(int key, out ushort value)
        {
            value = (ushort)this.DoReadInt(key);
            return this;
        }

        public YamlFluentDeserializer Read(string key, out ushort value)
        {
            this.Read(key, out string valueString);
            value = ushort.Parse(valueString);
            return this;
        }

        public YamlFluentDeserializer Read(string key, out int value)
        {
            this.Read(key, out string valueString);
            value = int.Parse(valueString);
            return this;
        }

        public int GetElementCount()
        {
            YamlNode node = this.parentStack.Peek();
            switch (node.NodeType)
            {
                case YamlNodeType.Sequence:
                {
                    return ((YamlSequenceNode) node).Children.Count;
                }

                case YamlNodeType.Mapping:
                {
                    return ((YamlMappingNode) node).Children.Count;
                }

                default:
                {
                    throw new SerializationException("GetElementCount called on invalid parent node");
                }
            }
        }
        
        public YamlFluentDeserializer ReadAll(out IList<int> values)
        {
            values = new List<int>();
            int count = this.GetElementCount();
            for (var i = 0; i < count; i++)
            {
                this.Read(i, out int value);
                values.Add(value);
            }

            return this;
        }

        public void Read(int key, IYamlSerializable value)
        {
            this.BeginRead(key);
            value.Deserialize(this);
            this.EndRead();
        }

        public void BeginRead(int key)
        {
            this.BeginRead(key.ToString(CultureInfo.InvariantCulture));
        }

        public void BeginRead(string key)
        {
            YamlNode node = this.parentStack.Peek();
            switch (node.NodeType)
            {
                case YamlNodeType.Mapping:
                    {
                        this.PushParent(this.GetChild(key, (YamlMappingNode)node));
                        break;
                    }

                default:
                    {
                        throw new SerializationException("BeginRead called on invalid parent node");
                    }
            }
        }

        public void BeginRead()
        {
            if (this.parentStack.Count == 0)
            {
                if (this.rootNode.Children.Count == 0)
                {
                    throw new SerializationException("Root node has no children");
                }

                if (this.nextRootNodeIndex >= this.rootNode.Children.Count)
                {
                    throw new SerializationException("End of Document");
                }
                
                this.PushParent(this.rootNode.Children[this.nextRootNodeIndex], this.nextRootNodeIndex);
                this.nextRootNodeIndex++;
                return;
            }
            
            YamlNode node = this.parentStack.Peek();
            switch (node.NodeType)
            {
                case YamlNodeType.Sequence:
                {
                    // Get the current sequence position and increment on the stack
                    int currentIndex = this.parentIndexStack.Pop();
                    this.parentIndexStack.Push(currentIndex++);

                    // Add the current index node as the new parent
                    this.PushParent(((YamlSequenceNode) node).Children[currentIndex]);
                    break;
                }
                
                default:
                {
                    throw new SerializationException("BeginRead called on invalid parent node");
                }
            }
        }

        public void EndRead()
        {
            this.PopParent();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void PushParent(YamlNode node, int index = 0)
        {
            this.parentStack.Push(node);
            this.parentIndexStack.Push(index);
        }

        private void PopParent()
        {
            this.parentStack.Pop();
            this.parentIndexStack.Pop();
        }

        private void ClearParentStack()
        {
            this.parentStack.Clear();
            this.parentIndexStack.Clear();
        }

        private YamlNode GetChild(string key, YamlMappingNode mapping)
        {
            // NOTE: This line is making a lot of assumptions, we want every one of them to crash, do not add try / catch here
            return mapping.First(x => ((YamlScalarNode)x.Key).Value.Equals(key, StringComparison.Ordinal)).Value;
        }
        
        private string DoReadString(object key)
        {
            YamlNode node = this.DoRead(key);
            return ((YamlScalarNode) node)?.Value;
        }
        
        private int DoReadInt(object key)
        {
            YamlNode node = this.DoRead(key);
            if (node == null)
            {
                return 0;
            }
            
            return int.Parse(((YamlScalarNode) node).Value);
        }
        
        private YamlNode DoRead(object key)
        {
            YamlNode node = this.parentStack.Peek();
            switch (node.NodeType)
            {
                case YamlNodeType.Sequence:
                {
                    return ((YamlSequenceNode) node).Children[(int)key];
                }

                case YamlNodeType.Mapping:
                {
                    return this.GetChild(key.ToString(), (YamlMappingNode) node);
                }

                default:
                {
                    throw new SerializationException("Read of Map called on invalid parent node");
                }
            }
        }
    }
}
