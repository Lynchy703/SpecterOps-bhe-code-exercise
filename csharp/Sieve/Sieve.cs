namespace Sieve;

public interface ISieve
{
    long NthPrime(long n);
}

public class SieveImplementation : ISieve
{

    //Segmented Version of Sieve. Same time complexity of original but handles larger numbers.
    //Simple version would require too much space for larger numbers. 
    // TimeComplexity: O(n log n) for simple vs segmented
    // Space complexity O(√(n log n) + segmentSize)   vs   O(n log n) for simple sieve
    public long NthPrime(long n)
    {
        n++;
        // check for a 0
        if (n < 1) throw new ArgumentOutOfRangeException(nameof(n));

        // Approximate upper bound using prime number theorem
        double nn = n;
        long limit = (long)(nn * (Math.Log(nn) + Math.Log(Math.Log(nn))));
        if (n < 6) limit = 15;

        // Small primes up to sqrt(limit)
        int sqrtLimit = (int)Math.Sqrt(limit) + 1;


        //Get the lower limit ones that don't need to be segmented.
        //These base primes will later be used to mark off multiples in each segment.
        var basePrimes = SimpleSieve(sqrtLimit);

        //count is how many primes we have found so far
        long count = 0;
        //segmentSize is 1 million numbers at a time -- cannot go over on memory at 1 million.
        int segmentSize = 1000000;

        // Code Loops through and segments the sieve instead of one loop. This allows higher numbers since we dont run out of
        // memory in the long
        for (long low = 2; low <= limit; low += segmentSize)
        {
            //Divides [2, limit] into smaller chunks [low, high].
            long high = Math.Min(low + segmentSize - 1, limit);
            //Creates a boolean array (isPrime) representing the current block.
            bool[] isPrime = new bool[high - low + 1];
            // fills entire array with true as default value
            Array.Fill(isPrime, true);


            //Mark non primes using the previous basePrimes values
            foreach (int p in basePrimes)
            {
                //For each small prime p (from basePrimes), find where to start marking in this segment:
                //p* p is the first composite of p.
                //((low + p - 1) / p) * p rounds up to the first multiple of p in this segment.
                long start = Math.Max((long)p * p, ((low + p - 1) / p) * p);
                //Mark non primes
                for (long j = start; j <= high; j += p)
                    isPrime[j - low] = false;
            }
            // l
            for (long i = low; i <= high; i++)
            {
                //checks if prime and if it is increments count
                if (isPrime[i - low])
                {
                    count++;

                    //if nth prime then return
                    if (count == n) return i;
                }
            }
        }
        //if not found but this shouldnt happen. Have a below zero check above as well.
        throw new Exception("Upper bound too small.");
    }

    //Returns primes up to a limit
    //Ex.
    //SimpleSieve(10);
    //Then:
    //The algorithm marks 4, 6, 8, 9, 10 as non-prime.
    //the result is [2, 3, 5, 7].
    private static List<int> SimpleSieve(int limit)
    {
        bool[] isPrime = new bool[limit + 1];
        Array.Fill(isPrime, true);
        isPrime[0] = isPrime[1] = false;
        //At this point values are as below since primes start at 2
        //Index:   0  1  2  3  4  5  6 ...
        //isPrime: F F  T T  T T  T...

        //Simple Sieve of Eratosthenes
        //Outer loop runs over potential prime numbers p from 2 up to √limit.
        for (int p = 2; p * p <= limit; p++)
        {
            //check if true in the array
            if (isPrime[p])
            {
                //Inner loop marks multiples of p as not prime.

                //Starts at p * p because smaller multiples have already been marked by smaller primes.
                //For example:
                //For p = 2, it marks 4, 6, 8, 10
                //For p = 3, it marks 9, 12, 15, 18
                for (int j = p * p; j <= limit; j += p)
                    isPrime[j] = false;

                //At this point all primes have true and every other number is false
            }
        }
        // this loop just collects the true values into a List
        var primes = new List<int>();
        for (int i = 2; i <= limit; i++)
            if (isPrime[i]) primes.Add(i);

        return primes;
    }

}