import multiprocessing  
from multiprocessing import Process
import random
import time

#Make a list with random numbers with a specfied length and max size of values.
def MakesRandomList(length, max):
	testList = []
	for x in range(0,length):
		testList.append(random.randrange(0, max))
	return testList
		
#Checks if inputed value is a prime.
def Is_prime(n):
	if(n < 2):
		return False
	for i in range(2,n):
		if (n%i) == 0:
			return False
	return True
	
################################### Sequential Version #################################################

#return smallest prime in list.
def FindSmallestPrime(list):
	lowestPrime = 0
	for x in list:
		if(Is_prime(int(x))):
			if(x < lowestPrime or lowestPrime==0):
				lowestPrime = x
	return lowestPrime
	
#return largest prime in list.
def FindLargestPrime(list):
	largestPrime = 0
	for x in list:
		if(Is_prime(int(x))):
			if(x > largestPrime or largestPrime==0):
				largestPrime = x
	return largestPrime	

#get the range of primes in list (sequential).
def SequentialRun(list):
	#Count the number of primes in each list.
	listLargestPrime = FindLargestPrime(list)
	listSmallestPrime = FindSmallestPrime(list)
	if(listLargestPrime == 0):
		print("No primes found")
	else:
		return listLargestPrime - listSmallestPrime
		
##################################### Parallel Version ################################################

#add smallest prime in list to queue.
def FindSmallestPrimeWithQueue(queue,list):
	lowestPrime = 0
	for x in list:
		if(Is_prime(int(x))):
			if(x < lowestPrime or lowestPrime==0):
				lowestPrime = x
	test = lowestPrime
	queue.put(test)
	
#add largest prime in list to queue.
def FindLargestPrimeWithQueue(queue,list):
	largestPrime = 0
	for x in list:
		if(Is_prime(int(x))):
			if(x > largestPrime or largestPrime==0):
				largestPrime = x
	test = largestPrime
	queue.put(test)
	
#get the range of primes in list (parallel).
def ParallelRun(list):
	queue = multiprocessing.Queue()
	
	#Setup two processes that will run in parallel. They will get the largest and smallest prime in list.
	p = multiprocessing.Process(target=FindLargestPrimeWithQueue,args=(queue,list,))
	p2 = multiprocessing.Process(target=FindSmallestPrimeWithQueue,args=(queue,list,))
	p.start()
	p2.start()
	p.join()
	p2.join()
	
	entry = queue.get()
	entry2 = queue.get()
	if(entry == 0):
		print("No primes found")
	else:
		listPrimeRange = 0
		#Make check to subtract largests number first.
		if(entry > entry2):
			return entry - entry2
		else:
			return entry2 - entry
		
###################################### Main ################################################

if __name__ == '__main__':
	print("Running PrimeRange.py")
	#Setup some variables.
	sizeOfList = 10000
	maxValue = 100000
	print("Size of List:" + str(sizeOfList))
	print("Max value in list:" + str(maxValue))
	
	#Generate list with random numbers.
	list = MakesRandomList(sizeOfList,maxValue)
	
	#Sequential run.
	print("\nSequential")
	t1 = time.time()
	result1 = SequentialRun(list)
	print("Time: " + str(time.time() - t1))
	print("Range of primes: "+ str(result1))

	
	#Parallel run.
	print("\nParallel")
	t2 = time.time()
	result2 = ParallelRun(list)
	print("Time: " + str(time.time() - t2))
	print("Range of primes: "+ str(result2))