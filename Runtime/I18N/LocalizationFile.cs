namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using IO;

    [Serializable]
    public class LocalizationFile : IEnumerable<LocalizationFileEntry>
    {
        private const char CommentPrefix = '#';
        private const string IdPrefix = "msgid";
        private const string StrPrefix = "msgstr";
        private const string LineFormatMain = "{0} \"{1}\"";
        private const string LineFormatExtra = "\"{0}\"";
        private const string HeaderFormat = @"""{0}: {1}\n""";

        private readonly IList<LocalizationFileEntry> entries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LocalizationFile()
        {
            this.entries = new List<LocalizationFileEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get { return this.entries.Count; }
        }

        public void Clear()
        {
            this.entries.Clear();
        }

        public void AddData(CultureInfo culture, LocalizationStringDictionary data)
        {
            foreach (string key in data.Keys)
            {
                var entry = new LocalizationFileEntry
                {
                    Id = new[] {key}
                };

                string value = data[key];
                if (!string.Equals(key, value, StringComparison.InvariantCulture))
                {
                    entry.Str = new[] {data[key]};
                }
                else
                {
                    entry.Str = new[] {string.Empty};
                }

                this.entries.Add(entry);
            }
        }

        public void SaveAsPO(CultureInfo language, ManagedFile file)
        {
            file.DeleteIfExists();
            using (var stream = file.OpenWrite())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine(LineFormatMain, IdPrefix, string.Empty);
                    writer.WriteLine(LineFormatMain, StrPrefix, string.Empty);

                    writer.WriteLine(HeaderFormat, "Project-Id-Version", string.Empty);
                    writer.WriteLine(HeaderFormat, "POT-Creation-Date", string.Empty);
                    writer.WriteLine(HeaderFormat, "PO-Revision-Date", string.Empty);
                    writer.WriteLine(HeaderFormat, "Last-Translator", string.Empty);
                    writer.WriteLine(HeaderFormat, "Language-Team", string.Empty);
                    writer.WriteLine(HeaderFormat, "MIME-Version", "1.0");
                    writer.WriteLine(HeaderFormat, "Content-Type", "text/plain; charset=UTF-8");
                    writer.WriteLine(HeaderFormat, "Content-Transfer-Encoding", "8bit");
                    writer.WriteLine(HeaderFormat, "Language", language.Name.Replace("-", "_"));
                    writer.WriteLine(HeaderFormat, "X-Generator", string.Empty);

                    writer.WriteLine(string.Empty);

                    // Write content
                    foreach (LocalizationFileEntry entry in this.entries)
                    {
                        writer.WriteLine(string.Concat(CommentPrefix, " ", entry.Note));
                        writer.WriteLine(LineFormatMain, IdPrefix, PrepareEntryForWriting(entry.Id[0]));
                        for (var i = 1; i < entry.Id.Length; i++)
                        {
                            writer.WriteLine(LineFormatExtra, PrepareEntryForWriting(entry.Str[i]));
                        }

                        writer.WriteLine(LineFormatMain, StrPrefix, PrepareEntryForWriting(entry.Str[0]));
                        for (var i = 1; i < entry.Str.Length; i++)
                        {
                            writer.WriteLine(LineFormatExtra, PrepareEntryForWriting(entry.Str[i]));
                        }

                        writer.WriteLine(string.Empty);
                    }
                }
            }
        }

        private string PrepareEntryForWriting(string entry)
        {
            return entry
                .Replace(@"\\", "#SLASH#")
                .Replace("\r\n", @"\\rn")
                .Replace("\n", @"\\n")
                .Replace("\r", @"\\r")
                .Replace("\"", "\\\"");
        }

        private string PrepareEntryForReading(string entry)
        {
            if (entry.StartsWith("\""))
            {
                entry = entry.Substring(1, entry.Length - 1);
            }

            if (entry.EndsWith("\""))
            {
                entry = entry.Substring(0, entry.Length - 1);
            }

            return entry.Replace("\\\"", "\"")
                .Replace(@"\\rn", "\r\n")
                .Replace(@"\\n", "\n")
                .Replace(@"\\r", "\r")
                .Replace("#SLASH#", @"\\");
        }

        private enum PoLoadState
        {
            NewEntry,
            ReadingHeader,
            ReadingId,
            ReadingStr
        }

        public void LoadFromPO(string data)
        {
            this.entries.Clear();

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    writer.Write(data);
                }

                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    this.LoadFromPO(reader);
                }
            }
        }

        public void LoadFromPO(ManagedFile file)
        {
            this.entries.Clear();

            if (!file.Exists)
            {
                return;
            }

            using (var stream = file.OpenRead())
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    this.LoadFromPO(reader);
                }
            }
        }

        public LocalizationFileEntry GetEntry(int index)
        {
            return this.entries[index];
        }

        public LocalizationStringDictionary ToDictionary()
        {
            var result = new LocalizationStringDictionary();
            foreach (LocalizationFileEntry entry in this.entries)
            {
                for (var i = 0; i < entry.Id.Length; i++)
                {
                    if (result.ContainsKey(entry.Id[0]))
                    {
                        throw new InvalidDataException("Duplicate key: " + entry.Id[0]);
                    }

                    result.Add(entry.Id[0], entry.Str[0]);
                }
            }

            return result;
        }

        public IEnumerator<LocalizationFileEntry> GetEnumerator()
        {
            for (var i = 0; i < this.entries.Count; i++)
            {
                yield return this.entries[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        private void LoadFromPO(StreamReader reader)
        {
            LocalizationFileEntry currentEntry = new LocalizationFileEntry();
            PoLoadState state = PoLoadState.NewEntry;
            IList<string> pendingReadData = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    switch (state)
                    {
                        case PoLoadState.ReadingId:
                        {
                            currentEntry.Id = pendingReadData.ToArray();
                            break;
                        }

                        case PoLoadState.ReadingStr:
                        {
                            currentEntry.Str = pendingReadData.ToArray();
                            break;
                        }
                    }

                    if (currentEntry.Id != null && currentEntry.Id.Length > 0)
                    {
                        this.entries.Add(currentEntry);
                    }

                    currentEntry = new LocalizationFileEntry();
                    pendingReadData.Clear();
                    state = PoLoadState.NewEntry;

                    continue;
                }

                switch (state)
                {
                    case PoLoadState.NewEntry:
                    {
                        if (line[0] == CommentPrefix)
                        {
                            if (line.StartsWith(CommentPrefix + " "))
                            {
                                line = line.Substring(1, line.Length - 2);
                                if (!string.IsNullOrEmpty(line))
                                {
                                    currentEntry.Note = line;
                                }
                            }

                            continue;
                        }

                        if (line.StartsWith(IdPrefix))
                        {
                            line = this.PrepareEntryForReading(line.Substring(IdPrefix.Length + 1,
                                line.Length - IdPrefix.Length - 1));
                            if (string.IsNullOrEmpty(line))
                            {
                                state = PoLoadState.ReadingHeader;
                                continue;
                            }

                            pendingReadData.Add(line);
                            state = PoLoadState.ReadingId;
                            continue;
                        }

                        break;
                    }

                    case PoLoadState.ReadingId:
                    {
                        if (line.StartsWith(StrPrefix))
                        {
                            // Save the read id strings
                            currentEntry.Id = pendingReadData.ToArray();
                            pendingReadData.Clear();

                            // Read the msg string
                            line = this.PrepareEntryForReading(line.Substring(StrPrefix.Length + 1,
                                line.Length - StrPrefix.Length - 1));
                            pendingReadData.Add(line);
                            state = PoLoadState.ReadingStr;
                            continue;
                        }

                        pendingReadData.Add(this.PrepareEntryForReading(line));
                        break;
                    }

                    case PoLoadState.ReadingStr:
                    {
                        pendingReadData.Add(this.PrepareEntryForReading(line));
                        break;
                    }
                }
            }
        }
    }
}