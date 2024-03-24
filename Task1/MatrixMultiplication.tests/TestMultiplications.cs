namespace MatrixMultiplication.Tests;

using Task1;

public class Tests
{
    [Test]
    public void Multiplication_Matrixes()
    {

        int[,] a = { { 1, 5 },
                     {4, 6 } };

        int[,] b = { { 6, 5 },
                     { 3, 9 } };

        int[,] actualMatrix = MatrixMultiplicationClass.Multiplication(a, b);

        int[,] exceptedMatrix = { { 21, 50 }, { 42, 74 } };

        Assert.That(exceptedMatrix[0, 0] == actualMatrix[0, 0]);
        Assert.That(exceptedMatrix[0, 1] == actualMatrix[0, 1]);
        Assert.That(exceptedMatrix[1, 0] == actualMatrix[1, 0]);
        Assert.That(exceptedMatrix[1, 1] == actualMatrix[1, 1]);
    }

    [Test]
    public void Is_Similar()
    {
        int[,] a = { { 5, 1 },
                     { 7, 0 } };

        int[,] b = { { 1, 9 },
                     { 6, 4 } };

        int[,] nonConcurent = MatrixMultiplicationClass.Multiplication(a, b);
        int[,] concurent = MatrixMultiplicationClass.MultiplicationConcurent(a, b);
        Assert.That(nonConcurent[0, 0] == concurent[0, 0]);
        Assert.That(nonConcurent[0, 1] == concurent[0, 1]);
        Assert.That(nonConcurent[1, 0] == concurent[1, 0]);
        Assert.That(nonConcurent[1, 1] == concurent[1, 1]);
    }
}