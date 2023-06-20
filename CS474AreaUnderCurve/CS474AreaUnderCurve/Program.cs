using System.Diagnostics;

namespace CS474AreaUnderCurve
{
    class Program
    {
        private static readonly int CC = Environment.ProcessorCount;

        //This will get the height value of a specified x value location.
        static double GetHeight(double location) => (4 / (1 + (location * location)));

        //This will get the X position inbetween given a section size and n.
        static double GetXValue(int n, double section) => n * section + section / 2;

        /// <summary>
        /// This method will sequentially calculate the area under the curve.
        /// </summary>
        /// <returns>The total value.</returns>
        static double SequentialAreaUnderSum(int size)
        {
            double total = 0;

            //loops through every section and adds rectangles.
            for (int i = 0; i < size; i++)
                total += (GetHeight(GetXValue(i, 1.0 * 1 / size)) * 1 / size);
            return total;
        }

        /// <summary>
        /// This method will parallelly calculate the area under the curve.
        /// </summary>
        /// <returns>The total value.</returns>
        static double ParallelAreaUnderSum(int size, int chunk)
        {
            double total = 0;
            Mutex mLock = new Mutex();

            //loops through every section and adds rectangles.
            Parallel.For(0, Convert.ToInt32(Math.Ceiling(1.0 * size / chunk)), j =>
            {
                int start = j * chunk;
                int end = (j + 1) * chunk;
                double localTotal = 0;
                for (int i = start; i < end; i++)
                {
                    //Break if we go over the specified length. (More loops than are sections)
                    if (i >= size)
                        break;
                    localTotal += (GetHeight(GetXValue(i, 1.0 * 1 / size)) * 1 / size);
                }
                mLock.WaitOne();
                total += localTotal;
                mLock.ReleaseMutex();
            });
            return total;
        }
        static bool CompareCheck(double val1, double val2) => val1 == val2;

        static void Main()
        {
            Stopwatch stopWatch = new();

            int[] sizes = { 100, 1000, 1000000, 2000000 };

            Console.WriteLine("Area Under Curve Algorithm Comparison");
            Console.WriteLine($"Processors:{CC}\n");

            foreach(int size in sizes)
            {
                // Setting up variables.
                int chunkSize = size / CC;
                Console.WriteLine($"Size: {size}\nChunk: {chunkSize}");

                // Sequential run.
                stopWatch.Start();
                double result1 = SequentialAreaUnderSum(size);
                stopWatch.Stop();
                Console.WriteLine($"Sequential Algorithm Time: {stopWatch.ElapsedMilliseconds}ms");
                stopWatch.Reset();

                // Parallel run.
                stopWatch.Start();
                double result2 = ParallelAreaUnderSum(size, chunkSize);
                stopWatch.Stop();
                Console.WriteLine($"Parallel Algorithm Time: {stopWatch.ElapsedMilliseconds}ms");
                stopWatch.Reset();
                Console.WriteLine($"Equal Check: {CompareCheck(Math.Round(result1), Math.Round(result2))}\n");
            }
        }
    }
}