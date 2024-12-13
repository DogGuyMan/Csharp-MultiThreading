using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Udemy.MultiThreading.Lecture3 {
    /***
    Executing sequential loop...
    Sequential loop time in milliseconds: 503
    Computation complete. Print results (y/n)? n
    Executing parallel loop...
    Parallel loop time in milliseconds: 101
    Computation complete. Print results (y/n)? n
    ***/
    public class ParallelExample_1
    {
        const int COL_COUNT1 = 180;
        const int COL_COUNT2 = 2000;
        const int ROW_COUNT1 = 270;

        public static void MajorAction() {
            double[,] m1 = InitializeMatrix(ROW_COUNT1, COL_COUNT1);
            double[,] m2 = InitializeMatrix(COL_COUNT1, COL_COUNT2);
        #region Sequential Loop 수행
            MultiplyMatricesSequential(m1, m2, out double[,] seqRes);
            OfferToPrint(ROW_COUNT1, COL_COUNT2, seqRes);
        #endregion

        #region Parallel Loop 수행
            MultiplyMatricesParallel(m1, m2, out double[,] parRes);
            OfferToPrint(ROW_COUNT1, COL_COUNT2, parRes);
        #endregion
        }

        #region 1 Sequential Loop

        public static void MultiplyMatricesSequential(double[,] matA, double[,] matB, out double[,] result)
        {
            result = new double[ROW_COUNT1, COL_COUNT2];
            
            Console.Error.WriteLine("Executing sequential loop...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int matACols = matA.GetLength(1);
            int matBCols = matB.GetLength(1);
            int matARows = matA.GetLength(0);

            for (int i = 0; i < matARows; i++)
            {
                for (int j = 0; j < matBCols; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < matACols; k++)
                    {
                        temp += matA[i, k] * matB[k, j];
                    }
                    result[i, j] += temp;
                }
            }

            stopwatch.Stop();
            Console.Error.WriteLine("Sequential loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);
        }

        #endregion

        #region 2 Parallel Loop
        public static void MultiplyMatricesParallel(double[,] matA, double[,] matB, out double[,] result)
        {
            var res = new double[ROW_COUNT1, COL_COUNT2];
            Console.Error.WriteLine("Executing parallel loop...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
   
            int matACols = matA.GetLength(1);
            int matBCols = matB.GetLength(1);
            int matARows = matA.GetLength(0);

            // A basic matrix multiplication.
            // Parallelize the outer loop to partition the source array by rows.
            Parallel.For(0, matARows, 
            i => {
                for (int j = 0; j < matBCols; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < matACols; k++)
                    {
                        temp += matA[i, k] * matB[k, j];
                    }
                    res[i, j] = temp;
                }
            }); // Parallel.For
            stopwatch.Stop();
            
            Console.Error.WriteLine("Parallel loop time in milliseconds: {0}",
            stopwatch.ElapsedMilliseconds);
            
            result = res;
        }
        #endregion

        #region Helper_Methods
        static double[,] InitializeMatrix(int rows, int cols)
        {
            double[,] matrix = new double[rows, cols];

            Random r = new Random();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = r.Next(100);
                }
            }
            return matrix;
        }

        private static void OfferToPrint(int rowCount, int colCount, in double[,] matrix)
        {
            Console.Error.Write("Computation complete. Print results (y/n)? ");
            char c = Console.ReadKey(true).KeyChar;
            Console.Error.WriteLine(c);
            if (Char.ToUpperInvariant(c) == 'Y')
            {
                if (!Console.IsOutputRedirected &&
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.WindowWidth = 180;
                }

                Console.WriteLine();
                for (int x = 0; x < rowCount; x++)
                {
                    Console.WriteLine("ROW {0}: ", x);
                    for (int y = 0; y < colCount; y++)
                    {
                        Console.Write("{0:#.##} ", matrix[x, y]);
                    }
                    Console.WriteLine();
                }
            }
        }
        #endregion

    }
}