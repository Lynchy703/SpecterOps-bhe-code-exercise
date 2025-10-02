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
        if (n < 1) throw new ArgumentOutOfRangeException(nameof(n));

        // Approximate upper bound using prime number theorem
        double nn = n;
        long limit = (long)(nn * (Math.Log(nn) + Math.Log(Math.Log(nn))));
        if (n < 6) limit = 15;

        // Small primes up to sqrt(limit)
        int sqrtLimit = (int)Math.Sqrt(limit) + 1;


        //Get the lower limit ones that don't need to be segmented.
        var basePrimes = SimpleSieve(sqrtLimit);

        long count = 0;
        int segmentSize = 1000000;

        // Code Loops through and segments the sieve instead of one loop. This allows higher numbers since we dont run out of
        // memory in the long
        for (long low = 2; low <= limit; low += segmentSize)
        {
            long high = Math.Min(low + segmentSize - 1, limit);
            bool[] isPrime = new bool[high - low + 1];
            Array.Fill(isPrime, true);


            //Fill Array with basePrimes precalculated with SimpleSieve
            foreach (int p in basePrimes)
            {
                long start = Math.Max((long)p * p, ((low + p - 1) / p) * p);
                for (long j = start; j <= high; j += p)
                    isPrime[j - low] = false;
            }

            for (long i = low; i <= high; i++)
            {
                if (isPrime[i - low])
                {
                    count++;
                    if (count == n) return i;
                }
            }
        }

        throw new Exception("Upper bound too small.");
    }

    private static List<int> SimpleSieve(int limit)
    {
        bool[] isPrime = new bool[limit + 1];
        Array.Fill(isPrime, true);
        isPrime[0] = isPrime[1] = false;
        //Simple Sieve of Eratosthenes
        for (int p = 2; p * p <= limit; p++)
        {
            if (isPrime[p])
            {
                for (int j = p * p; j <= limit; j += p)
                    isPrime[j] = false;
            }
        }

        var primes = new List<int>();
        for (int i = 2; i <= limit; i++)
            if (isPrime[i]) primes.Add(i);

        return primes;
    }

}