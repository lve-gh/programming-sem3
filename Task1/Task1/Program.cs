using Task1;

string pathOfFirstMatrix = args[0];
pathOfFirstMatrix ??= "";
var matrix1 = MatrixReadingWritingClass.MatrixRead(pathOfFirstMatrix);
string pathOfSecondMatrix = args[1];
pathOfSecondMatrix ??= "";
var matrix2 = MatrixReadingWritingClass.MatrixRead(pathOfSecondMatrix);
var matrix3 = MatrixMultiplicationClass.Multiplication(matrix1, matrix2);
matrix3 = MatrixMultiplicationClass.MultiplicationConcurent(matrix1, matrix2);

if (matrix3 != null)
{
    string pathOfResultMatrix = args[2];
    pathOfResultMatrix ??= "";
    MatrixReadingWritingClass.MatrixWrite(matrix3, pathOfResultMatrix);
}
 else
    throw new ArgumentException("Unable to multiplicate matrixes");
