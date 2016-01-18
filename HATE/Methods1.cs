using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HATE
{
    class StringCont
    {
        public byte[] badr;
        public byte[] bstr;
        public int adr;
        //public bool rstr;
        public int loc;

        public StringCont(byte[] a, int l, List<byte> y) { badr = a; adr = BitConverter.ToInt32(a, 0); loc = l; bstr = y.ToArray(); }
    }

    partial class HATE
    {

        bool ShuffleAudio_Func()
        {
            byte[] sprtheader = { 0x42, 0x47, 0x4E, 0x44 };
            byte[] readbuf = new byte[4];

            FileStream data = Safe.OpenFileStream("./data.win");
            if (data == null) { return false; }

            data.Position = 23300000;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x41 && readbuf[1] == 0x55 && readbuf[2] == 0x44 && readbuf[3] == 0x4F) { Console.WriteLine("audio match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
            int strnum = BitConverter.ToInt32(readbuf, 0);
            long strptrbgn = data.Position;
            List<byte[]> sprptrs = new List<byte[]>();
            //StringCont[] strptrs = new StringCont[strnum];
            Console.WriteLine(strnum);

            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                sprptrs.Add(a);
                // Console.WriteLine(sprptrs[sprptrs.Count - 1]);
            }

            sprptrs.Shuffle();

            data.Position = strptrbgn;

            for (int i = 0; i < strnum; i++)
            {
                data.Write(sprptrs[i], 0, 4);
            }

            data.Position = 0;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x53 && readbuf[1] == 0x4f && readbuf[2] == 0x4e && readbuf[3] == 0x44) { Console.WriteLine("audio match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
             strnum = BitConverter.ToInt32(readbuf, 0);
             strptrbgn = data.Position;
            sprptrs = new List<byte[]>();
            //StringCont[] strptrs = new StringCont[strnum];
            Console.WriteLine(strnum);

            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                sprptrs.Add(a);
                // Console.WriteLine(sprptrs[sprptrs.Count - 1]);
            }

            sprptrs.Shuffle();

            data.Position = strptrbgn;

            for (int i = 0; i < strnum; i++)
            {
                data.Write(sprptrs[i], 0, 4);
            }




            data.Close();
            Console.WriteLine("audio end");
            return true;
        }


        bool ShuffleRoom_Func()
        {
            byte[] sprtheader = { 0x42, 0x47, 0x4E, 0x44 };
            byte[] readbuf = new byte[4];

            FileStream data = Safe.OpenFileStream("./data.win");
            if (data == null) { return false; }

            data.Position = 1900000;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x42 && readbuf[1] == 0x47 && readbuf[2] == 0x4E && readbuf[3] == 0x44) { Console.WriteLine("bg match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
            int strnum = BitConverter.ToInt32(readbuf, 0);
            long strptrbgn = data.Position;
            List<byte[]> sprptrs = new List<byte[]>();
            //StringCont[] strptrs = new StringCont[strnum];
            Console.WriteLine(strnum);

            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                sprptrs.Add(a);
                // Console.WriteLine(sprptrs[sprptrs.Count - 1]);
            }

            sprptrs.Shuffle();

            data.Position = strptrbgn;

            for (int i = 0; i < strnum; i++)
            {
                data.Write(sprptrs[i], 0, 4);
            }
            data.Close();
            Console.WriteLine("bg end");
            return true;
        }


        bool ShuffleFont_Func()
        {
            byte[] sprtheader = { 0x53, 0x50, 0x52, 0x54 };
            byte[] readbuf = new byte[4];

            FileStream data = Safe.OpenFileStream("./data.win");
            if (data == null) { return false; }

            data.Position = 1900000;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x46 && readbuf[1] == 0x4F && readbuf[2] == 0x4E && readbuf[3] == 0x54) { Console.WriteLine("font match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
            int strnum = BitConverter.ToInt32(readbuf, 0);
            long strptrbgn = data.Position;
            List<byte[]> sprptrs = new List<byte[]>();
            //StringCont[] strptrs = new StringCont[strnum];
            Console.WriteLine(strnum);

            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                sprptrs.Add(a);
                // Console.WriteLine(sprptrs[sprptrs.Count - 1]);
            }

            sprptrs.Shuffle();

            data.Position = strptrbgn;

            for (int i = 0; i < strnum; i++)
            {
                data.Write(sprptrs[i], 0, 4);
            }
            data.Close();
            Console.WriteLine("font end");
            return true;
        }

        bool HitboxFix_Func()
        {
            byte[] sprtheader = { 0x53, 0x50, 0x52, 0x54 };
            byte[] readbuf = new byte[4];

            FileStream data = Safe.OpenFileStream("./data.win");
            if (data == null) { return false; }

            data.Position = 15000;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x53 && readbuf[1] == 0x50 && readbuf[2] == 0x52 && readbuf[3] == 0x54) { Console.WriteLine("hitbox match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
            int strnum = BitConverter.ToInt32(readbuf, 0);
            long strptrbgn = data.Position;
            List<byte[]> sprptrs = new List<byte[]>();
            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                sprptrs.Add(a);
                // Console.WriteLine(sprptrs[sprptrs.Count - 1]);
            }
            for (int i = 0; i < strnum - 1; i++)
            {
                //Console.WriteLine("pass " + i + " begin, addr " + BitConverter.ToInt32(sprptrs[i], 0));
                int sx, sy, sz;
                data.Position = BitConverter.ToInt32(sprptrs[i], 0);
                data.Position += 4;
                data.Read(readbuf, 0, 4);
                sx = BitConverter.ToInt32(readbuf, 0);
                data.Read(readbuf, 0, 4);
                sy = BitConverter.ToInt32(readbuf, 0);
                sz = sx * sy / 8;
                Console.WriteLine(sz);
                if (sz < 8) { continue; }
                data.Position = BitConverter.ToInt32(sprptrs[i + 1], 0) - sz;
                int c = 8;
                data.Write(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, 0, 8);
                while (c < sz) { data.Write(new byte[] { 0 }, 0, 1); c++; }

            }
            data.Close();
            Console.WriteLine("hitbox end");
            return true;
        }

        bool ShuffleGFX_Func()
        {
            byte[] sprtheader = { 0x53, 0x50, 0x52, 0x54 };
            byte[] readbuf = new byte[4];

            FileStream data = Safe.OpenFileStream("./data.win");
            if (data == null) { return false; }

            data.Position = 15000;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x53 && readbuf[1] == 0x50 && readbuf[2] == 0x52 && readbuf[3] == 0x54) { Console.WriteLine("sprite match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
            int strnum = BitConverter.ToInt32(readbuf, 0);
            long strptrbgn = data.Position;
            List<byte[]> sprptrs = new List<byte[]>();
            //StringCont[] strptrs = new StringCont[strnum];
            Console.WriteLine(strnum);

            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                sprptrs.Add(a);
                // Console.WriteLine(sprptrs[sprptrs.Count - 1]);
            }

            sprptrs.Shuffle();

            data.Position = strptrbgn;

            for (int i = 0; i < strnum; i++)
            {
                data.Write(sprptrs[i], 0, 4);
            }
            data.Close();
            Console.WriteLine("sprite end");
            return true;
        }


        bool ShuffleStrings_Func()
        {
            byte[] strgheader = { 0x53, 0x54, 0x52, 0x47 };
            byte[] readbuf = new byte[4];

            FileStream data = Safe.OpenFileStream("./data.win");
            if (data == null) { return false; }

            data.Position = 10750000;

            for (int i = 0; i < 2137000; i++)
            {
                data.Read(readbuf, 0, 4);
                data.Position -= 3;
                if (readbuf[0] == 0x53 && readbuf[1] == 0x54 && readbuf[2] == 0x52 && readbuf[3] == 0x47) { Console.WriteLine("string match"); break; }
            }

            data.Position += 7;
            data.Read(readbuf, 0, 4);
            int strnum = BitConverter.ToInt32(readbuf, 0);
            long strptrbgn = data.Position;
            StringCont[] strptrs = new StringCont[strnum];
            Console.WriteLine(strnum);

            for (int i = 0; i < strnum; i++)
            {
                byte[] a = new byte[4];
                data.Read(a, 0, 4);
                strptrs[i] = new StringCont(a, (int)data.Position, new List<byte>());
            }
            List<StringCont> dialstr = new List<StringCont>();
            List<StringCont> goodstr = new List<StringCont>();
            List<StringCont> batlstr = new List<StringCont>();
            string[] bannedwords = { "script", "_", "string", "round", "float", "random", "floor", "choose", "ceil", "abs", "arccos",
"arcsin",
"arctan",
"arctan2", "chr", "sin", "cos", "exp", "frac", "ln", "log10", "log2", "logn", "max", "max3", "mean", "median", "min", "min3", "ord","power",
"radtodeg", "degtorad", "ctan",
"random",
"real","sign","sleep","sqr","sqrt","tan","argument"};

            foreach (StringCont s in strptrs)
            {
                data.Position = s.adr;
                byte control = (byte)data.ReadByte();

                List<byte> str = new List<byte>();
                bool smth = false;
                while (true)
                {
                    str.Add((byte)data.ReadByte());
                    if (str[str.Count - 1] == 0 && smth) { break; }
                    if (str[str.Count - 1] != 0) { smth = true; }
                    //if (control == 8 || control == 18 || control == 12 || control == 4) { break; }
                }

                string rstr = new string(str.Select(x => (char)x).ToArray());

                /* if (rstr.Contains("/%%"))
                 {
                     dialstr.Add(s);
                     // Console.WriteLine(rstr);

                 }
                /* else if (rstr.Contains("%%%"))
                 {
                     batlstr.Add(s);
                 }*/
                if (rstr.Length >= 3)
                {

                    if (rstr.Contains("/%") && !(bannedwords.Any(rstr.Contains))) { dialstr.Add(new StringCont(s.badr, s.loc, str)); }
                    /*else
                    { goodstr.Add(new StringCont(s.badr, s.loc, str)); }*/

                    goodstr.Add(s);
                }

                //Console.WriteLine(adr);
                /*if (!(bannedwords.Any(rstr.Contains)) && rstr.Length >= 4)
                {

                    data.Position -= 4;
                    data.Write(new byte[] { (byte)'/', (byte)'%', (byte)'%' }, 0, 3);
                    goodstr.Add(s);
                }*/
            }
            // dialstr = dialstr.OrderBy(x => RNG.Next()).ToList();
            // goodstr = goodstr.OrderBy(x => RNG.Next()).ToList();
            foreach (StringCont y in dialstr) { y.bstr = Ext.Garble(y.bstr, 1); }
            //Ext.Shuffle(goodstr);
            //Ext.Shuffle(dialstr);

            for (int i = 0; i < dialstr.Count; i++)
            {
                data.Position = dialstr[i].adr + 1;


                // Console.WriteLine(BitConverter.ToInt32(strptrs[i], 0));
                data.Write(dialstr[i].bstr, 0, dialstr[i].bstr.Count());


                data.Position = dialstr[(i + 1) % (dialstr.Count - 1)].adr;
                // Console.WriteLine(BitConverter.ToInt32(strptrs[i], 0));
                data.Write(dialstr[i].badr, 0, 4);
            }

            for (int i = 0; i < goodstr.Count; i++)
            {
                data.Position = goodstr[(i + 1) % (goodstr.Count - 1)].adr;
                // Console.WriteLine(BitConverter.ToInt32(strptrs[i], 0));
                data.Write(goodstr[i].badr, 0, 4);
            }

            /*  for (int i = 0; i < batlstr.Count; i++)
              {
                  data.Position = batlstr[(i + 1) % (batlstr.Count - 1)].loc;
                  // Console.WriteLine(BitConverter.ToInt32(strptrs[i], 0));
                  data.Write(batlstr[i].badr, 0, 4);
              }*/

            data.Position = strptrbgn;



            data.Close();
            Console.WriteLine("string end");
            return true;
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
                if (((ar[i] > 48 && ar[i] < 123)) && (HATE.RNG.NextDouble() < chnc))
                {

                    a.Add((byte)(HATE.RNG.Next(80) + 47));

                }
                else
                {
                    a.Add(ar[i]);
                }
            }
            for (int i = 0; i < a.Count; i++)
            {
                // Console.Write(a[i] + " - " + ar[i] + '\n');
            }
            Console.WriteLine(new string(a.Select(x => (char)x).ToArray()));
            return a.ToArray();
        }
    }


}
