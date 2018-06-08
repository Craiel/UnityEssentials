namespace Craiel.UnityEssentials.Runtime.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using NLog;

    public class ManagedDirectory : ManagedPath
    {
        public static readonly ManagedDirectory TempDirectory = new ManagedDirectory(System.IO.Path.GetTempPath());

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string trimmedPath;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ManagedDirectory(string path, bool assertFileExistence)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Check if we are creating from a File
                if (assertFileExistence && File.Exists(path))
                {
                    throw new InvalidOperationException("File with the same name exists: " + path);
                }

                if (!this.EndsWithSeparator)
                {
                    this.Path += System.IO.Path.DirectorySeparatorChar;
                }
            }

            if (string.IsNullOrEmpty(this.Path))
            {
                return;
            }

            this.trimmedPath = this.Path;
            if (this.EndsWithSeparator)
            {
                this.trimmedPath = this.Path.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            }

            this.GetPathUsingDefaultSeparator();
            this.DirectoryName = this.Path;
            this.DirectoryNameWithoutPath = System.IO.Path.GetFileName(this.GetStringUsingDefaultSeparator(this.trimmedPath));
        }

        public ManagedDirectory(ManagedFile file)
            : this(file.DirectoryName, true)
        {
        }

        public ManagedDirectory(string path)
            : this(path, true)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool Exists
        {
            get
            {
                return Directory.Exists(this.DirectoryName);
            }
        }

        public static ManagedDirectory GetTempDirectory()
        {
            return TempDirectory.ToDirectory(System.IO.Path.GetRandomFileName());
        }

        public static IList<ManagedFileResult> GetFiles(IEnumerable<ManagedDirectoryFilter> filters)
        {
            System.Diagnostics.Debug.Assert(filters != null, "Filters need to be specified");

            IList<ManagedFileResult> results = new List<ManagedFileResult>();
            foreach (ManagedDirectoryFilter filter in filters)
            {
                if (filter.Directory == null || filter.Directory.IsNull || !filter.Directory.Exists)
                {
                    Logger.Warn("Specified directory is invalid: {0}", filter.Directory);
                    continue;
                }

                foreach (string filterString in filter.FilterStrings)
                {
                    ManagedFileResult[] files = filter.Directory.GetFiles(filterString, filter.Option);
                    if (files == null)
                    {
                        continue;
                    }

                    foreach (ManagedFileResult file in files)
                    {
                        if (results.Contains(file))
                        {
                            continue;
                        }

                        results.Add(file);
                    }
                }
            }

            return results;
        }

        public static IList<ManagedDirectoryResult> GetDirectories(IEnumerable<ManagedDirectoryFilter> filters)
        {
            IList<ManagedDirectoryResult> results = new List<ManagedDirectoryResult>();
            foreach (ManagedDirectoryFilter filter in filters)
            {
                if (filter.Directory.IsNull || !filter.Directory.Exists)
                {
                    Logger.Warn("Specified directory is invalid: {0}", filter.Directory);
                    continue;
                }

                foreach (string filterString in filter.FilterStrings)
                {
                    ManagedDirectoryResult[] directories = filter.Directory.GetDirectories(filterString, filter.Option);
                    if (directories == null)
                    {
                        continue;
                    }

                    foreach (ManagedDirectoryResult directory in directories)
                    {
                        if (results.Contains(directory))
                        {
                            continue;
                        }

                        results.Add(directory);
                    }
                }
            }

            return results;
        }

        public static IList<ManagedDirectory> ReRootDirectories(ManagedDirectory root, IEnumerable<ManagedDirectory> directories)
        {
            Debug.Assert(root != null && !root.IsNull, "Re-root requires valid root directory");

            IList<ManagedDirectory> results = new List<ManagedDirectory>();
            foreach (ManagedDirectory directory in directories)
            {
                System.Diagnostics.Debug.Assert(directory.IsRelative, "Can not re-root absolute directories!");

                results.Add(root.ToDirectory(directory));
            }

            return results;
        }

        public void Create()
        {
            if (this.Exists)
            {
                return;
            }

            Directory.CreateDirectory(this.DirectoryName);
        }

        public void Delete(bool recursive = false)
        {
            Directory.Delete(this.DirectoryName, recursive);
        }
        
        public ManagedDirectory ToDirectory<T>(params T[] other)
        {
            return new ManagedDirectory(this.CombineBefore(other));
        }

        public ManagedDirectory GetParent()
        {
            int expectedLength = this.trimmedPath.Length - this.DirectoryNameWithoutPath.Length - 1;
            if (expectedLength <= 0)
            {
                // No parent anymore
                return null;
            }

            string parentPath = this.trimmedPath.Substring(0, expectedLength);

            var result = new ManagedDirectory(parentPath);
            if (string.IsNullOrEmpty(result.DirectoryNameWithoutPath))
            {
                return null;
            }

            return result;
        }

        public ManagedFile ToFile<T>(params T[] other)
        {
            return new ManagedFile(this.CombineBefore(other));
        }

        public long GetFreeSpace()
        {
            if (this.Drive == null)
            {
                Logger.Warn("GetFreeSpace called with no drive available!");
                return 0;
            }

            return this.Drive.AvailableFreeSpace;
        }

        public void RemoveAttributes(params FileAttributes[] attributes)
        {
            var info = new DirectoryInfo(this.Path);
            foreach (FileAttributes attribute in attributes)
            {
                info.Attributes &= ~attribute;
            }
        }

        public ManagedFileResult[] GetFiles(string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!this.Exists)
            {
                return new ManagedFileResult[0];
            }

            string[] files = Directory.GetFiles(this.DirectoryName, pattern, options);
            var results = new ManagedFileResult[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string relative = files[i].Replace(this.ToString(), string.Empty);
                var result = new ManagedFileResult
                                 {
                                     Root = this,
                                     Absolute = new ManagedFile(files[i]),
                                     Relative = new ManagedFile(relative)
                                 };
                results[i] = result;
            }

            return results;
        }

        public ManagedDirectoryResult[] GetDirectories(string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!this.Exists)
            {
                return new ManagedDirectoryResult[0];
            }

            string[] subDirectories = Directory.GetDirectories(this.DirectoryName, pattern, options);
            var results = new ManagedDirectoryResult[subDirectories.Length];
            for (int i = 0; i < subDirectories.Length; i++)
            {
                string relative = subDirectories[i].Replace(this.ToString(), string.Empty);
                var result = new ManagedDirectoryResult
                                 {
                                     Absolute = new ManagedDirectory(subDirectories[i]),
                                     Relative = new ManagedDirectory(relative, assertFileExistence: false)
                                 };
                results[i] = result;
            }

            return results;
        }

        public override string ToString()
        {
            return this.GetPath();
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as ManagedDirectory;
            if (typed == null)
            {
                return false;
            }

            return typed.Path == this.Path;
        }

        public ManagedDirectory FindParent(string name, bool matchFullName = true, bool caseSensitive = false)
        {
            ManagedDirectory firstParent = this.GetParent();
            if (firstParent == null)
            {
                return null;
            }

            var queue = new Queue<ManagedDirectory>();
            queue.Enqueue(firstParent);
            while (queue.Count > 0)
            {
                var directory = queue.Dequeue();
                bool isMatch;
                if (matchFullName)
                {
                    isMatch = directory.DirectoryNameWithoutPath.Equals(name, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    isMatch = caseSensitive 
                        ? directory.DirectoryNameWithoutPath.Contains(name) 
                        : directory.DirectoryNameWithoutPath.ToLowerInvariant().Contains(name.ToLowerInvariant());
                }

                if (isMatch)
                {
                    return directory;
                }

                ManagedDirectory parent = directory.GetParent();
                if (parent == null || parent.Equals(directory))
                {
                    break;
                }

                queue.Enqueue(parent);
            }

            return null;
        }

        public bool Move(ManagedDirectory target, bool overwrite = false, bool clearTargetWhenOverwriting = false)
        {
            if (!this.Exists)
            {
                Logger.Error("Move called on non-existing source");
                return false;
            }

            if (target.Exists)
            {
                if (overwrite)
                {
                    target.Delete(clearTargetWhenOverwriting);
                }
                else
                {
                    Logger.Error("Can not move directory {0}, target exists: {1}", this, target);
                    return false;
                }
            }

            System.IO.Directory.Move(this.GetPath(), target.GetPath());
            return target.Exists;
        }

        public int Copy(
            ManagedDirectory target,
            bool recursive = true,
            bool overwrite = false,
            bool clearTargetWhenOverwriting = false)
        {
            if (!this.Exists)
            {
                Logger.Error("Copy called on non-existing source");
                return 0;
            }

            if (target.Exists)
            {
                if (overwrite && clearTargetWhenOverwriting)
                {
                    target.Delete(true);
                }
            }

            ManagedFileResult[] files = this.GetFiles(options: recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // First pass, check only
            foreach (ManagedFileResult file in files)
            {
                ManagedFile targetFile = target.ToFile(file.Relative);
                if (targetFile.Exists && !overwrite)
                {
                    Logger.Error("Copy would fail, target file exist and overwrite is set to false ({0})", targetFile);
                    return 0;
                }
            }

            // Second pass, actual copy, now we can safely overwrite
            foreach (ManagedFileResult file in files)
            {
                ManagedFile targetFile = target.ToFile(file.Relative);
                file.Absolute.CopyTo(targetFile, true);
            }

            return files.Length;
        }
    }
}
