import multiprocessing  
from multiprocessing import Process
import random
from math import sqrt
from itertools import islice,count
import time

#Make a list with random numbers with a specfied length and max size of values.
def MakesRandomList(length, max):
	testList = []
	for x in range(0,length):
		testList.append(random.randrange(0, max))
	return testList
	
#prints values in list.
def PrintList(list):
	for x in list:
		print(x)
		
#Checks if inputed value is a prime.
def Is_prime(a):
	if a < 2:
		return False
	for x in islice(count(2),int(sqrt(a)-1)):
		if a % x == 0:
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
		listPrimeRange = listLargestPrime - listSmallestPrime
		#Output the results.
		print("Largest prime: "+ str(listLargestPrime))
		print("Smallest prime: "+ str(listSmallestPrime))
		print("Range of primes: "+ str(listPrimeRange))
		
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
	
	#Output the results.
	entry = queue.get()
	entry2 = queue.get()
	if(entry == 0):
		print("No primes found")
	else:
		listPrimeRange = 0
		#Make check to subtract largests number first.
		#Output the Results
		if(entry > entry2):
			listPrimeRange = entry - entry2
			print("Largest prime: "+ str(entry))
			print("Smallest prime: "+ str(entry2))
			print("Range of primes: "+ str(listPrimeRange))
		else:
			listPrimeRange = entry2 - entry
			print("Largest prime: "+ str(entry2))
			print("Smallest prime: "+ str(entry))
			print("Range of primes: "+ str(listPrimeRange))
		
###################################### Main ################################################

if __name__ == '__main__':
	print("Running PrimeRange.py")
	#Setup some variables.
	sizeOfList = 1000000
	maxValue = 200000
	print("Size of List:" + str(sizeOfList))
	print("Max value in list:" + str(maxValue))
	
	#Generate list with random numbers.
	list = MakesRandomList(sizeOfList,maxValue)
	
	#Sequential run.
	print("\nSequential")
	t1 = time.time()
	SequentialRun(list)
	print("Time: " + str(time.time() - t1))
	
	#Parallel run.
	print("\nParallel")
	t2 = time.time()
	ParallelRun(list)
	print("Time: " + str(time.time() - t2))