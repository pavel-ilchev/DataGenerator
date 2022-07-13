namespace DataGenerator.Services;

public class FakeDataService : IFakeDataService
{
    private static readonly Random rnd = new();

    public object GetFakeData(string columnName, string type, string maxLenght, int locationId)
    {
        object result;
        switch (type)
        {
            case "int":
            case "smallint":
            case "tinyint":
                result = columnName.Contains("location", StringComparison.InvariantCultureIgnoreCase)
                    ? locationId
                    : rnd.Next(255);
                break;
            case "varchar":
            case "nvarchar":
                result = columnName.Contains("custid", StringComparison.InvariantCultureIgnoreCase)
                    ? Guid.NewGuid()
                    : this.GetStringData(columnName, maxLenght);
                break;
            case "bit":
                result = false;
                break;
            case "date":
            case "smalldatetime":
                result = DateTime.Now;
                break;
            case "decimal":
                // result = (float)Math.Round(rnd.NextDouble() * 5000, 2);
                result = rnd.Next(255);
                break;
            default:
                result = null;
                break;
        }

        return result;
    }

    private string GetStringData(string columnName, string maxLenght)
    {
        return "asd";
    }
}