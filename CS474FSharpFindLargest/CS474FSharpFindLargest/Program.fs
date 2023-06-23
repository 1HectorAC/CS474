open System
open System.Threading.Tasks

let CC = Environment.ProcessorCount

//This function to get the largest number index of two numbers in array.
let GetLargestBetweenTwoInArray (one,two: int, f: array<_>) = if f[one] > f[two] then one else two

//This function will check if the two numbers are equal
let EqualCheck (one,two:int) = one = two

//Sequential Part.
let SequentialLargestAlgorithm (arr:array<_>) =
    let mutable maxIndex = 0
    for i in 1.. arr.Length-1 do (maxIndex <- GetLargestBetweenTwoInArray(maxIndex, i,arr))
    arr[maxIndex]

//Parallel Part.
let ParallelLargestAlgorithm (arr:array<_>, chunk: int) = 
    let mutable maxIndex =  0;
    let monitor = new Object()
    Parallel.For(0, arr.Length / chunk, fun i ->
        let start1 = i * chunk
        let end1 = (i + 1) * chunk
        let mutable localMax = start1
        for i in start1.. (end1 - 1) do (localMax <- GetLargestBetweenTwoInArray(i,localMax,arr)) |> ignore
        lock (monitor) (fun () -> maxIndex <- GetLargestBetweenTwoInArray(localMax, maxIndex,arr)) |> ignore
         ) |> ignore
    arr[maxIndex]

let main() =
    printfn "Find Largest Number Algorithm Comparison with F#"
    printfn "Processors: %d\n" CC
    let timer = new System.Diagnostics.Stopwatch()
    let rnd = System.Random()
    let sizes = [|1000;1000000;2000000;20000000|]
    
    for size in sizes do
        let chunk = size / CC;
        printfn "Size: %d\nChunk Size: %d" size chunk

        //Setup array with random numbers.
        let arrayTest = Array.zeroCreate size
        for i in 0.. (size-1) do arrayTest[i] <- rnd.Next()

        timer.Start()
        let sequentialLargest = SequentialLargestAlgorithm(arrayTest) 
        timer.Stop()
        printfn "Sequential Algorithm Time: %sms" (timer.ElapsedMilliseconds.ToString())
        timer.Reset()

        timer.Start()
        let parallelLargest = ParallelLargestAlgorithm(arrayTest, chunk)
        timer.Stop()
        printfn "Parallel Algorithm Time: %sms" (timer.ElapsedMilliseconds.ToString())
        timer.Reset()
        printfn"Equal Check: %b\n" (EqualCheck(parallelLargest,sequentialLargest))
    
main()