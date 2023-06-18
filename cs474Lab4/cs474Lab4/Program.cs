using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace cs474Lab4
{
    class Program
    {
        const int sectionSize = 20000000;

        private static int CC = Environment.ProcessorCount;

        private static readonly int Chunk = sectionSize / CC;

        //This will get the height value of of a specified x value location.
        static double GetHeight(double location) =>  (4 / (1 + (location * location)));

        //This will get the X position inbetween given a section size and n.
        static double GetXValue(int n, double section) => n * section + section / 2;

        /// <summary>
        /// This method will sequentially calculate the area under the curve.
        /// </summary>
        /// <returns>The total value</returns>
        static double SequentialAreaUnderSum()
        {
            double total = 0;

            //loops through every section and adds rectangles.
            for (int i = 0; i < sectionSize; i++)
                total += (GetHeight(GetXValue(i, 1.0 * 1 / sectionSize)) * 1 / sectionSize);
            return total;
        }

        /// <summary>
        /// This method will parallelly calculate the area under the curve.
        /// </summary>
        /// <returns>The total value</returns>
        static double ParallelAreaUnderSum()
        {
            double total = 0;
            Mutex mLock = new Mutex();

            //loops through every section and adds rectangles.
            Parallel.For(0, Convert.ToInt32(Math.Ceiling(1.0 * sectionSize / Chunk)), j =>
            {
                int start = j * Chunk;
                int end = (j + 1) * Chunk;
                double localTotal = 0;
                for (int i = start; i < end; i++)
                {
                    //Break if we go over the specified length. (More loops than are sections)
                    if (i >= sectionSize)
                        break;
                    localTotal += (GetHeight(GetXValue(i, 1.0 * 1 / sectionSize)) * 1 / sectionSize);
                }
                mLock.WaitOne();
                total += localTotal;
                mLock.ReleaseMutex();
            });
            return total;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Area Under Curve Algorithm Comparison");
            Console.WriteLine($"Section Size: {sectionSize}\nChunk Size: {Chunk}\n");

            Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Sequential Algorithm:");
            stopWatch.Start();
            double result1 = SequentialAreaUnderSum();
            stopWatch.Stop();
            Console.WriteLine($"Result: {result1}\nTime: {stopWatch.ElapsedMilliseconds}ms\n");

            stopWatch.Reset();
            Console.WriteLine("Parallel Algorithm:");
            stopWatch.Start();
            double result2 = ParallelAreaUnderSum();
            stopWatch.Stop();
            Console.WriteLine($"Result: {result2}\nTime: {stopWatch.ElapsedMilliseconds}ms\n");
        }
    }
}
