namespace GameOfLife
{
    public interface ITemplate
    {
        string Name { get; }
        int Height { get; }
        int Width { get; }
        Cell[][] Cells { get; }
    }
}
