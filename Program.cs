// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Xml;
using System;
class Programm
{
    static string connectionString ="Server= (localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True";
    static void Main()
    {
        CreateDatabaseAndTable();
        InsertDataIntoTable();
        // Получаем список всех автомобилей и выводим на экран
        List<Car> allCars = GetAllCars();
       Console.WriteLine("Все автомобили:");
         DisplayCars(allCars);

        // Создаем вторую коллекцию с автомобилями младше 2018 года и выводим на экра
        List<Car> carsYoungerThan2018 = GetCarsYoungerThan2018(allCars);
        Console.WriteLine($"\nАвтомобили младше 2018 года:");
        DisplayCars(carsYoungerThan2018);
    }
    static List<Car> GetCarsYoungerThan2018(List<Car> cars)
    {
        int targetYear = 2018;
        return cars.Where(car => car.Year < targetYear).ToList();
    }

    static void CreateDatabaseAndTable()
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Создаем базу данных "Cars", если ее не существует
            string createDatabaseQuery = "IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'Cars') CREATE DATABASE Cars";
            ExecuteNonQuery(connection, createDatabaseQuery);

            // Используем базу данных "Cars"
            connection.ChangeDatabase("Cars");

            // Создаем таблицу "CarTable", если ее не существует
            string createTableQuery = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CarTable') " +
                                      "CREATE TABLE CarTable (Id INT PRIMARY KEY, Model NVARCHAR(50), Year INT)";
            ExecuteNonQuery(connection, createTableQuery);

            connection.Close();
        }
    }

    static void InsertDataIntoTable()
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;Initial Catalog=Cars";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Убедимся, что мы в базе данных "Cars"
            connection.ChangeDatabase("Cars");

            // Очищаем таблицу перед вставкой новых данных
            string clearTableQuery = "DELETE FROM CarTable";
            ExecuteNonQuery(connection, clearTableQuery);

            // Заполняем таблицу данными
            string insertDataQuery = "INSERT INTO CarTable (Id, Model, Year) VALUES " +
                                     "(1, 'Toyota Camry', 2022), " +
                                     "(2, 'Honda Accord', 2019), " +
                                     "(3, 'Ford Mustang', 2017), " +
                                     "(4, 'Chevrolet Malibu', 2018), " +
                                     "(5, 'Nissan Altima', 2016)";
            ExecuteNonQuery(connection, insertDataQuery);

            connection.Close();
        }
    }

    static List<Car> GetAllCars()
    {
        List<Car> cars = new List<Car>();
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;Initial Catalog=Cars";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Убеждаемся, что мы в базе данных "Cars"
            connection.ChangeDatabase("Cars");

            // Получаем данные из таблицы CarTable
            string selectQuery = "SELECT * FROM CarTable";
            using (SqlCommand command = new SqlCommand(selectQuery, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["Id"];
                        string model = (string)reader["Model"];
                        int year = (int)reader["Year"];

                        cars.Add(new Car { Id = id, Model = model, Year = year });
                    }
                }
            }

            connection.Close();
        }

        return cars;
    }
    

    static void DisplayCars(List<Car> cars)
    {
        foreach (Car car in cars)
        {
            Console.WriteLine($"Id: {car.Id}, Model: {car.Model}, Year: {car.Year}");
        }
    }

    static void ExecuteNonQuery(SqlConnection connection, string query)
    {
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            _ = command.ExecuteNonQuery();
        }
    }

    class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }
}