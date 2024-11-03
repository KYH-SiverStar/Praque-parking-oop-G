using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Praque_parking_oop_G;

ParkingGarage garage;
try
{
    // Läsa konfigurationsdata från fil
    string configData = File.ReadAllText("config.json");
    var config = JsonConvert.DeserializeObject<Configuration>(configData);

    int numSpots = config.NumberOfSpots;
    garage = new ParkingGarage(numSpots);

    // Läsa parkeringsdata från fil
    garage.LoadFromFile("parking_data.json");

    bool running = true;
    while (running)
    {
        // Visar en meny med olika valmöjligheter
        Console.WriteLine("1. parkera fordon\n2. Flytta fordon\n3. Ta bort fordon\n4. Sök fordon\n5. Skriv ut parkeringsplatser\n6. Skriv ut sammanfattande rapport\n7. Utgång");
        Console.Write("Välj ett alternativ: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.Write("Ange fordonstyp (CAR/MC/BUS/BICYCLE): ");
                string type = Console.ReadLine().ToUpper();
                Console.Write("Ange registreringsnummer: ");
                string regNumber = Console.ReadLine().ToUpper();
                Vehicle vehicle;
                if (type == "CAR")
                {
                    vehicle = new Car(regNumber);
                }
                else if (type == "MC")
                {
                    vehicle = new MC(regNumber);
                }
                else if (type == "bus")
                {
                    vehicle = new Bus(regNumber);
                }
                else if (type == "bicycle")
                {
                    vehicle = new Bicycle(regNumber);
                }
                else
                {
                    Console.WriteLine("Ogiltig fordonstyp.");
                    continue;
                }
                garage.ParkVehicle(vehicle);
                break;
            case "2":
                Console.Write("ange från plats: ");
                if (int.TryParse(Console.ReadLine(), out int fromSpot))
                {
                    Console.Write("ange till plats: ");
                    if (int.TryParse(Console.ReadLine(), out int toSpot))
                    {
                        garage.MoveVehicle(fromSpot, toSpot);
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt platsnummer.");
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt platsnummer.");
                }
                break;
            case "3":
                Console.Write("Ange registreringsnummer: ");
                regNumber = Console.ReadLine().ToUpper();
                garage.RemoveVehicle(regNumber);
                break;
            case "4":
                Console.Write("Ange registreringsnummer: ");
                regNumber = Console.ReadLine().ToUpper();
                garage.SearchVehicle(regNumber);
                break;
            case "5":
                garage.PrintParkingSpots();
                break;
            case "6":
                garage.PrintSummaryReport();
                break;
            case "7":
                running = false;
                break;
            default:
                Console.WriteLine("Ogiltigt alternativ, försök igen.");
                break;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Fel vid läsning av konfigurationsfilen: {ex}");
}
