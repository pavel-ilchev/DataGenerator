namespace DataGenerator.Models;

public class TableDto
{
    public string TableName { get; set; }
    public List<ColumnDto> Columns { get; set; }
    public int Order { get; set; }

    public DependenciesDto Dependencies { get; set; }
}