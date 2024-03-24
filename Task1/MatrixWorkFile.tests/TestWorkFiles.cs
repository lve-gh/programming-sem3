namespace MatrixWorkFile.Tests;

using Task1;
public class Tests
{
    [Test]
    public void Write_Matrix()
    {
        int[,] exceptedMatrix = { { 1, 5 },
                                  { 4, 6 }};
        MatrixReadingWritingClass.MatrixWrite(exceptedMatrix, "matrix_test.txt");
        int[,] actualMatrix = MatrixReadingWritingClass.MatrixRead("matrix_test.txt");
        Assert.That(exceptedMatrix[0, 0] == actualMatrix[0, 0]);
        Assert.That(exceptedMatrix[0, 1] == actualMatrix[0, 1]);
        Assert.That(exceptedMatrix[1, 0] == actualMatrix[1, 0]);
        Assert.That(exceptedMatrix[1, 1] == actualMatrix[1, 1]);
    }
}