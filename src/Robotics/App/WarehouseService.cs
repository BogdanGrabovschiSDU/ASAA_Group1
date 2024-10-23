using Npgsql;
using System;

public class WarehouseService
{
    private readonly string _connectionString = "Host=localhost;Username=postgres;Password=yourpassword;Database=warehouse";

    public void SaveData(string key, string value)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand("INSERT INTO data (key, value) VALUES (@k, @v)", connection);
        cmd.Parameters.AddWithValue("k", key);
        cmd.Parameters.AddWithValue("v", value);
        cmd.ExecuteNonQuery();

        Console.WriteLine($"Data saved to Warehouse: Key={key}, Value={value}");
    }
}
    