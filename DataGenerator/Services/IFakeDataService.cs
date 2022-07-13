namespace DataGenerator.Services;

public interface IFakeDataService
{
    object GetFakeData(string columnName, string type, string maxLenght, int locationId);
}