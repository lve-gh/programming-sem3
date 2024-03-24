namespace Task1
{
    /// <summary>
    /// Class of reading and writting matrixes.
    /// </summary>
    public class MatrixReadingWritingClass
    {
        /// <summary>
        /// Read the matrix.
        /// </summary>
        public static int[,] MatrixRead(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Unable to read matrix");
            using StreamReader matrixReader = new StreamReader(path);
            var data = matrixReader.ReadToEnd();

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

            matrixReader.Close();
            return matrix;
        }
        /// <summary>
        /// Write the matrix.
        /// </summary>
        public static void MatrixWrite(int[,] matrix, string path)
        {
            using StreamWriter matrixWriter = new StreamWriter(path, false);
            matrixWriter.WriteLine(matrix.GetLength(0) + " " + matrix.GetLength(0));
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    matrixWriter.Write(matrix[i, j] + " ");
                }
                matrixWriter.WriteLine();
            }

            matrixWriter.Close();
        }
    }
}
