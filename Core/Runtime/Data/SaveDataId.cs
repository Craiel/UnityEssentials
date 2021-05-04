namespace Craiel.UnityEssentials.Runtime.Data
{
    using System;
    using System.Linq;

    [Serializable]
    public struct SaveDataId
    {
        private static readonly string[] InvalidFileCharacters = System.IO.Path.GetInvalidFileNameChars().Select(x => x.ToString()).ToArray();
        
        public static readonly SaveDataId Invalid = new SaveDataId();
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SaveDataId(string id)
        {
            this.Id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly string Id;
        
        public static bool operator ==(SaveDataId value1, SaveDataId value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(SaveDataId value1, SaveDataId value2)
        {
            return !(value1 == value2);
        }

        public bool Equals(SaveDataId other)
        {
            return string.Equals(this.Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SaveDataId && this.Equals((SaveDataId)obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Id;
        }

        public static SaveDataId Create(string name)
        {
            var result = name.Replace(' ', '_');
            for (var i = 0; i < InvalidFileCharacters.Length; i++)
            {
                result = result.Replace(InvalidFileCharacters[i], string.Empty);
            }

            return new SaveDataId(result.ToLowerInvariant());
        } 
    }
}