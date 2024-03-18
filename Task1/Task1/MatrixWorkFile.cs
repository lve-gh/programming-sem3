using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    /// <summary>
    /// Class of reading and writting matrixes.
    /// </summary>
    public class MatrixWorkFileClass
    {
        /// <summary>
        /// Read the matrix.
        /// </summary>
        public static int[,] MatrixRead(string path)
        {
            if (!File.Exists(path))
                throw new Exception("Матрицы нельзя перемножить");
            StreamReader sr = new StreamReader(path);
            var data = sr.ReadToEnd();

            string[] dataSplit = data.Split();

            int rows = int.Parse(dataSplit[0]);
            int columns = int.Parse(dataSplit[1]);
            int readingIndex = 2;
            int[,] matrix = new int[rows, columns];

            for (int i = 0; i < rows;)
            {
                for (int j = 0; j < columns;)
                {
                    if (dataSplit[readingIndex] != "\n" && dataSplit[readingIndex] != " " && dataSplit[readingIndex] != "\r" && dataSplit[readingIndex] != "")
                    {
                        matrix[i, j] = int.Parse(dataSplit[readingIndex]);
                        j++;
                    }
                    readingIndex++;
                }
                i++;
            }

            sr.Close();
            return matrix;
        }
        /// <summary>
        /// Write the matrix.
        /// </summary>
        public static void MatrixWrite(int[,] matrix, string path)
        {
            StreamWriter sw = new StreamWriter(path, false);
            sw.WriteLine(matrix.GetLength(0) + " " + matrix.GetLength(0));
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    sw.Write(matrix[i, j] + " ");
                }
                sw.WriteLine();
            }

            sw.Close();
        }
    }
}
