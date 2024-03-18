namespace MatrixWorkFile.tests;

using Task1;
using System.Diagnostics;
using System.IO;
using System.Numerics;

public class Tests
{
    [Test]
    public void TestReadingWriting()
    {
        string path = "11111";
        MatrixWorkFileClass.MatrixRead(path);
    }

    [Test]
    public void WriteCon()
    {
        int[,] exceptedMatrix = { { 1, 5 },
                                  { 4, 6 }};
        MatrixWorkFileClass.MatrixWrite(exceptedMatrix, "matrix_test.txt");
        int[,] actualMatrix = MatrixWorkFileClass.MatrixRead("matrix_test.txt");
        Debug.Assert(exceptedMatrix[0, 0] == actualMatrix[0, 0]);
        Debug.Assert(exceptedMatrix[0, 1] == actualMatrix[0, 1]);
        Debug.Assert(exceptedMatrix[1, 0] == actualMatrix[1, 0]);
        Debug.Assert(exceptedMatrix[1, 1] == actualMatrix[1, 1]);
    }
}