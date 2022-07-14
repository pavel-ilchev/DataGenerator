namespace DataGenerator.Services.Interfaces;

public interface IFakeDataService
{
    object GetFakeData(string columnName, string type, string maxLength, int locationId, int index);
}