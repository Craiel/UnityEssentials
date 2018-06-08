﻿namespace Craiel.UnityEssentials.Runtime.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using NLog;

    public class ManagedFile : ManagedPath
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ManagedFile(string path)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                this.FileName = System.IO.Path.GetFileName(path);
                this.Extension = System.IO.Path.GetExtension(path);
                this.DirectoryName = System.IO.Path.GetDirectoryName(path);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; protected set; }
        public string Extension { get; protected set; }

        public DateTime LastWriteTime
        {
            get
            {
                return File.GetLastWriteTime(this.Path);
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return File.GetCreationTime(this.Path);
            }
        }

        public long Size
        {
            get
            {
                if (!this.Exists)
                {
                    return -1;
                }

                var info = new FileInfo(this.Path);
                return info.Length;
            }
        }

        public override bool Exists
        {
            get
            {
                return File.Exists(this.Path);
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.FileName);
            }
        }

        public static ManagedFile GetTempFile()
        {
            return ManagedDirectory.TempDirectory.ToFile(System.IO.Path.GetRandomFileName());
        }

        public static ManagedFile GetRandomFile()
        {
            return new ManagedFile(System.IO.Path.GetRandomFileName());
        }

        public static bool FileExists(ManagedFile file)
        {
            return file != null && !file.IsNull && file.Exists;
        }

        public ManagedFile ChangeExtension(string newExtension)
        {
            if (this.IsNull)
            {
                throw new InvalidOperationException();
            }

            return new ManagedFile(System.IO.Path.ChangeExtension(this.Path, newExtension));
        }

        public FileStream OpenCreate(FileMode mode = FileMode.Create)
        {
            return new FileStream(this.Path, mode, FileAccess.ReadWrite, FileShare.Read);
        }

        public FileStream OpenRead()
        {
            return File.OpenRead(this.Path);
        }

        public FileStream OpenWrite(FileMode mode = FileMode.OpenOrCreate)
        {
            switch (mode)
            {
                case FileMode.Append:
                    {
                        return new FileStream(this.Path, mode, FileAccess.Write, FileShare.Read);
                    }

                default:
                    {
                        return new FileStream(this.Path, mode, FileAccess.ReadWrite, FileShare.Read);
                    }
            }
        }

        public XmlReader OpenXmlRead()
        {
            return XmlReader.Create(this.Path);
        }

        public XmlWriter OpenXmlWrite()
        {
            return XmlWriter.Create(this.Path);
        }

        public string ReadAsString()
        {
            return File.ReadAllText(this.GetPath());
        }

        public void WriteAsString(string contents)
        {
            File.WriteAllText(this.GetPath(), contents);
        }

        public byte[] ReadAsByte()
        {
            using (FileStream stream = this.OpenRead())
            {
                Debug.Assert(stream.Length <= int.MaxValue);

                var length = (int)stream.Length;
                var result = new byte[length];
                int bytesRead = stream.Read(result, 0, length);
                if (bytesRead != length)
                {
                    throw new InvalidOperationException(string.Format("Expected to read {0} bytes but got {1}", length, bytesRead));
                }

                return result;
            }
        }

        public bool ReadAsList(ref IList<string> target)
        {
            if (!this.Exists)
            {
                return false;
            }

            target.Clear();
            using (var stream = this.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        target.Add(reader.ReadLine());
                    }
                }
            }

            return true;
        }

        public IList<string> ReadAsList()
        {
            IList<string> lines = new List<string>();
            if (this.ReadAsList(ref lines))
            {
                return lines;
            }

            return null;
        }

        public void Delete()
        {
            File.Delete(this.Path);
        }

        public void DeleteIfExists()
        {
            if (!this.Exists)
            {
                return;
            }

            this.Delete();
        }

        public void RemoveAttributes(params FileAttributes[] attributes)
        {
            var info = new FileInfo(this.Path);
            foreach (FileAttributes attribute in attributes)
            {
                info.Attributes &= ~attribute;
            }
        }

        public void Move(ManagedFile target)
        {
            File.Move(this.Path, target.GetPath());
        }

        public ManagedDirectory GetDirectory()
        {
            if (string.IsNullOrEmpty(this.DirectoryName))
            {
                return null;
            }

            return new ManagedDirectory(this.DirectoryName);
        }
        
        public ManagedFile ToFile<T>(params T[] other)
        {
            StringBuilder newPathBuilder = new StringBuilder(this.Path);
            foreach (T param in other)
            {
                newPathBuilder.Append(param);
            }

            return new ManagedFile(newPathBuilder.ToString());
        }

        public override string ToString()
        {
            return this.Path;
        }

        public bool StartsWith(string pattern, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return this.FileName.StartsWith(pattern, comparisonType);
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as ManagedFile;
            if (typed == null)
            {
                return false;
            }

            return typed.Path == this.Path;
        }

        public bool CopyTo(ManagedFile target, bool overwrite = false)
        {
            target.GetDirectory().Create();
            try
            {
                File.Copy(this.Path, target.Path, overwrite);
                return target.Exists;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to copy file {0} to {1}", this, target);
            }

            return false;
        }

        public bool Equals(ManagedFile other, StringComparison comparison)
        {
            return other.GetPath().Equals(this.Path, comparison);
        }

        public ManagedFile Rotate(int maxRotations)
        {
            if (this.Exists)
            {
                // Build a list of all possible files
                IList<ManagedFile> files = new List<ManagedFile>();
                for (int i = maxRotations - 1; i >= 0; i--)
                {
                    files.Add(this.ChangeExtension(string.Concat(".", i, this.Extension)));
                }

                // Delete the oldest one if it's there
                files[0].DeleteIfExists();

                // Move all files one up
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].Exists)
                    {
                        files[i].Move(files[i - 1]);
                    }
                }

                this.Move(files[files.Count - 1]);
                return files[files.Count - 1];
            }

            return this;
        }
    }
}
