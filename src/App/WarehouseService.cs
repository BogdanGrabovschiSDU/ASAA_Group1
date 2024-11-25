using Npgsql;
using System;

public class WarehouseService
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

    public void SaveData(string partName, int bikeTypeId, int stock)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Step 1: Check if the BikeType exists in the Bike_Types table.
            using var checkBikeTypeCmd = new NpgsqlCommand("SELECT \"BikeTypeID\" FROM \"Bike_Types\" WHERE \"BikeTypeID\" = @bikeTypeId", connection);
            checkBikeTypeCmd.Parameters.AddWithValue("bikeTypeId", bikeTypeId);
            var bikeTypeExists = checkBikeTypeCmd.ExecuteScalar() != null;

            if (!bikeTypeExists)
            {
                Console.WriteLine("Error: BikeTypeID does not exist.");
                return;
            }

            // Step 2: Insert the part into the Bike_Parts table.
            using var cmdInsertPart = new NpgsqlCommand("INSERT INTO \"Bike_Parts\" (\"PartName\", \"BikeTypeID\", \"Stock\") VALUES (@partName, @bikeTypeId, @stock)", connection);
            cmdInsertPart.Parameters.AddWithValue("partName", partName);
            cmdInsertPart.Parameters.AddWithValue("bikeTypeId", bikeTypeId);
            cmdInsertPart.Parameters.AddWithValue("stock", stock);
            cmdInsertPart.ExecuteNonQuery();

            // Step 3: Insert the stock data into the Bike_Parts_Stock table.
            using var cmdInsertStock = new NpgsqlCommand("INSERT INTO \"Bike_Parts_Stock\" (\"BikeTypeID\", \"PartID\", \"Stock\") VALUES (@bikeTypeId, (SELECT \"PartID\" FROM \"Bike_Parts\" WHERE \"PartName\" = @partName AND \"BikeTypeID\" = @bikeTypeId), @stock)", connection);
            cmdInsertStock.Parameters.AddWithValue("bikeTypeId", bikeTypeId);
            cmdInsertStock.Parameters.AddWithValue("partName", partName);
            cmdInsertStock.Parameters.AddWithValue("stock", stock);
            cmdInsertStock.ExecuteNonQuery();

            Console.WriteLine($"Data saved to Warehouse: Part={partName}, BikeTypeID={bikeTypeId}, Stock={stock}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
