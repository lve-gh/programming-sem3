using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    /// <summary>
    /// Class with functions of matrix multiplication.
    /// </summary>
    public class MatrixMultiplicationClass
    {
        /// <summary>
        /// Multiplicate the matrix.
        /// </summary>
        public static int[,] Multiplication(int[,] a, int[,] b)
        {
            var timeStart = DateTime.Now;
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Length / a.GetLength(0) != b.GetLength(0))
            {
                throw new Exception("Матрицы нельзя перемножить");
            }
            int[,] newMatrix = new int[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                //counterTemp1++;
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        newMatrix[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            var timeTotal = DateTime.Now - timeStart;
            Console.WriteLine("Без многопоточки:");
            Console.WriteLine(timeTotal);
            Console.WriteLine(newMatrix[0, 0]);
            return newMatrix;
        }

        /// <summary>
        /// Multiplicate the matrix concurently.
        /// </summary>
        public static int[,] MultiplicationConcurent(int[,] a, int[,] b)
        {
            var timeStart = DateTime.Now;
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (a.Length / a.GetLength(0) != b.GetLength(0))
            {
                throw new Exception("Матрицы нельзя перемножить");
            }
            int[,] newMatrix = new int[a.GetLength(0), b.GetLength(1)];
            var threads = new Thread[Environment.ProcessorCount];
            var chunkSize = (b.GetLength(1)) / threads.Length + 1;
            for (var m = 0; m < threads.Length; m++)
            {
                var locall = m;
                threads[m] = new Thread(() =>
                {
                    for (int i = 0; i < a.GetLength(0); i++)
                    {
                        for (var j = locall * chunkSize; j < (locall + 1) * chunkSize && j < b.GetLength(1); j++)
                        {
                            for (int k = 0; k < b.GetLength(0); k++)
                            {
                                newMatrix[i, j] += a[i, k] * b[k, j];

                            }
                        }
                    }
                });
            }
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();

            var timeTotal = DateTime.Now - timeStart;
            Console.WriteLine("C многопоточкой:");
            Console.WriteLine(timeTotal);
            Console.WriteLine(newMatrix[0, 0]);
            return newMatrix;
        }
    }
}
