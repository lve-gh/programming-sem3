using hw1;
namespace MatrixWorkFile.tests;

public class Tests
{
    [Test]
    public void ReadNotExisted()
    {
        string path = "11111";
        MatrixWorkFileClass.MatrixRead(path);
    }
    [Test]
    public void WriteCon()
    {
        int[,] a = { { 1 } };
        MatrixWorkFileClass.MatrixWrite(a, "con");
    }
}