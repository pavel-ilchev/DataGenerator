namespace DataGenerator.Models;

public class DependenciesDto
{
    public string PkColName { get; set; }

    public Dictionary<string, string> FkTablesByColName { get; set; }
}