using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace HATE
{
    struct StringPointer
    {
        public Pointer Base;
        public string Ending;
        public byte[] Str;

        public StringPointer(Pointer ptr, byte[] str)
        {
            Base = ptr;
            Str = str;
            char[] FormatChars = { '%', '/' };
            List<char> Ending = new List<char>();

            for (int i = 1; i < str.Length; i++)
            {
                char C = (char)str[str.Length - i];

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

    struct Pointer
    {
        public byte[] Ptr;
        public long PtrLocation;

        public Pointer(byte[] ptr, long loc) { Ptr = ptr; PtrLocation = loc; }
    }

    partial class HATE
    {
        public bool LoadDataAndFind(string header, long loc, float shufflechance, Func<FileStream, float, string, bool> Action)
        {
            byte[] ToFind = header.ToCharArray().Select(x => (byte)x).ToArray();
            byte[] ReadBuffer = new byte[ToFind.Length];
            FileStream Data = Safe.OpenFileStream("./data.win");
            if (Data == null) { return false; }

            Data.Position = loc;

            for (int i = 0; i < 2137420; i++)
            {
                Data.Read(ReadBuffer, 0, ToFind.Length);
                Data.Position -= 3;

                if (ReadBuffer.Select((value, index) => value == ToFind[index]).All(x => x))
                {
                    Console.WriteLine(header + " Memory Region Found at " + Data.Position.ToString("X") + ".");
                    Data.Position += 7;

                    if (!Action(Data, shufflechance, header))
                    {
                        Console.WriteLine("An Error Occured While Attempting To Modify " + header + " Memory Region.");

                        Data.Close();
                        return false;
                    }

                    Data.Close();
                    Console.WriteLine(header + " Memory Region Modified Successfully.");
                    return true;
                }
            }

            Console.WriteLine("Error: " + header + " Memory Region Not Found.");
            return false;
        }

        public bool SimpleShuffle(FileStream Data, float shufflechance, string header)
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
                    byte[] _tmp = new byte[4];
                    Data.Read(_tmp, 0, 4);
                    PointerList.Add(new Pointer(_tmp, Data.Position - 4));
                }
            }

            PointerList.Shuffle();
            Data.Position = PointerArrayBegin;

            for (int i = 0; i < PointerList.Count; i++)
            {
                Data.Position = PointerList[(i + 1) % PointerList.Count].PtrLocation;
                Data.Write(PointerList[i].Ptr, 0, 4);
            }

            return true;
        }

        public bool ShuffleAudio_Func(float chance)
        {
            return LoadDataAndFind("AUDO", 23300000, chance, SimpleShuffle) && LoadDataAndFind("SOND", 0, chance, SimpleShuffle);
        }

        public bool ShuffleBG_Func(float chance)
        {
            return LoadDataAndFind("BGND", 1900000, chance, SimpleShuffle);
        }

        public bool ShuffleFont_Func(float chance)
        {
            return LoadDataAndFind("FONT", 1900000, chance, SimpleShuffle);
        }

        public bool HitboxFix_Func(float chance)
        {
            return LoadDataAndFind("SPRT", 15000, chance, delegate (FileStream Data, float shufflechance, string header)
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

                return true;
            });
        }

        public bool ShuffleGFX_Func(float chance)
        {
            return LoadDataAndFind("SPRT", 15000, chance, delegate (FileStream Data, float shufflechance, string header)
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

                PointerList.Shuffle();
                Data.Position = PointerArrayBegin;

                for (int i = 0; i < PointerList.Count; i++)
                {
                    Data.Position = PointerList[(i + 1) % PointerList.Count].PtrLocation;
                    Data.Write(PointerList[i].Ptr, 0, 4);
                }

                return true;
            });
        }

        public bool ShuffleText_Func(float chance)
        {
            return LoadDataAndFind("STRG", 11100000, chance, delegate (FileStream Data, float shufflechance, string header)
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
                    byte[] PointerBuffer = new byte[4];
                    Data.Read(PointerBuffer, 0, 4);
                    PointerList.Add(new Pointer(PointerBuffer, Data.Position - 4));
                }

                for (int i = 0; i < PointerList.Count; i++)
                {
                    Data.Position = BitConverter.ToInt32(PointerList[i].Ptr, 0);
                    byte StrlenByte = (byte)Data.ReadByte();
                    List<byte> ByteString = new List<byte>();
                    bool StringBegun = false;

                    for (int j = -3; j < StrlenByte; j++)
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

                    string ConvertedString = new string(ByteString.Select(x => (char)x).ToArray());

                    if (StrlenByte >= 3 && !(BannedStrings.Any(ConvertedString.Contains)))
                    {
                        StrPointerList.Add(new StringPointer(PointerList[i], ByteString.ToArray()));
                    }
                }

                List<Pointer> DialogueStrings = new List<Pointer>();
                List<Pointer> TerminatingStrings = new List<Pointer>();
                List<Pointer> HalfTerminatingStrings = new List<Pointer>(); // idk what they actually do but had to come up with something

                foreach (StringPointer s in StrPointerList)
                {
                    if (s.Ending == "/")
                    {
                        DialogueStrings.Add(s.Base);
                    }
                    else if (s.Ending == "/%%")
                    {
                        TerminatingStrings.Add(s.Base);
                    }
                    else if (s.Ending == "/%")
                    {
                        HalfTerminatingStrings.Add(s.Base);
                    }
                }

                DialogueStrings.Shuffle();
                TerminatingStrings.Shuffle();
                HalfTerminatingStrings.Shuffle();

                for (int i = 0; i < DialogueStrings.Count; i++)
                {
                    Data.Position = DialogueStrings[(i + 1) % DialogueStrings.Count].PtrLocation;
                    Data.Write(DialogueStrings[i].Ptr, 0, 4);
                }

                for (int i = 0; i < TerminatingStrings.Count; i++)
                {
                    Data.Position = TerminatingStrings[(i + 1) % TerminatingStrings.Count].PtrLocation;
                    Data.Write(TerminatingStrings[i].Ptr, 0, 4);
                }

                for (int i = 0; i < HalfTerminatingStrings.Count; i++)
                {
                    Data.Position = HalfTerminatingStrings[(i + 1) % HalfTerminatingStrings.Count].PtrLocation;
                    Data.Write(HalfTerminatingStrings[i].Ptr, 0, 4);
                }

                return true;
            });
        }
    }

    public static class Ext
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = HATE.RNG.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static byte[] Garble(this byte[] ar, float chnc)
        {
            List<byte> a = new List<byte>();

            for (int i = 0; i < ar.Count(); i++)
            {
                if (((ar[i] > 47 && ar[i] < 58) || (ar[i] > 96 && ar[i] < 123) || (ar[i] > 64 && ar[i] < 91)) && (HATE.RNG.NextDouble() < chnc))
                {
                    a.Add((byte)(HATE.RNG.Next(75) + 47));
                }
                else
                {
                    a.Add(ar[i]);
                }
            }
            return a.ToArray();
        }
    }


}
