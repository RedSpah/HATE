using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HATE
{
    static class Shuffle
    {
        public const int WordSize = 4;
        public const int NumDataSegments = 25;


        public static bool LoadDataAndFind(string seeked_header, Random random, float shufflechance, StreamWriter logstream, string resource_file, Func<FileStream, Random, float, string, StreamWriter, bool> shufflefunc)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            byte[] readBuffer = new byte[WordSize];

            using (FileStream stream = new FileStream(resource_file, FileMode.OpenOrCreate))
            {
                logstream.WriteLine($"Opened {resource_file}.");
                stream.Position = 8;

                int dataSegmentCounter = 0;

                while (dataSegmentCounter++ < NumDataSegments)
                {
                    stream.Read(readBuffer, 0, WordSize);

                    string headerName = new string(readBuffer.Select(x => (char)x).ToArray());

                    if (headerName == seeked_header)
                    {
                        logstream.WriteLine($"{seeked_header} Memory Region Found at {stream.Position.ToString("X")}.");

                        stream.Position += WordSize;

                        try
                        {
                            if (!shufflefunc(stream, random, shufflechance, seeked_header, logstream))
                            {
                                logstream.WriteLine($"An Error Occured While Attempting To Modify {seeked_header} Memory Region.");
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            logstream.Write($"Exception Caught While Attempting To Modify {seeked_header} Memory Region. -> {e}");
                            throw;
                        }

                        logstream.WriteLine($"{seeked_header} Memory Region Modified Successfully.");
                        logstream.WriteLine($"Closed {resource_file}.");
                        return true;
                    }

                    stream.Read(readBuffer, 0, WordSize);
                    stream.Position += BitConverter.ToInt32(readBuffer, 0);
                }

                logstream.WriteLine($"Could not find {seeked_header} Memory Region.");
                logstream.WriteLine($"Closed {resource_file}.");
            }
            return false;
        }



        enum ComplexShuffleStep : byte { Accumulation, FirstLog, Shuffling, SecondLog, Writing, ThirdLog }

        public static Func<FileStream, Random, float, string, StreamWriter, bool> ComplexShuffle(
            Func<FileStream, Random, float, StreamWriter, List<ResourcePointer>> accumulator,
            Func<FileStream, Random, float, StreamWriter, List<ResourcePointer>, List<ResourcePointer>> shuffler,
            Func<FileStream, Random, float, StreamWriter, List<ResourcePointer>, bool> writer)
        {
            return (FileStream stream, Random random, float chance, string header, StreamWriter logstream) =>
            {
                bool success = false;
                ComplexShuffleStep step = ComplexShuffleStep.Accumulation;
                try
                {
                    List<ResourcePointer> pointerList = accumulator(stream, random, chance, logstream);
                    step = ComplexShuffleStep.FirstLog;
                    logstream.WriteLine($"Added {pointerList.Count} pointers to {header} List.");
                    step = ComplexShuffleStep.Shuffling;
                    pointerList = shuffler(stream, random, chance, logstream, pointerList);
                    step = ComplexShuffleStep.SecondLog;
                    logstream.WriteLine($"Shuffled {pointerList.Count} pointers to {header} List.");
                    step = ComplexShuffleStep.Writing;
                    success = writer(stream, random, chance, logstream, pointerList);
                    step = ComplexShuffleStep.ThirdLog;
                    logstream.WriteLine($"Written {pointerList.Count} shuffled pointers to {header} List.");
                }
                catch (Exception ex)
                {
                    logstream.WriteLine($"Caught exception [{ex}] while editing {header} memory block, during step {step}.");
                    throw;
                }

                return success;
            };
        }

        public static List<ResourcePointer> SimpleAccumulator(FileStream stream, Random random, float shufflechance, StreamWriter logstream)
        {
            byte[] readBuffer = new byte[WordSize];
            int pointerNum = 0;
            List<ResourcePointer> pointerList = new List<ResourcePointer>();

            stream.Read(readBuffer, 0, WordSize);
            pointerNum = BitConverter.ToInt32(readBuffer, 0);

            for (int i = 0; i < pointerNum; i++)
            {
                stream.Read(readBuffer, 0, WordSize);

                if (random.NextDouble() < shufflechance)
                    pointerList.Add(new ResourcePointer(BitConverter.ToInt32(readBuffer, 0), (int)(stream.Position - WordSize)));
            }

            return pointerList;
        }

        public static List<ResourcePointer> SimpleShuffler(FileStream stream, Random random, float shufflechance, StreamWriter logstream, List<ResourcePointer> pointerlist)
        {
            pointerlist.Shuffle(PointerSwapLoc, random);
            return pointerlist;
        }

        public static bool SimpleWriter(FileStream stream, Random random, float shufflechance, StreamWriter logstream, List<ResourcePointer> pointerlist)
        {
            foreach (ResourcePointer ptr in pointerlist)
            {
                stream.Position = ptr.Location;
                stream.Write(BitConverter.GetBytes(ptr.Address), 0, WordSize);
            }
            return true;
        }

        public static Func<FileStream, Random, float, string, StreamWriter, bool> SimpleShuffle = ComplexShuffle(SimpleAccumulator, SimpleShuffler, SimpleWriter);

        public static List<ResourcePointer> StringDumpAccumulator(FileStream stream, Random random, float shufflechance, StreamWriter logstream)
        {
            byte[] readBuffer = new byte[WordSize];
            int pointerNum = 0;
            List<ResourcePointer> pointerList = new List<ResourcePointer>();

            stream.Read(readBuffer, 0, WordSize);
            pointerNum = BitConverter.ToInt32(readBuffer, 0);

            for (int i = 0; i < pointerNum; i++)
            {
                stream.Read(readBuffer, 0, WordSize);
                long cur_pos = stream.Position;
                int str_ptr = BitConverter.ToInt32(readBuffer, 0);

                stream.Position = str_ptr;
                stream.Read(readBuffer, 0, WordSize);
                int str_size = BitConverter.ToInt32(readBuffer, 0);

                byte[] byteBuf = new byte[str_size];

                stream.Read(byteBuf, 0, str_size);
                string output = new string(byteBuf.Select(x => (char)x).ToArray());

                logstream.WriteLine($"Str at: {cur_pos}, ptr_to: {str_ptr}, str: {output}");
                stream.Position = cur_pos;
            }

            return pointerList;
        }

        public static void PointerSwapLoc(ResourcePointer lref, ResourcePointer rref)
        {
            int tmp = lref.Location;
            lref.Location = rref.Location;
            rref.Location = tmp;
        }

        public static readonly Regex json_line_regex = new Regex("\\s*\"(.+)\":\\s*\"(.+)\",", RegexOptions.ECMAScript);

        public class JSONStringEntry
        {
            public string Key;
            public string Ending;
            public string Str;

            public JSONStringEntry(string key, string str)
            {
                Key = key;
                Str = str;
                char[] FormatChars = { '%', '/', 'C' };
                List<char> Ending = new List<char>();

                for (int i = 1; i < str.Length; i++)
                {
                    char C = str[str.Length - i];

                    if (FormatChars.Contains(C))
                        Ending.Add(C);
                    else
                        break;
                }

                Ending.Reverse();
                this.Ending = new string(Ending.ToArray());
            }
        }

        public static void JSONSwapLoc(JSONStringEntry lref, JSONStringEntry rref)
        {
            string tmp = lref.Key;
            lref.Key = rref.Key;
            rref.Key = tmp;
        }

        public static bool JSONStringShuffle(string resource_file, string target_file, Random random, float shufflechance, StreamWriter logstream)
        {
            // actual JSON libraries are complicated, thankfully we can bodge it together with magic and friendship
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            byte[] readBuffer = new byte[WordSize];

            List<JSONStringEntry> strings = new List<JSONStringEntry>();

            using (FileStream stream = new FileStream(resource_file, FileMode.OpenOrCreate))
            {
                StreamReader text_reader = new StreamReader(stream);
                logstream.WriteLine($"Opened {resource_file}.");
                long date = 0;
                

                while (stream.Position != stream.Length)
                {
                    string cur_line = text_reader.ReadLine();
                    if (cur_line == "}" || cur_line == "{") { continue; }

                    Match m = json_line_regex.Match(cur_line);


                  //  logstream.WriteLine($"line: {cur_line} | group_size: {m.Groups.Count}");
                  //  logstream.Flush();

                    string[] match = {m.Groups[1].ToString(), m.Groups[2].ToString() };
                   // logstream.WriteLine($"aaaaa: {match[0]} _ {match[1]}");

                    strings.Add(new JSONStringEntry(match[0], match[1]));
                }
            }
            logstream.WriteLine($"Closed {resource_file}.");

            logstream.WriteLine($"Gathered {strings.Count} JSON String Entries. ");

            string[] bannedStrings = { "_" };
            string[] bannedKeys = { "date" };

            List<JSONStringEntry> good_strings = new List<JSONStringEntry>();
            List<JSONStringEntry> final_list = new List<JSONStringEntry>();

            foreach (JSONStringEntry entry in strings)
            {
                
                if (entry.Str.Length >= 3 && entry.Key.Contains('_'))
                {
                    good_strings.Add(entry);
                }
                else
                {
                    final_list.Add(entry);
                }
            }

            logstream.WriteLine($"Kept {good_strings.Count} good JSON string entries.");
            logstream.WriteLine($"Fastforwarded {final_list.Count} JSON string entries to the final phase.");

            Dictionary<string, List<JSONStringEntry>> stringDict = new Dictionary<string, List<JSONStringEntry>>();
            int totalStrings = 0;

            foreach (JSONStringEntry s in good_strings)
            {
                //if (!string.IsNullOrWhiteSpace(s.Ending))
                //{
                    if (!stringDict.ContainsKey(s.Ending))
                        stringDict[s.Ending] = new List<JSONStringEntry>();

                    stringDict[s.Ending].Add(s);
                    totalStrings++;
                //}
            }

            

            foreach (string ending in stringDict.Keys)
            {
                logstream.WriteLine($"Added {stringDict[ending].Count} JSON string entries of ending <{ending}> to dialogue string List.");

                stringDict[ending].Shuffle(JSONSwapLoc, random);

                final_list = final_list.Concat(stringDict[ending]).ToList();
            }

            using (FileStream out_stream = new FileStream(target_file, FileMode.Create, FileAccess.ReadWrite))
            {
                logstream.WriteLine($"Opened {target_file}.");
                StreamWriter writer = new StreamWriter(out_stream);
                writer.Write("{\n");

                foreach (JSONStringEntry s in final_list)
                {
                    char comma = (s.Key == final_list[final_list.Count - 1].Key) ? ' ' : ',';
                    writer.WriteLine($"\t\"{s.Key}\": \"{s.Str}\"{comma} ");
                }
                writer.Write("}\n");
                writer.Flush();
            }
            logstream.WriteLine($"Closed {target_file}.");


            return false;

        }
}
}
