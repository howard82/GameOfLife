namespace GameOfLife
{
    public interface IGameOfLife
    {
        int Height { get; }
        int Width { get; }
        Cell[][] Cells { get; }
        void InsertTemplate(ITemplate template, int templateX, int templateY);
        void TakeTurn();
    }
}
