namespace DataGenerator.Models;

public class ColumnDto
{
    public int OrdinalPosition { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string? StringMaxLenght { get; set; }
    public int NumericPrecision { get; set; }
    public int NumericScale { get; set; }
}