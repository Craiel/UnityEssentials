namespace Craiel.UnityEssentials.Runtime.AI.BTree.Utils
{
    using System;
    using System.Collections.Generic;
    using BTree;
    using Contracts;
    using Exceptions;
    using Runtime.Enums;
    using Runtime.Utils;

    /// <summary>
    /// Serializer for <see cref="BehaviorStream{T}"/>
    /// </summary>
    /// <typeparam name="T">the type of <see cref="IBlackboard"/> the tree is using</typeparam>
    public class BehaviorTreeSerializer<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Serialize the given tree to string format using Json
        /// </summary>
        /// <param name="tree">the tree to serialize</param>
        /// <returns>the string data for the tree</returns>
        /// <exception cref="SerializationException">if any data is not able to serialize</exception>
        public string Serialize(BehaviorStream<T> tree)
        {
            var serializer = new YamlFluentSerializer();
            serializer.Begin(YamlContainerType.Dictionary)
                .Add("Size", tree.stream.Length)
                .Add("GrowBy", tree.GrowBy)
                .Add("RootId", tree.Root.Value)
            .End();

            serializer.Begin(YamlContainerType.Dictionary);
            for (var i = 0; i < tree.stream.Length; i++)
            {
                if (tree.stream[i] == null)
                {
                    serializer.Add(i);
                    continue;
                }

                serializer.Add(i, tree.stream[i].GetType().AssemblyQualifiedName);
            }

            serializer.End();

            serializer.Begin(YamlContainerType.Dictionary);
            for (var i = 0; i < tree.stream.Length; i++)
            {
                if (tree.stream[i] == null)
                {
                    serializer.Add(i);
                    continue;
                }

                serializer.Add(i, tree.stream[i]);
            }
            serializer.End();
            
            return serializer.Serialize();
        }

        /// <summary>
        /// Deserialize the given data into a <see cref="BehaviorStream{T}"/>.
        /// </summary>
        /// <param name="blackboard">the <see cref="IBlackboard"/> the deserialized tree will use</param>
        /// <param name="data">the data to load from</param>
        /// <returns>the deserialized tree</returns>
        /// <exception cref="SerializationException">if the data fails to deserialize</exception>
        public BehaviorStream<T> Deserialize(T blackboard, string data)
        {
            int size;
            int growBy;
            ushort id;
            var deserializer = new YamlFluentDeserializer(data)
                .Read("Size", out size)
                .Read("GrowBy", out growBy)
                .Read("RootId", out id);

            BehaviorStream<T> result = new BehaviorStream<T>(blackboard, size, growBy) { Root = new TaskId(id) };

            IDictionary<int, string> typeMap = new Dictionary<int, string>();
            deserializer.BeginRead();
            for (var i = 0; i < size; i++)
            {
                string typeName;
                deserializer.Read(i, out typeName);
                typeMap.Add(i, typeName);
            }

            deserializer.EndRead();

            deserializer.BeginRead();
            for (var i = 0; i < size; i++)
            {
                if (string.IsNullOrEmpty(typeMap[i]))
                {
                    continue;
                }

                Type type = Type.GetType(typeMap[i]);
                if (type == null)
                {
                    throw new SerializationException("Could not get type information for " + typeMap[i]);
                }

                Task<T> task = Activator.CreateInstance(type) as Task<T>;
                if (task == null)
                {
                    throw new SerializationException("Could not create task from type " + type.AssemblyQualifiedName);
                }

                deserializer.Read(i, task);
                result.stream[i] = task;
            }

            deserializer.EndRead();

            return result;
        }
    }
}
