using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HATE
{
    static class Shuffle
    {
        public const int WordSize = 4;
        public const int NumDataSegments = 23;

        public static bool LoadDataAndFind(string seeked_header, Random random, float shufflechance, StreamWriter logstream, string resource_file, Func<FileStream, Random, float, string, StreamWriter, bool> shufflefunc)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            byte[] readBuffer = new byte[WordSize];

            using (FileStream stream = new FileStream(resource_file, FileMode.OpenOrCreate))
            {
                logstream.WriteLine("Opened " + resource_file + ".");
                stream.Position = 8;

                int dataSegmentCounter = 0;

                while (dataSegmentCounter++ < NumDataSegments)
                {
                    stream.Read(readBuffer, 0, WordSize);

                    string headerName = new string(readBuffer.Select(x => (char)x).ToArray());

                    if (headerName == seeked_header)
                    {
                        logstream.WriteLine(seeked_header + " Memory Region Found at " + stream.Position.ToString("X") + ".");

                        stream.Position += WordSize;

                        try
                        {
                            if (!shufflefunc(stream, random, shufflechance, seeked_header, logstream))
                            {
                                logstream.WriteLine("An Error Occured While Attempting To Modify " + seeked_header + " Memory Region.");
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            logstream.Write("Exception Caught While Attempting To Modify " + seeked_header + " Memory Region. -> " + e);
                            throw;
                        }

                        logstream.WriteLine(seeked_header + " Memory Region Modified Successfully.");
                        logstream.WriteLine("Closed " + resource_file + ".");
                        return true;
                    }

                    stream.Read(readBuffer, 0, WordSize);
                    stream.Position += BitConverter.ToInt32(readBuffer, 0);
                }

                logstream.WriteLine("Could not find " + seeked_header + "Memory Region.");

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
                    logstream.WriteLine("Added " + pointerList.Count + " pointers to " + header + " List.");
                    step = ComplexShuffleStep.Shuffling;
                    pointerList = shuffler(stream, random, chance, logstream, pointerList);
                    step = ComplexShuffleStep.SecondLog;
                    logstream.WriteLine("Shuffled " + pointerList.Count + " pointers to " + header + " List.");
                    step = ComplexShuffleStep.Writing;
                    success = writer(stream, random, chance, logstream, pointerList);
                    step = ComplexShuffleStep.ThirdLog;
                    logstream.WriteLine("Written " + pointerList.Count + " shuffled pointers to " + header + " List.");
                }
                catch (Exception ex)
                {
                    logstream.WriteLine($"Caught exception [{ex.ToString()}] while editing {header} memory block, during step {step.ToString()}.");
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
                {
                    pointerList.Add(new ResourcePointer(BitConverter.ToInt32(readBuffer, 0), (int)(stream.Position - WordSize)));
                }
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


        public static void PointerSwapLoc(ResourcePointer lref, ResourcePointer rref)
        {
            int tmp = lref.Location;
            lref.Location = rref.Location;
            rref.Location = tmp;
        }

    }
}
