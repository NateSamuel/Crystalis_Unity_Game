
//Design by Barbara Reichart lecture series, 2024
public interface ILevel
{
    int Length { get; }
    int Width { get; }

    bool IsBlocked(int x, int y);
    int Floor(int x, int y);

}
