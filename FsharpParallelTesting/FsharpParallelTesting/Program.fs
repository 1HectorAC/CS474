﻿open System
open System.Threading.Tasks

//Setup some variables.
let size = 100000000
let CC = Environment.ProcessorCount
let chunk = 50

//This function to get the largest number index of two numbers in array.
let GetLargestBetweenTwoInArray (one,two: int, f: array<_>) = if f.[one] > f.[two] then one else two

//This function will check if the two numbers are equal
let EqualCheck (one,two:int) = if one = two then "Success" else "fail"

//Parallel Part.
let ParallelLargestAlgorithm (f:array<_>) = 
    let mutable maxIndex =  0;
    let monitor = new Object()
    Parallel.For(0, size / chunk, fun i ->
        let start1 = i * chunk
        let end1 = (i + 1) * chunk
        let mutable localMax = start1
        for i in start1.. (end1 - 1) do (localMax <- GetLargestBetweenTwoInArray(i,localMax,f)) |> ignore
        lock (monitor) (fun () -> maxIndex <- GetLargestBetweenTwoInArray(localMax, maxIndex,f)) |> ignore
         ) |> ignore
    f.[maxIndex]

//Sequential Part.
let SequentialLargestAlgorithm (f:array<_>) =
    let mutable maxIndex = 0
    for i in 1.. size-1 do (maxIndex <- GetLargestBetweenTwoInArray(maxIndex, i,f))
    f.[maxIndex]

let main() =
    let timer = new System.Diagnostics.Stopwatch()
    let rnd = System.Random()

    //Setup array with random numbers.
    let arrayTest = Array.zeroCreate size
    for i in 0.. (size-1) do arrayTest.[i] <- rnd.Next()

    //Print out the values in array.
    //Array.ForEach(arrayTest, fun i -> Console.WriteLine(i))

    timer.Start()
    let parallelLargest = ParallelLargestAlgorithm(arrayTest)
    timer.Stop()
    printfn "Parallel Algorithm\nMax Value:%d \nTime:%sms" parallelLargest (timer.ElapsedMilliseconds.ToString())

    timer.Reset()
    timer.Start()
    let sequentialLargest = SequentialLargestAlgorithm(arrayTest) 
    timer.Stop()
    printfn "\nSequential Algorithm\nMax Value:%d \nTime:%sms" sequentialLargest (timer.ElapsedMilliseconds.ToString())

    printfn"\nEqual Check: %s\n" (EqualCheck(parallelLargest,sequentialLargest))
    
main()
