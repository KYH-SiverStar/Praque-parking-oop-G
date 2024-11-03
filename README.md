




using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;






// Klass Vehicle
public abstract class Vehicle
{
    public string RegistrationNumber { get; set; }
    public DateTime ArrivalTime { get; set; }
    public abstract int HourlyRate { get; }


    // Konstruktör för Vehicle
    protected Vehicle(string registrationNumber)
    {
        RegistrationNumber = registrationNumber;
        ArrivalTime = DateTime.Now; // Sätter ankomsttid till nuvarande tid
    }



    // Metod för parkeringsduration
    public TimeSpan GetParkingDuration()
    {
        return DateTime.Now - ArrivalTime; // Beräknar skillnaden mellan nuvarande tid och ankomsttid
    }
}



// Klass för Car
public class Car : Vehicle
{
    public override int HourlyRate => 20; // Timtaxa för bil

    public Car(string registrationNumber) : base(registrationNumber) { }
}



// Klass för Motorcycle (MC)
public class MC : Vehicle
{
    public override int HourlyRate => 10; // Timtaxa för motorcykel

    public MC(string registrationNumber) : base(registrationNumber) { }
}




      // ParkingSpot klass som kan hålla flera fordon baserat på storlek

public class ParkingSpot
{
    public List<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();         // Lista över fordon
    public int SpotSize { get; set; }        // Storlek på parkeringsplats

    public bool IsFull => Vehicles.Count >= SpotSize;          // Kontrollerar om platsen är full

           // Konstruktör för ParkingSpot
    public ParkingSpot(int spotSize)
    {
        SpotSize = spotSize;        //     Storlek på platsen
    }



          // Metod för att parkera fordon

    public void ParkVehicle(Vehicle vehicle)
    {
        if (!IsFull)
            Vehicles.Add(vehicle);       //    Att lägga till fordonet om platsen inte är full
    }



    //    Metod för att ta bort fordon

    public void RemoveVehicle(Vehicle vehicle)
    {
        Vehicles.Remove(vehicle); // Tar bort fordonet från platsen
    }
}



// Json converter för Vehicle

public class VehicleConverter : JsonConverter<Vehicle>
{

    //   Metod för att läsa fordon från JSON


    public override Vehicle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var registrationNumber = root.GetProperty("RegistrationNumber").GetString(); // Hämta registreringsnummer
            var type = root.GetProperty("Type").GetString(); // Hämta fordonstyp

            return type switch
            {
                "Car" => new Car(registrationNumber), // Skapa ny bil
                "MC" => new MC(registrationNumber), // Skapa ny motorcykel
                _ => throw new NotSupportedException($"Unsupported vehicle type: {type}") // Hantera ogiltig fordonstyp
            };
        }
    }



    //  Metod för att skriva fordon till JSON


    public override void Write(Utf8JsonWriter writer, Vehicle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("RegistrationNumber", value.RegistrationNumber);
        writer.WriteString("Type", value.GetType().Name); // Skriv fordonstyp
        writer.WriteString("ArrivalTime", value.ArrivalTime.ToString("o")); // Skriv ankomsttid
        writer.WriteEndObject();
    }
}



  //  Parkeringsgarage klass

public class ParkingGarage
{
    private List<ParkingSpot> parkingSpots; // Lista över parkeringsplatser
    private readonly string dataFilePath = "parkingData.json"; // Filväg för data
    private readonly string priceConfigPath = "priceConfig.json"; // Filväg för priskonfiguration

    public ParkingGarage()
    {
        LoadConfiguration(); 
        LoadData(); 
    }



    //  Metod för att ladda konfiguration


    private void LoadConfiguration()
    {
        try
        {
            if (File.Exists(priceConfigPath))
            {
                var configData = File.ReadAllText(priceConfigPath); 
                var config = JsonSerializer.Deserialize<Dictionary<string, int>>(configData); 

                int spotCount = config.GetValueOrDefault("SpotCount", 100); // Antal parkeringsplatser
                int defaultSpotSize = config.GetValueOrDefault("DefaultSpotSize", 1); // Standardstorlek på plats

                parkingSpots = Enumerable.Range(0, spotCount)
                                         .Select(_ => new ParkingSpot(defaultSpotSize)) // Skapa parkeringsplatser
                                         .ToList();
            }
            else
            {
                Console.WriteLine("Configuration file missing. Using default settings."); 
                parkingSpots = Enumerable.Range(0, 100)
                                         .Select(_ => new ParkingSpot(1)) 
                                         .ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration file: {ex.Message}");     // Felmeddelande!
            parkingSpots = new List<ParkingSpot>();
        }
    }




       // Metod för att ladda data


    private void LoadData()
    {
        try
        {
            if (File.Exists(dataFilePath))
            {
                var options = new JsonSerializerOptions { Converters = { new VehicleConverter() }, PropertyNameCaseInsensitive = true };
                var jsonData = File.ReadAllText(dataFilePath); // Läs datan
                parkingSpots = JsonSerializer.Deserialize<List<ParkingSpot>>(jsonData, options); 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data file: {ex.Message}");       //    Felmeddelande!
        }
    }




        // Metod för att spara data


    private void SaveData()
    {
        try
        {
            var options = new JsonSerializerOptions { Converters = { new VehicleConverter() }, WriteIndented = true };
            var jsonData = JsonSerializer.Serialize(parkingSpots, options); // Serialisera parkeringsplatser
            File.WriteAllText(dataFilePath, jsonData); // Skriv data till fil
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}"); // Felmeddelande
        }
    }




    // Metod för att parkera fordon



    public void ParkVehicle(Vehicle vehicle)
    {
        foreach (var spot in parkingSpots)    // Loopa igenom parkeringsplatser
        {
            if (!spot.IsFull)   // Kontrollera om platsen är full
            {
                spot.ParkVehicle(vehicle);    // Parkera fordonet
                SaveData();     // Spara data
                Console.WriteLine($"{vehicle.GetType().Name} parked with registration number {vehicle.RegistrationNumber}.");   // Meddelande om parkering
                return;
            }
        }
        Console.WriteLine("No available spots."); // Meddelande om att inga platser är lediga
    }




    // Metod för att flytta fordon
    public void MoveVehicle(int fromSpot, int toSpot)
    {
        if (fromSpot < 1 || fromSpot > parkingSpots.Count || toSpot < 1 || toSpot > parkingSpots.Count)
        {
            Console.WriteLine("Invalid spots."); // Felmeddelande om ogiltiga platser
            return;
        }

        var sourceSpot = parkingSpots[fromSpot - 1]; // Källa
        var destinationSpot = parkingSpots[toSpot - 1]; // Destination

        if (sourceSpot.Vehicles.Count > 0 && !destinationSpot.IsFull)
        {
            var vehicle = sourceSpot.Vehicles[0]; // Hämta fordonet från källan
            sourceSpot.RemoveVehicle(vehicle); // Ta bort fordonet från källan
            destinationSpot.ParkVehicle(vehicle); // Parkera fordonet på destinationen
            SaveData(); // Spara data
            Console.WriteLine($"{vehicle.RegistrationNumber} moved from spot {fromSpot} to {toSpot}."); // Meddelande om flytt
        }
        else
        {
            Console.WriteLine("Invalid move."); // Felmeddelande om ogiltig flytt
        }
    }




    // Metod för att ta bort fordon
    public void RemoveVehicle(string regNumber)
    {
        foreach (var spot in parkingSpots) // Loopa igenom parkeringsplatser
        {
            var vehicle = spot.Vehicles.FirstOrDefault(v => v.RegistrationNumber == regNumber); // Hämta fordonet baserat på registreringsnummer
            if (vehicle != null) // Kontrollera om fordonet finns
            {
                var duration = vehicle.GetParkingDuration(); // Hämta parkeringsduration
                int fee = (int)Math.Ceiling(duration.TotalMinutes / 60) * vehicle.HourlyRate; // Beräkna avgift
                spot.RemoveVehicle(vehicle); // Ta bort fordonet från platsen
                SaveData(); // Spara data
                Console.WriteLine($"Vehicle {regNumber} removed. Parked for {duration.TotalMinutes} minutes. Fee: {fee} CZK."); // Meddelande om borttagning
                return; // Avsluta metoden
            }
        }
        Console.WriteLine("Vehicle not found."); // Meddelande om att fordonet inte hittades
    }




    // Metod för att skriva ut parkeringsplatser
    public void PrintParkingSpots()
    {
        for (int i = 0; i < parkingSpots.Count; i++)
        {
            Console.WriteLine($"Spot {i + 1}: {(parkingSpots[i].Vehicles.Count == 0 ? "Empty" : string.Join(", ", parkingSpots[i].Vehicles.Select(v => v.RegistrationNumber)))}"); // Skriv ut parkeringsstatus
        }
    }
}




// Huvudprogrammet
class Program
{
    static void Main()
    {
        ParkingGarage garage = new ParkingGarage(); // Skapa en ny instans av parkeringsgaraget

        bool running = true;
        while (running)
        {
            Console.WriteLine("1. Park vehicle\n2. Move vehicle\n3. Remove vehicle\n4. Print parking spots\n5. Exit"); // Meny
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter vehicle type (CAR/MC): "); // Begär fordonstyp
                    string type = Console.ReadLine().ToUpper();
                    Console.Write("Enter registration number: "); // Begär registreringsnummer
                    string regNumber = Console.ReadLine();

                    Vehicle vehicle = type switch
                    {
                        "CAR" => new Car(regNumber), // Skapa en ny bil
                        "MC" => new MC(regNumber), // Skapa en ny motorcykel
                        _ => null
                    };
                    if (vehicle != null)
                        garage.ParkVehicle(vehicle); // Parkera fordonet
                    else
                        Console.WriteLine("Invalid vehicle type."); // Felmeddelande om ogiltig fordonstyp
                    break;

                case "2":
                    Console.Write("Enter from spot: "); // Begär från plats
                    if (int.TryParse(Console.ReadLine(), out int fromSpot))
                    {
                        Console.Write("Enter to spot: "); // Begär till plats
                        if (int.TryParse(Console.ReadLine(), out int toSpot))
                            garage.MoveVehicle(fromSpot, toSpot); // Flytta fordonet
                        else
                            Console.WriteLine("Invalid spot number."); // Felmeddelande om ogiltigt platsnummer
                    }
                    else
                        Console.WriteLine("Invalid spot number."); // Felmeddelande om ogiltigt platsnummer
                    break;

                case "3":
                    Console.Write("Enter registration number: "); // Begär registreringsnummer för borttagning
                    regNumber = Console.ReadLine();
                    garage.RemoveVehicle(regNumber); // Ta bort fordonet
                    break;

                case "4":
                    garage.PrintParkingSpots(); // Skriv ut parkeringsplatser
                    break;

                case "5":
                    running = false; // Avsluta programmet
                    break;

                default:
                    Console.WriteLine("Invalid option."); // Felmeddelande om ogiltig option
                    break;
            }
        }
    }
}
