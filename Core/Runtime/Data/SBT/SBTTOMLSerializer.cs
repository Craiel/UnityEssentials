namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System;
    using System.Globalization;
    using System.Text;
    using Enums;
    using Nodes;

    public class SBTTOMLSerializer : ISBTNodeSerializer
    {
        private const int IndentSize = 4;
        
        private readonly StringBuilder data;

        private int currentIndent;
        private string currentNameSpace = "";

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTTOMLSerializer()
        {
            this.data = new StringBuilder();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool OptionLineBreakArrays { get; set; }
        
        public void Serialize(ISBTNode node)
        {
            this.data.Clear();
            
            this.SerializeNode(node);
        }

        public string GetData()
        {
            return this.data.ToString();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string GetCurrentIndent()
        {
            return new String(' ', this.currentIndent * IndentSize);
        }

        private void SerializeNode(ISBTNode node)
        {
            switch (node.Type)
            {
                case SBTType.Unknown:
                case SBTType.List:
                case SBTType.Stream:
                {
                    throw new NotSupportedException();
                }

                case SBTType.Dictionary:
                {
                    var typed = (SBTNodeDictionary) node;
                    foreach (string key in typed)
                    {
                        ISBTNode child = typed.Read(key);
                        if (!string.IsNullOrEmpty(child.Note))
                        {
                            this.data.AppendFormat("\n{0}# {1}\n", this.GetCurrentIndent(), child.Note);
                        }
                        
                        bool isSubNode = child.Type == SBTType.Dictionary;
                        string previousNameSpace = this.currentNameSpace;
                        if (isSubNode)
                        {
                            if (!string.IsNullOrEmpty(this.currentNameSpace))
                            {
                                this.currentNameSpace = string.Concat(this.currentNameSpace, ".", key);
                            }
                            else
                            {
                                this.currentNameSpace = key;
                            }

                            this.data.Append(this.GetCurrentIndent());
                            this.data.AppendFormat("[{0}]\n", this.currentNameSpace);
                            
                            this.currentIndent++;
                        }
                        else
                        {
                            this.data.Append(this.GetCurrentIndent());
                            this.data.Append(key + " = ");
                        }
                        
                        SerializeNode(child);

                        if (isSubNode)
                        {
                            this.currentIndent--;
                            this.currentNameSpace = previousNameSpace;
                        }
                        else
                        {
                            this.data.Append(Environment.NewLine);
                        }
                    }
                    
                    break;
                }

                case SBTType.StringArray:
                {
                    var typed = (SBTNodeArrayString) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.AppendFormat("\"{0}\"", typed.Read(i));
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.ByteArray:
                {
                    var typed = (SBTNodeArrayByte) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.ShortArray:
                {
                    var typed = (SBTNodeArrayShort) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.UShortArray:
                {
                    var typed = (SBTNodeArrayUShort) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.IntArray:
                {
                    var typed = (SBTNodeArrayInt) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.UIntArray:
                {
                    var typed = (SBTNodeArrayUInt) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.LongArray:
                {
                    var typed = (SBTNodeArrayLong) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.ULongArray:
                {
                    var typed = (SBTNodeArrayULong) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }

                case SBTType.SingleArray:
                {
                    var typed = (SBTNodeArraySingle) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }
                
                case SBTType.DoubleArray:
                {
                    var typed = (SBTNodeArrayDouble) node;
                    this.data.Append("[ ");
                    for (var i = 0; i < typed.Length; i++)
                    {
                        this.data.Append(typed.Read(i).ToString());
                        if (i < typed.Length - 1)
                        {
                            this.data.Append(", ");
                        }
                    }

                    this.data.Append(" ]");
                    break;
                }
                
                case SBTType.String:
                {
                    this.data.Append(string.Concat("\"", ((SBTNodeString) node).Data, "\""));
                    break;
                }
                
                case SBTType.Byte:
                {
                    this.data.Append(((SBTNodeByte) node).Data.ToString());
                    break;
                }
                
                case SBTType.Short:
                {
                    this.data.Append(((SBTNodeShort) node).Data.ToString());
                    break;
                }
                
                case SBTType.UShort:
                {
                    this.data.Append(((SBTNodeUShort) node).Data.ToString());
                    break;
                }
                
                case SBTType.Int:
                {
                    this.data.Append(((SBTNodeInt) node).Data.ToString());
                    break;
                }
                
                case SBTType.UInt:
                {
                    this.data.Append(((SBTNodeUInt) node).Data.ToString());
                    break;
                }
                
                case SBTType.Long:
                {
                    this.data.Append(((SBTNodeLong) node).Data.ToString());
                    break;
                }
                
                case SBTType.ULong:
                {
                    this.data.Append(((SBTNodeULong) node).Data.ToString());
                    break;
                }
                
                case SBTType.Single:
                {
                    this.data.Append(((SBTNodeSingle) node).Data.ToString(CultureInfo.InvariantCulture));
                    break;
                }
                
                case SBTType.Double:
                {
                    this.data.Append(((SBTNodeDouble) node).Data.ToString(CultureInfo.InvariantCulture));
                    break;
                }

                case SBTType.DateTime:
                {
                    this.data.Append(((SBTNodeDateTime) node).Data.ToString("R"));
                    break;
                }
                
                case SBTType.TimeSpan:
                {
                    this.data.Append(((SBTNodeTimeSpan) node).Data.Ticks.ToString());
                    break;
                }

                case SBTType.Vector2:
                {
                    var typed = (SBTNodeVector2) node;
                    string valueString = JoinArrayValues(
                        typed.Data.x.ToString(CultureInfo.InvariantCulture), 
                        typed.Data.y.ToString(CultureInfo.InvariantCulture));
                    this.data.Append(string.Concat("[ ", valueString, " ]"));
                    break;
                }
                
                case SBTType.Vector3:
                {
                    var typed = (SBTNodeVector3) node;
                    string valueString = JoinArrayValues(
                        typed.Data.x.ToString(CultureInfo.InvariantCulture), 
                        typed.Data.y.ToString(CultureInfo.InvariantCulture),
                        typed.Data.z.ToString(CultureInfo.InvariantCulture));
                    this.data.Append(string.Concat("[ ", valueString, " ]"));
                    break;
                }
                
                case SBTType.Quaternion:
                {
                    var typed = (SBTNodeQuaternion) node;
                    string valueString = JoinArrayValues(
                        typed.Data.x.ToString(CultureInfo.InvariantCulture), 
                        typed.Data.y.ToString(CultureInfo.InvariantCulture),
                        typed.Data.z.ToString(CultureInfo.InvariantCulture),
                        typed.Data.w.ToString(CultureInfo.InvariantCulture));
                    this.data.Append(string.Concat("[ ", valueString, " ]"));
                    break;
                }
                
                case SBTType.Color:
                {
                    var typed = (SBTNodeColor) node;
                    string valueString = JoinArrayValues(
                        typed.Data.r.ToString(CultureInfo.InvariantCulture), 
                        typed.Data.g.ToString(CultureInfo.InvariantCulture),
                        typed.Data.b.ToString(CultureInfo.InvariantCulture),
                        typed.Data.a.ToString(CultureInfo.InvariantCulture));
                    this.data.Append(string.Concat("[ ", valueString, " ]"));
                    break;
                }
            }
        }

        private string JoinArrayValues(params string[] values)
        {
            if (this.OptionLineBreakArrays)
            {
                return string.Join(",\n", values);                
            }
            
            return string.Join(", ", values);
        }
    }
}