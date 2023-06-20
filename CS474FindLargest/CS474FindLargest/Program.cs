using System.Diagnostics;

namespace CS474FindLargest
{
    class Program
    {
        private static readonly int CC = Environment.ProcessorCount;

        /// <summary>
        /// This function will return the largest number of an inputted array using a sequential search.
        /// </summary>
        /// <param name="array">The array that will be searched.</param>
        /// <returns>The largest number integer in array.</returns>
        static int LargestNumberFinderSequential(int[] array)
        {
            int largestNum = array[0];

            for (int i = 1; i < array.Length; i++)
                if (array[i] > largestNum)
                    largestNum = array[i];
            return largestNum;
        }

        /// <summary>
        /// This function will return the largest number of an inputted array using a parallel search.
        /// </summary>
        /// <param name="array">The array that will be searched.</param>
        /// <param name="chunk">The size of the chunk each process will look through.</param>
        /// <returns>The largest number integer in array.</returns>
        static int LargestNumberFinderParallel(int[] array, int chunk)
        {
            int largestNum = array[0];

            Parallel.For(0, array.Length / chunk, i =>
            {
                //Calculate location of chunk.
                int iSize = i * chunk;
                //Loop though a chunk of the array.
                for (int j = iSize; j < (iSize + chunk); j++)
                    if (array[j] > largestNum)
                        largestNum = array[j];
            });
            return largestNum;
        }

        static bool CompareCheck(int val1, int val2) => val1 == val2;

        static void Main()
        {
            Random random = new();
            Stopwatch stopWatch = new();
            int[] sizes = { 1000, 1000000, 2000000, 20000000 };

            Console.WriteLine("Finding Largest Number Algorithm Comparison");
            Console.WriteLine($"Processors: {CC}\n");

            foreach (int size in sizes)
            {
                //Setting up variables.
                int chunkSize = size / CC;
                int[] array = new int[size];
                Console.WriteLine($"SIZE: {size}\nChunk Size: {chunkSize}");

                //Filling the array sequentially with random numbers.
                for (int j = 0; j < size; j++)
                    array[j] = random.Next();

                //Finding the largest random number sequentially and recording the time.
                stopWatch.Start();
                int result1 = LargestNumberFinderSequential(array);
                stopWatch.Stop();
                Console.WriteLine($"Sequential Algorithm Time: {stopWatch.ElapsedMilliseconds}ms");
                stopWatch.Reset();

                //Finding the largest random number in parallel and recording the time.
                stopWatch.Start();
                int result2 = LargestNumberFinderParallel(array, chunkSize);
                stopWatch.Stop();
                Console.WriteLine($"Parallel Algorithm Time: {stopWatch.ElapsedMilliseconds}ms");
                stopWatch.Restart();

                Console.WriteLine($"Equal Check: {CompareCheck(result1, result2)}\n");
            }
        }
    }
}