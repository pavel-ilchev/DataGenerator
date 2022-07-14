namespace DataGenerator.Services;

using Faker;
using Interfaces;
using Models.Enums;

public class FakeDataService : IFakeDataService
{
    public static readonly Random rnd = new();

    private readonly Dictionary<string, List<string>> ModelsByMake = new()
    {
        { "BMW", new List<string> { "X3", "X5", "740i" } },
        { "Audi", new List<string> { "A4", "A5", "A8" } },
        { "Nissan", new List<string> { "Ariya", "Altima", "Armada" } },
        { "Toyota", new List<string> { "Avalon", "Avensis", "Prius" } },
        { "GMC", new List<string> { "Canyon Extended Cab", "Canyon Crew Cab" } },
        { "Chevrolet", new List<string> { "Trailblazer", "Equinox", "Traverse", "Camaro" } },
        { "Dodge", new List<string> { "Charger", "Challenger", "Durango" } },
        { "Ford", new List<string> { "Bronco Sport", "Mustang", "Escape" } },
        { "Buick", new List<string> { "Enclave", "Encore", "Envision" } }
    };

    private string lastMake;

    public object GetFakeData(string columnName, string type, string maxLength, int locationId, int index)
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
                result = this.GetStringData(columnName, maxLength, index);
                break;
            case "bit":
                result = rnd.Next(2) > 0;
                break;
            case "date":
            case "smalldatetime":
                result = this.GetDate(columnName, index);
                break;
            case "decimal":
                result = rnd.Next(255);
                break;
            default:
                result = null;
                break;
        }

        return result;
    }


    public static string RandomNumber(int length)
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    private string GetStringData(string columnName, string maxLength, int index)
    {
        var type = this.DefineColumnType(columnName);
        string result;
        switch (type)
        {
            case ColumnType.Street:
                result = Address.StreetName();
                break;
            case ColumnType.City:
                result = Address.USCity();
                break;
            case ColumnType.State:
                result = Address.StateAbbreviation();
                break;
            case ColumnType.Zip:
                result = Address.USZipCode();
                break;
            case ColumnType.Email:
                result = User.Email();
                break;
            case ColumnType.Identifier:
                result = Guid.NewGuid().ToString();
                break;
            case ColumnType.FirstName:
                result = Name.FirstName();
                break;
            case ColumnType.LastName:
                result = Name.LastName();
                break;
            case ColumnType.Phone:
                result = $"555{RandomNumber(7)}";
                break;
            case ColumnType.Vehicle:
                result = this.GetVehicleData(columnName);
                break;
            case ColumnType.Latitude:
                result = GeoLocation.Latitude().ToString();
                break;
            case ColumnType.Longitude:
                result = GeoLocation.Longitude().ToString();
                break;
            case ColumnType.Unknown:
            default:
                int charCount = rnd.Next(10, 50);
                int.TryParse(maxLength, out int length);
                charCount = charCount > length ? length : charCount;
                string? lorem = Lorem.Sentence(20);
                result = lorem.Substring(0, charCount);
                break;
        }

        return result;
    }

    private DateTime? GetDate(string columnName, int index)
    {
        DateTime? date = DateTime.Now;
        if (columnName.Contains("update", StringComparison.InvariantCultureIgnoreCase) ||
            columnName.Contains("create", StringComparison.InvariantCultureIgnoreCase))
        {
            date = DateTime.Now;
        }
        else if (columnName.Contains("delete", StringComparison.InvariantCultureIgnoreCase))
        {
            bool isDeleted = rnd.Next(11) > 9;
            date = isDeleted ? DateTime.Now : null;
        }
        else if (columnName.Contains("birth", StringComparison.InvariantCultureIgnoreCase))
        {
            int monthsBack = rnd.Next(252, 1200);
            date = DateTime.Now.AddMonths(-monthsBack);
        }
        else if (columnName.Contains("first", StringComparison.InvariantCultureIgnoreCase))
        {
            int monthsBack = rnd.Next(12);
            date = DateTime.Now.AddMonths(-monthsBack);
        }
        else if (columnName.Contains("open", StringComparison.InvariantCultureIgnoreCase))
        {
            int daysBack = rnd.Next(12);
            date = DateTime.Now.AddDays(-daysBack);
        }

        // for the every next row for the same customer subtract 3 months
        return date?.AddMonths(-index * 3) ?? date;
    }

    private ColumnType DefineColumnType(string columnName)
    {
        var type = ColumnType.Unknown;
        if (columnName.EndsWith("id", StringComparison.InvariantCultureIgnoreCase) ||
            columnName.EndsWith("ordernumber", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Identifier;
        }
        else if (columnName.Contains("make", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("model", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("year", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("license", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("vin", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Vehicle;
        }
        else if (columnName.Contains("lname", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("lastname", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.LastName;
        }
        else if (columnName.Contains("fname", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("firstname", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("name", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.FirstName;
        }
        else if (columnName.Contains("email", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Email;
        }
        else if (columnName.Contains("phone", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Phone;
        }
        else if (columnName.Contains("street", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Street;
        }
        else if (columnName.Contains("city", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.City;
        }
        else if (columnName.Contains("state", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.State;
        }
        else if (columnName.Contains("zip", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Zip;
        }
        else if (columnName.Contains("latitude", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("lat", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Latitude;
        }
        else if (columnName.Contains("longitude", StringComparison.InvariantCultureIgnoreCase) ||
                 columnName.Contains("lng", StringComparison.InvariantCultureIgnoreCase))
        {
            type = ColumnType.Longitude;
        }

        return type;
    }

    private string GetVehicleData(string columnName)
    {
        string data = string.Empty;
        if (columnName.Contains("make", StringComparison.InvariantCultureIgnoreCase))
        {
            int index = rnd.Next(this.ModelsByMake.Count);
            this.lastMake = this.ModelsByMake.Keys.ToList()[index];
            data = this.lastMake;
        }
        else if (columnName.Contains("model", StringComparison.InvariantCultureIgnoreCase))
        {
            var models = this.ModelsByMake[this.lastMake];
            data = models[rnd.Next(this.ModelsByMake[this.lastMake].Count)];
        }
        else if (columnName.Contains("year", StringComparison.InvariantCultureIgnoreCase))
        {
            data = rnd.Next(1950, DateTime.Now.Year + 1).ToString();
        }
        else if (columnName.Contains("license", StringComparison.InvariantCultureIgnoreCase))
        {
            data = RandomString(7);
        }
        else if (columnName.Contains("vin", StringComparison.InvariantCultureIgnoreCase))
        {
            data = RandomString(17);
        }

        return data;
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[rnd.Next(s.Length)]).ToArray());
    }
}