using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HATE
{
    public class StringPointer
    {
        public Pointer Base;
        public string Ending;
        public string Str;

        public StringPointer(Pointer ptr, string str)
        {
            Base = ptr;
            Str = str;
            char[] FormatChars = { '%', '/', 'C' };
            List<char> Ending = new List<char>();

            for (int i = 1; i < str.Length; i++)
            {
                char C = str[str.Length - i];

                if (FormatChars.Contains(C))
                {
                    Ending.Add(C);
                }
                else
                {
                    break;
                }
            }

            Ending.Reverse();
            this.Ending = new string(Ending.ToArray());
        }
    }

    public class Pointer
    {
        public byte[] Ptr;
        public long PtrLocation;

        public Pointer(byte[] ptr, long loc) { Ptr = ptr; PtrLocation = loc; }
    }

    partial class MainForm
    {
        const int SearchAttempts = 2137420;
        const int WordSize = 4;

        public bool LoadDataAndFind(string header, long loc, Random random, float shufflechance, Func<FileStream, Random, float, string, bool> Action)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            byte[] ToFind = header.ToCharArray().Select(x => (byte)x).ToArray();
            byte[] ReadBuffer = new byte[ToFind.Length];

            using (FileStream Data = new FileStream("./" + DataWin, FileMode.OpenOrCreate))
            {
                LogWriter.WriteLine("Opened " + DataWin + ".");
                Data.Position = loc;

                for (int i = 0; i < SearchAttempts; i++)
                {
                    Data.Read(ReadBuffer, 0, ToFind.Length);

                    if (ReadBuffer.Select((value, index) => value == ToFind[index]).All(x => x))
                    {
                        LogWriter.WriteLine(header + " Memory Region Found at " + Data.Position.ToString("X") + ".");

                        Data.Position += WordSize;

                        try
                        {
                            if (!Action(Data, random, shufflechance, header))
                            {
                                LogWriter.WriteLine("An Error Occured While Attempting To Modify " + header + " Memory Region.");
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            LogWriter.Write("Exception Caught While Attempting To Modify " + header + " Memory Region. -> " + e);
                            throw;
                        }

                        LogWriter.WriteLine(header + " Memory Region Modified Successfully.");
                        LogWriter.WriteLine("Closed " + DataWin + ".");
                        return true;
                    }
                }

                LogWriter.WriteLine("Error: " + header + " Memory Region Not Found.");
            }

            return false;
        }

        public bool SimpleShuffle(FileStream Data, Random random, float shufflechance, string header)
        {
            byte[] ReadBuffer = new byte[WordSize];
            int PointerNum = 0;
            long PointerArrayBegin = 0;
            List<Pointer> PointerList = new List<Pointer>();

            Data.Read(ReadBuffer, 0, WordSize);
            PointerNum = BitConverter.ToInt32(ReadBuffer, 0);
            PointerArrayBegin = Data.Position;

            for (int i = 0; i < PointerNum; i++)
            {
                Data.Position = PointerArrayBegin + i * WordSize;

                if (RNG.NextDouble() < shufflechance)
                {
                    Data.Read(ReadBuffer, 0, WordSize);
                    PointerList.Add(new Pointer(DeepCopy(ReadBuffer), Data.Position - WordSize));
                }
            }

            LogWriter.WriteLine("Added " + PointerList.Count + " pointers to " + header + " List.");

            PointerList.Shuffle(PointerSwapLoc, random);

            foreach (Pointer Ptr in PointerList)
            {
                Data.Position = Ptr.PtrLocation;
                Data.Write(Ptr.Ptr, 0, WordSize);
            }

            LogWriter.WriteLine("Wrote " + PointerList.Count + " pointers to " + DataWin + ".");

            return true;
        }

        public bool ShuffleAudio_Func(Random random, float chance)
        {
            return LoadDataAndFind("AUDO", 23300000, random, chance, SimpleShuffle) && LoadDataAndFind("SOND", 0, random, chance, SimpleShuffle);
        }

        public bool ShuffleBG_Func(Random random, float chance)
        {
            return LoadDataAndFind("BGND", 1900000, random, chance, SimpleShuffle);
        }

        public bool ShuffleFont_Func(Random random, float chance)
        {
            return LoadDataAndFind("FONT", 1900000, random, chance, SimpleShuffle);
        }

        public bool HitboxFix_Func(Random random, float chance)
        {
            return LoadDataAndFind("SPRT", 15000, random, chance, delegate (FileStream Data, Random random, float shufflechance, string header)
            {
                byte[] ReadBuffer = new byte[4];
                int PointerNum = 0;
                long PointerArrayBegin = 0;
                List<Pointer> PointerList = new List<Pointer>();

                Data.Read(ReadBuffer, 0, 4);
                PointerNum = BitConverter.ToInt32(ReadBuffer, 0);
                PointerArrayBegin = Data.Position;

                for (int i = 0; i < PointerNum; i++)
                {
                    byte[] PointerBuffer = new byte[4];
                    Data.Read(PointerBuffer, 0, 4);
                    PointerList.Add(new Pointer(PointerBuffer, Data.Position - 4));
                }
                LogWriter.WriteLine("Added " + PointerNum + " sprite pointers to " + header + " List.");

                for (int i = 0; i < PointerNum - 1; i++)
                {
                    int SpriteSizeX, SpriteSizeY, SpriteHitboxSize;
                    Data.Position = BitConverter.ToInt32(PointerList[i].Ptr, 0) + 4;
                    Data.Read(ReadBuffer, 0, 4);
                    SpriteSizeX = BitConverter.ToInt32(ReadBuffer, 0);
                    Data.Read(ReadBuffer, 0, 4);
                    SpriteSizeY = BitConverter.ToInt32(ReadBuffer, 0);
                    SpriteHitboxSize = SpriteSizeX * SpriteSizeY / 8;

                    if (SpriteHitboxSize < 8 || SpriteHitboxSize > 9001)
                    {
                        continue;
                    }

                    Data.Position = BitConverter.ToInt32(PointerList[i + 1].Ptr, 0) - SpriteHitboxSize;
                    if (SpriteHitboxSize % 2 != 0)
                    {
                        Data.Write(Enumerable.Repeat((byte)0, SpriteHitboxSize / 2 - 3).ToArray(), 0, SpriteHitboxSize / 2 - 3);
                        Data.Write(new byte[] { 255, 255, 255, 255, 255, 255, 255 }, 0, 7);
                        Data.Write(Enumerable.Repeat((byte)0, SpriteHitboxSize / 2 - 3).ToArray(), 0, SpriteHitboxSize / 2 - 3);
                    }
                    else
                    {
                        Data.Write(Enumerable.Repeat((byte)0, SpriteHitboxSize / 2 - 3).ToArray(), 0, SpriteHitboxSize / 2 - 3);
                        Data.Write(new byte[] { 15, 255, 255, 255, 255, 240 }, 0, 6);
                        Data.Write(Enumerable.Repeat((byte)0, SpriteHitboxSize / 2 - 3).ToArray(), 0, SpriteHitboxSize / 2 - 3);
                    }
                }
                LogWriter.WriteLine("Wrote " + PointerNum + " hitboxes to " + DataWin + ".");

                return true;
            });
        }

        public bool ShuffleGFX_Func(Random random_, float chance)
        {
            return LoadDataAndFind("SPRT", 15000, random_, chance, delegate (FileStream Data, Random random, float shufflechance, string header)
            {
                byte[] ReadBuffer = new byte[4];
                int PointerNum = 0;
                long PointerArrayBegin = 0;
                List<Pointer> PointerList = new List<Pointer>();

                Data.Read(ReadBuffer, 0, 4);
                PointerNum = BitConverter.ToInt32(ReadBuffer, 0);
                PointerArrayBegin = Data.Position;

                for (int i = 0; i < PointerNum; i++)
                {
                    if (RNG.NextDouble() < shufflechance)
                    {
                        byte[] _tmp = new byte[4], _tmp2 = new byte[4];
                        Data.Read(_tmp, 0, 4);
                        Pointer Ptr = new Pointer(_tmp, Data.Position - 4);
                        long Pos = Data.Position;
                        Data.Position = BitConverter.ToInt32(Ptr.Ptr, 0);
                        Data.Read(_tmp2, 0, 4);
                        Data.Position = BitConverter.ToInt32(_tmp2, 0);

                        List<byte> ByteString = new List<byte>();
                        bool StringBegun = false;

                        for (int j = 0; j < 128; j++)
                        {
                            ByteString.Add((byte)Data.ReadByte());

                            if (ByteString[ByteString.Count - 1] == 0 && StringBegun)
                            {
                                break;
                            }

                            if (ByteString[ByteString.Count - 1] != 0)
                            {
                                StringBegun = true;
                            }
                        }

                        string ConvertedString = new string(ByteString.Where(x => x == '_' || (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z') || (x >= '0' && x <= '9')).Select(x => (char)x).ToArray());

                        if (!FriskSpriteHandles.Contains(ConvertedString.Trim()) || FriskMode)
                        {
                            PointerList.Add(Ptr);
                        }

                        Data.Position = Pos;
                    }
                }

                LogWriter.WriteLine("Added " + PointerList.Count + " out of " + PointerNum + " sprite pointers to " + header + " List.");

                PointerList.Shuffle(delegate (Pointer LeftPtr, Pointer RightPtr)
                {
                    long Tmp = LeftPtr.PtrLocation;
                    LeftPtr.PtrLocation = RightPtr.PtrLocation;
                    RightPtr.PtrLocation = Tmp;
                }, random);

                LogWriter.WriteLine("Shuffled " + PointerList.Count + " sprite pointers.");

                Data.Position = PointerArrayBegin;

                for (int i = 0; i < PointerList.Count; i++)
                {
                    Data.Position = PointerList[i].PtrLocation;
                    Data.Write(PointerList[i].Ptr, 0, 4);
                }

                LogWriter.WriteLine("Wrote " + PointerList.Count + " sprite pointers to " + DataWin + ".");

                return true;
            });
        }

        public bool ShuffleText_Func(Random random_, float chance)
        {          
            return LoadDataAndFind("STRG", 10700000, random_, chance, delegate (FileStream Data, Random random, float shufflechance, string header)
            {
                byte[] ReadBuffer = new byte[4];
                int PointerNum = 0;
                long PointerArrayBegin = 0;
                List<Pointer> PointerList = new List<Pointer>();
                List<StringPointer> StrPointerList = new List<StringPointer>();
                string[] BannedStrings = { "_" };

                Data.Read(ReadBuffer, 0, 4);
                PointerNum = BitConverter.ToInt32(ReadBuffer, 0);
                PointerArrayBegin = Data.Position;

                for (int i = 0; i < PointerNum; i++)
                {
                    if (RNG.NextDouble() < shufflechance)
                    {
                        byte[] PointerBuffer = new byte[4];
                        Data.Read(PointerBuffer, 0, 4);
                        PointerList.Add(new Pointer(PointerBuffer, Data.Position - 4));
                    }
                }

                LogWriter.WriteLine("Added " + PointerList.Count + " out of " + PointerNum + " string pointers to STRG List.");

                for (int i = 0; i < PointerList.Count; i++)
                {
                    Data.Position = BitConverter.ToInt32(PointerList[i].Ptr, 0);
                    byte StrlenByte = (byte)Data.ReadByte();
                    Data.Position += 3;
                    List<byte> ByteString = new List<byte>();

                    for (int j = 0; j < StrlenByte; j++)
                    {
                        byte TMP = (byte)Data.ReadByte();

                        if (TMP == 0)
                        {
                            break;
                        }

                        else
                        {
                            ByteString.Add(TMP);
                        }

                    }

                    string ConvertedString = new string(ByteString.Select(x => (char)x).ToArray());

                    if (StrlenByte >= 3 && !(BannedStrings.Any(ConvertedString.Contains)))
                    {
                        StrPointerList.Add(new StringPointer(PointerList[i], ConvertedString));
                    }
                }

                LogWriter.WriteLine("Added " + StrPointerList.Count + " good string pointers to SprPointerList.");

                Dictionary<string, List<Pointer>> StringDict = new Dictionary<string, List<Pointer>>();
                int TotalStrings = 0;

                foreach (StringPointer s in StrPointerList)
                {
                    if (s.Ending != "")
                    {
                        if (!StringDict.ContainsKey(s.Ending))
                        {
                            StringDict[s.Ending] = new List<Pointer>();
                        }

                        StringDict[s.Ending].Add(s.Base);
                        TotalStrings++;
                    }
                }

                foreach (string ending in StringDict.Keys)
                {
                    LogWriter.WriteLine("Added " + StringDict[ending].Count + " string pointers of ending " + ending + " to dialogue string List.");

                    StringDict[ending].Shuffle(delegate (Pointer LeftPtr, Pointer RightPtr)
                    {
                        long Tmp = LeftPtr.PtrLocation;
                        LeftPtr.PtrLocation = RightPtr.PtrLocation;
                        RightPtr.PtrLocation = Tmp;
                    }, random);

                    for (int i = 0; i < StringDict[ending].Count; i++)
                    {
                        Data.Position = StringDict[ending][i].PtrLocation;
                        Data.Write(StringDict[ending][i].Ptr, 0, 4);

                    }
                }

                LogWriter.WriteLine("Wrote " + TotalStrings + " string pointers to " + DataWin + ".");

                return true;
            });
        }

        public void PointerSwapLoc(Pointer lref, Pointer rref)
        {
            long tmp = lref.PtrLocation;
            lref.PtrLocation = rref.PtrLocation;
            rref.PtrLocation = tmp;
        }

        public T[] DeepCopy<T>(T[] Array) where T : struct
        {
            T[] tmp = new T[Array.Length];

            for (int i = 0; i < Array.Length; i++)
            {
                tmp[i] = Array[i];
            }

            return tmp;
        }

    }  
}
