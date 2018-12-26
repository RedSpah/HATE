using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace HATE.Core
{
    //TODO: Make a cmd version to see if that makes it work on macOS 14.10
    //TODO: Make sure you don't shuffle the string obj_writer_slash_Draw_0_gml_147_0 ("||")
    public class StringPointer
    {
        public ResourcePointer Base;
        public string Ending;
        public string Str;

        public StringPointer(ResourcePointer ptr, string str)
        {
            Base = ptr;
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

    public class ResourcePointer
    {
        public int Address;
        public int Location;

        public ResourcePointer(int ptr, int loc) { Address = ptr; Location = loc; }
        public ResourcePointer(byte[] ptr, int loc) { Address = BitConverter.ToInt32(ptr, 0); Location = loc; }
    }



    partial class MainForm
    {
        public bool ShuffleAudio_Func(Random random, float chance, StreamWriter logstream)
        {
            return Shuffle.LoadDataAndFind("SOND", random, chance, logstream, _dataWin, Shuffle.SimpleShuffle) && 
                   Shuffle.LoadDataAndFind("AUDO", random, chance, logstream, _dataWin, Shuffle.SimpleShuffle);               
        }

        public bool ShuffleBG_Func(Random random, float chance, StreamWriter logstream)
        {
            return Shuffle.LoadDataAndFind("BGND", random, chance, logstream, _dataWin, Shuffle.SimpleShuffle);              
        }

        public bool ShuffleFont_Func(Random random, float chance, StreamWriter logstream)
        {
            return Shuffle.LoadDataAndFind("FONT", random, chance, logstream, _dataWin, Shuffle.SimpleShuffle);
        }

        public bool HitboxFix_Func(Random random_, float chance, StreamWriter logstream_)
        {
            return Shuffle.LoadDataAndFind("SPRT", random_, chance, logstream_, _dataWin, Shuffle.ComplexShuffle(Shuffle.SimpleAccumulator, HitboxFix_Shuffler, Shuffle.SimpleWriter));
        }

        public bool ShuffleGFX_Func(Random random_, float chance, StreamWriter logstream_)
        {
            return Shuffle.LoadDataAndFind("SPRT", random_, chance, logstream_, _dataWin, Shuffle.ComplexShuffle(ShuffleGFX_Accumulator, Shuffle.SimpleShuffler, Shuffle.SimpleWriter));
        }

        public bool ShuffleText_Func(Random random_, float chance, StreamWriter logstream_)
        {
            if (lblGameName.Text == "Deltarune")
            {
                return Shuffle.JSONStringShuffle("./lang/lang_en.json", "./lang/lang_en.json", random_, chance, logstream_) &&
                       Shuffle.JSONStringShuffle("./lang/lang_ja.json", "./lang/lang_ja.json", random_, chance, logstream_);
            }
            else
            {
                MessageBox.Show(lblGameName.Text);
                return Shuffle.LoadDataAndFind("STRG", random_, chance, logstream_, _dataWin, Shuffle.ComplexShuffle(Shuffle.SimpleAccumulator, ShuffleText_Shuffler, Shuffle.SimpleWriter));
            }
        }

        // TODO: clean this
        public List<ResourcePointer> ShuffleGFX_Accumulator(FileStream stream, Random random, float shufflechance, StreamWriter logstream)
        {
            byte[] readBuffer = new byte[Shuffle.WordSize];
            int pointerNum = 0;
            long pointerArrayBegin = 0;
            List<ResourcePointer> pointerList = new List<ResourcePointer>();

            stream.Read(readBuffer, 0, 4);
            pointerNum = BitConverter.ToInt32(readBuffer, 0);
            pointerArrayBegin = stream.Position;

            for (int i = 0; i < pointerNum; i++)
            {
                if (random.NextDouble() < shufflechance)
                {
                    byte[] _tmp = new byte[4], _tmp2 = new byte[4];
                    stream.Read(_tmp, 0, 4);
                    ResourcePointer ptr = new ResourcePointer(_tmp, (int)stream.Position - 4);
                    long pos = stream.Position;
                    stream.Position = ptr.Address;
                    stream.Read(_tmp2, 0, 4);
                    stream.Position = BitConverter.ToInt32(_tmp2, 0);

                    List<byte> byteString = new List<byte>();
                    bool stringBegun = false;

                    for (int j = 0; j < 128; j++)
                    {
                        byteString.Add((byte)stream.ReadByte());

                        if (byteString[byteString.Count - 1] == 0 && stringBegun)
                            break;

                        if (byteString[byteString.Count - 1] != 0)
                            stringBegun = true;
                    }

                    string convertedString = new string(byteString.Where(x => x == '_' || (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z') || (x >= '0' && x <= '9')).Select(x => (char)x).ToArray());

                    if (!_friskSpriteHandles.Contains(convertedString.Trim()) || _friskMode)
                        pointerList.Add(ptr);

                    stream.Position = pos;
                }
            }

            logstream.WriteLine($"Added {pointerList.Count} out of {pointerNum} sprite pointers to SPRT List.");

            return pointerList;
        }

        // TODO: implement this
        public List<ResourcePointer> HitboxFix_Shuffler(FileStream stream, Random random, float shufflechance, StreamWriter logstream, List<ResourcePointer> pointerlist)
        {
            foreach (ResourcePointer spriteptr in pointerlist)
            {
                // Previous implementation of HitboxFix didn't work at all and was unsafe to boot, better implementation will be added later.
            }
            logstream.WriteLine($"Wrote {pointerlist.Count} hitboxes to {_dataWin}.");

            return pointerlist;
        }

        // TODO: delete this
        // TODO: clean this
        public List<ResourcePointer> ShuffleText_Shuffler(FileStream stream, Random random, float shufflechance, StreamWriter logstream, List<ResourcePointer> pointerlist)
        {
            string[] bannedStrings = { "_" };
            List<ResourcePointer> shuffledPointerList = new List<ResourcePointer>();
            List<StringPointer> strPointerList = new List<StringPointer>();


            for (int i = 0; i < pointerlist.Count; i++)
            {
                stream.Position = pointerlist[i].Address;
                byte StrlenByte = (byte)stream.ReadByte();
                stream.Position += 3;
                List<byte> ByteString = new List<byte>();

                for (int j = 0; j < StrlenByte; j++)
                {
                    byte TMP = (byte)stream.ReadByte();

                    if (TMP == 0)
                        break;
                    else
                        ByteString.Add(TMP);

                }

                string convertedString = new string(ByteString.Select(x => (char)x).ToArray());

                if (StrlenByte >= 3 && !bannedStrings.Any(convertedString.Contains))
                    strPointerList.Add(new StringPointer(pointerlist[i], convertedString));
            }

            logstream.WriteLine($"Added {strPointerList.Count} good string pointers to SprPointerList.");

            Dictionary<string, List<ResourcePointer>> stringDict = new Dictionary<string, List<ResourcePointer>>();
            int totalStrings = 0;

            foreach (StringPointer s in strPointerList)
            {
                if (!string.IsNullOrWhiteSpace(s.Ending))
                {
                    if (!stringDict.ContainsKey(s.Ending))
                        stringDict[s.Ending] = new List<ResourcePointer>();

                    stringDict[s.Ending].Add(s.Base);
                    totalStrings++;
                }
            }

            foreach (string ending in stringDict.Keys)
            {
                logstream.WriteLine($"Added {stringDict[ending].Count} string pointers of ending {ending} to dialogue string List.");

                stringDict[ending].Shuffle(Shuffle.PointerSwapLoc, random);

                shuffledPointerList = shuffledPointerList.Concat(stringDict[ending]).ToList();
            }

            return shuffledPointerList;
        }

        public void DebugListChunks(string resource_file, StreamWriter logstream)
        {
            byte[] readBuffer = new byte[4];

            using (FileStream stream = new FileStream(resource_file, FileMode.OpenOrCreate))
            {
                logstream.WriteLine($"Opened {resource_file}.");
                stream.Position = 8;

                int dataSegmentCounter = 0;

                while (stream.Position != stream.Length)
                {
                    stream.Read(readBuffer, 0, 4);
                    string headerName = new string(readBuffer.Select(x => (char)x).ToArray());
                    stream.Read(readBuffer, 0, 4);
                    int chunk_size = BitConverter.ToInt32(readBuffer, 0);
                    logstream.WriteLine($"Chunk #{dataSegmentCounter}: {headerName} found, size {chunk_size}.");
                    stream.Position += chunk_size;
                    dataSegmentCounter++;
                }

                logstream.WriteLine($"Closed {resource_file}.");
            }
        }

        
    }
}
