﻿namespace Craiel.UnityEssentials
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using IO;
    using NLog;

    public static class AssemblyExtensions
    {
        private const char ResourceDelimiter = '.';

        private const string ResourceLocalIndicator = @"__.";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Version GetVersion(Type type)
        {
            return Assembly.GetAssembly(type).GetVersionCompat();
        }

        public static Version GetVersionCompat(this Assembly assembly)
        {
            return assembly.GetName().Version;
        }

        public static ManagedFile GetAssemblyFileCompat(this Assembly assembly)
        {
            return new ManagedFile(assembly.Location);
        }

        public static ManagedDirectory GetDirectory(this Assembly assembly)
        {
            ManagedFile file = assembly.GetAssemblyFileCompat();
            return file == null ? null : file.GetDirectory();
        }

        public static IList<ManagedFile> GetLoadedAssemblyFiles(AppDomain domain = null)
        {
            if (domain == null)
            {
                domain = AppDomain.CurrentDomain;
            }

            Assembly[] assemblies = domain.GetAssemblies();
            if (assemblies.Length <= 0)
            {
                Logger.Warn("Could not locate any assemblies in domain {0}", domain);
                return null;
            }

            IList<ManagedFile> results = new List<ManagedFile>(assemblies.Length);
            foreach (Assembly assembly in assemblies)
            {
                ManagedFile file = assembly.GetAssemblyFileCompat();
                if (file == null)
                {
                    continue;
                }

                results.Add(file);
            }

            return results;
        }

        public static string GetLocalizedResourcePath(string resourcePath, string assemblyRoot)
        {
            string localizedResourcePath = resourcePath;
            if (!string.IsNullOrEmpty(assemblyRoot))
            {
                localizedResourcePath = localizedResourcePath.Replace(assemblyRoot, string.Empty);
            }

            localizedResourcePath = localizedResourcePath.TrimStart(ResourceDelimiter);

            // Check if we have a local indicator in the resource path
            string localizedResourcePathSuffix = string.Empty;
            int localIndicatorIndex = localizedResourcePath.IndexOf(ResourceLocalIndicator, StringComparison.Ordinal);
            if (localizedResourcePath.Contains(ResourceLocalIndicator))
            {
                localizedResourcePathSuffix = localizedResourcePath.Substring(localIndicatorIndex + ResourceLocalIndicator.Length, localizedResourcePath.Length - localIndicatorIndex - ResourceLocalIndicator.Length);
                localizedResourcePath = string.Empty;
            }

            // Files with *.*.*.ext are translated to *\*\*.ext
            string[] segments = localizedResourcePath.Split(new[] { ResourceDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            string extension = string.Empty;
            if (segments.Length < 2 || segments[segments.Length - 1].Length > 5)
            {
                extension = string.Empty;
            }
            else if (segments.Length > 0)
            {
                extension = segments[segments.Length - 1];
                segments = segments.Take(segments.Length - 1).ToArray();
            }

            // Ignore internal resources (*.g.*)
            if (segments.Length > 0 && segments[0] == "g")
            {
                return null;
            }

            string file = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), segments) + localizedResourcePathSuffix;
            if (!string.IsNullOrEmpty(extension))
            {
                file = string.Concat(file, ".", extension);
            }

            return file;
        }

        public static string LoadResourceAsString(this Assembly assembly, string resourcePath, string assemblyRoot = "")
        {
            byte[] data = assembly.LoadResource(resourcePath, assemblyRoot);
            if (data == null)
            {
                return null;
            }

            return System.Text.Encoding.ASCII.GetString(data).TrimStart('?');
        }

        public static byte[] LoadResource(
            this Assembly assembly,
            string resourcePath,
            string assemblyRoot = "")
        {
            string resourceFileName;
            return assembly.LoadResource(resourcePath, out resourceFileName, assemblyRoot);
        }

        public static byte[] LoadResource(this Assembly assembly, string resourcePath, out string resourceFileName, string assemblyRoot = "")
        {
            resourceFileName = GetLocalizedResourcePath(resourcePath, assemblyRoot);
            if (resourceFileName == null)
            {
                // The file was not valid for loading
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    Logger.Error("Could not Open Resource Stream: {0}", resourcePath);
                    return null;
                }
                
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static IDictionary<string, byte[]> LoadResources(this Assembly assembly, string path = null)
        {
            IDictionary<string, byte[]> results = new Dictionary<string, byte[]>();

            string[] resources = assembly.GetManifestResourceNames();
            if (resources.Length <= 0)
            {
                Logger.Warn("No resource to load for {0}", assembly);
                return null;
            }

            string assemblyRoot = assembly.GetName().Name + '.';
            if (!string.IsNullOrEmpty(path))
            {
                assemblyRoot += path;
            }

            foreach (string resource in resources)
            {
                if (!resource.StartsWith(assemblyRoot))
                {
                    continue;
                }

                string file;
                byte[] data = LoadResource(assembly, resource, out file, assemblyRoot);
                if (data != null)
                {
                    results.Add(file, data);
                }
            }

            return results;
        }

        public static IList<ManagedFile> ExtractResources(this Assembly assembly, ManagedDirectory target, string path = null, bool replace = true)
        {
            ManagedDirectory location = assembly.GetDirectory();
            if (!location.Exists)
            {
                throw new InvalidOperationException("ExtractResources with invalid location");
            }

            target.Create();

            IDictionary<string, byte[]> resources = LoadResources(assembly, path);
            if (resources == null || resources.Count <= 0)
            {
                return null;
            }

            IList<ManagedFile> results = new List<ManagedFile>();
            foreach (string file in resources.Keys)
            {
                var targetFile = target.ToFile(file);
                targetFile.GetDirectory().Create();
                results.Add(targetFile);

                using (var writer = targetFile.OpenCreate())
                {
                    writer.Write(resources[file], 0, resources[file].Length);
                    Logger.Info("Extracted {0} ({1})", targetFile, targetFile.Size);
                }
            }

            return results;
        }
    }
}
