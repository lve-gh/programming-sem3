using Task1;

string path = "";

path = args[0];

path ??= "";

int[,] matrix1 = MatrixWorkFileClass.MatrixRead(path);

path = args[1];

path ??= "";

int[,] matrix2 = MatrixWorkFileClass.MatrixRead(path);

int[,] matrix3 = MatrixMultiplicationClass.Multiplication(matrix1, matrix2);

matrix3 = MatrixMultiplicationClass.MultiplicationConcurent(matrix1, matrix2);


if (matrix3 != null)
{
    for (int i = 0; i < matrix3.GetLength(0); i++)
    {
        for (int j = 0; j < matrix3.Length / matrix3.GetLength(0); j++)
        {
            Console.WriteLine(matrix3[i, j]);
        }
    }
    Console.WriteLine("Введите путь сохранения матрицы:");
    path = args[2];
    path ??= "";
    MatrixWorkFileClass.MatrixWrite(matrix3, path);
}
 else
    Console.WriteLine("Ошибка выполнения умножения");
