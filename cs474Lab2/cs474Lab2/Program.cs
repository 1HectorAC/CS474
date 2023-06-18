using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace cs474Lab2
{
    class Program
    {

        //Declaring constant variable SIZE.
        const int SIZE = 200000000;

        /// <summary>
        /// This function will return the largest number of an inputted array using a sequential search.
        /// </summary>
        /// <param name="array">The array that will be searched.</param>
        static int LargestNumberFinderSequential(int[] array)
        {
            int largestNum = array[0];

            for (int i = 1; i < SIZE; i++)
            {
                if (array[i] > largestNum)
                    largestNum = array[i];
            }
            return largestNum;
        }

        /// <summary>
        /// This function will return the largest number of an inputted array. Search is done in parallel with parallel algorithm.
        /// </summary>
        /// <param name="array">The array that will be searched.</param>
        /// <param name="chunk">The size of the chunk each process will look through.</param>
        /// <returns></returns>
        
        static int LargestNumberFinderParallel(int[] array, int chunk)
        {
            int largestNum = array[0];
            Parallel.For(0, SIZE / chunk, i => {
                //Calculate location of chunk.
                int iSize = i * chunk;
                //Loop though a chunk of the array.
                for (int j = iSize; j < (iSize + chunk); j++)
                {
                    if (array[j] > largestNum)
                        largestNum = array[j];
                }
            });
            return largestNum;
        }

        /// <summary>
        /// This function will return the largest number of an inputted array. Search is done in parallel with parallel algorithm. No control of task size.
        /// </summary>
        /// <param name="array">The array that will be searched.</param>
        /// <returns></returns>
        static int LargestNumberFinderParallel2(int[] array)
        {
            int largestNum = array[0];

            Parallel.ForEach(array, i => {
                if (i > largestNum)
                    largestNum = i;
            });
            return largestNum;
        }


        static void Main(string[] args)
        {
            //Setting up variables.
            int chunkSize = SIZE / Environment.ProcessorCount;
            int[] array = new int[SIZE];

            Console.WriteLine("SIZE: " + SIZE);
            Console.WriteLine("Chunk Size: " + chunkSize);


            //Filling the array sequentially with random numbers.
            Console.WriteLine("Generating random numbers for the array...");
            Random random = new Random();
            for (int i = 0; i < SIZE; i++)
            {
                array[i] = random.Next();
            }

            //Finding the largest random number sequentially and recording the time.
            Console.WriteLine("\n\nRunning sequential largest number search...");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int largestNum = LargestNumberFinderSequential(array);
            stopWatch.Stop();
            Console.WriteLine($"Time: {stopWatch.Elapsed}\nLargest Number: {largestNum}\n");
            stopWatch.Reset();

            
            //Finding the largest random number in parallel and recording the time.
            Console.WriteLine("Running parallel largest number search 1 (For loop with chunks ver.)...");
            stopWatch.Start();
            int largestNum2 = LargestNumberFinderParallel(array, chunkSize);
            stopWatch.Stop();
            Console.WriteLine($"Time: {stopWatch.Elapsed}\nLargest Number: {largestNum2}\n");

            
            //Finding the largest random number in parallel and recording the time. Note that chunk size must be set in this version.
            Console.WriteLine("Running parallel largest number search 2 (Foreach ver.)...");
            stopWatch.Start();
            int largestNum3 = LargestNumberFinderParallel2(array);
            stopWatch.Stop();
            Console.WriteLine($"Time: {stopWatch.Elapsed}\nLargest Number: {largestNum3}\n");
            

        }
    }
}
