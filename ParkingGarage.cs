using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Praque_parking_oop_G;

public class ParkingGarage
{
    private List<ParkingSpot> parkingSpots;

    // Konstruktor som initialiserar antalet parkeringsrutor från konfigurationsfilen.
    public ParkingGarage(int numSpots)
    {
        parkingSpots = new List<ParkingSpot>(numSpots);
        for (int i = 0; i < numSpots; i++)
        {
            parkingSpots.Add(null);
        }
    }

    // Metod för att parkera ett fordon på första lediga plats eller fylla en MC-plats om möjligt.
    public void ParkVehicle(Vehicle vehicle)
    {
        for (int i = 0; i < parkingSpots.Count; i++)
        {
            if (parkingSpots[i] == null)
            {
                parkingSpots[i] = new ParkingSpot(vehicle);
                Console.WriteLine($"Fordon {vehicle.Type} med registreringsnummer {vehicle.RegNumber} parkerat på platsen {i + 1}.");
                SaveToFile("parking_data.json");
                return;
            }
        }
        Console.WriteLine("Inga lediga platser.");
    }

    // Metod för att flytta ett fordon från en ruta till en annan.
    public void MoveVehicle(int fromSpot, int toSpot)
    {
        try
        {
            if (parkingSpots[fromSpot - 1] != null && parkingSpots[toSpot - 1] == null)
            {
                parkingSpots[toSpot - 1] = parkingSpots[fromSpot - 1];
                parkingSpots[fromSpot - 1] = null;
                Console.WriteLine($"Fordonet har flyttats från platsen {fromSpot} till plats {toSpot}.");
                SaveToFile("parking_data.json");
            }
            else
            {
                throw new InvalidOperationException("Ogiltig flytt.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    // Metod för att ta bort ett fordon från en ruta och beräkna hur länge det varit parkerat.
    public void RemoveVehicle(string regNumber)
    {
        try
        {
            for (int i = 0; i < parkingSpots.Count; i++)
            {
                if (parkingSpots[i] != null && parkingSpots[i].Vehicle.RegNumber.Equals(regNumber.ToUpper()))
                {
                    var parkingDuration = DateTime.Now - parkingSpots[i].Vehicle.ArrivalTime;
                    parkingSpots[i] = null;
                    Console.WriteLine($"Vehicle {regNumber.ToUpper()} removed from spot {i + 1}. Parked for {parkingDuration.TotalMinutes} minutes.");
                    SaveToFile("parking_data.json");
                    return;
                }
            }
            throw new InvalidOperationException("Fordonet hittades inte.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    // Metod för att söka efter ett fordon med ett specifikt registreringsnummer.
    public void SearchVehicle(string regNumber)
    {
        for (int i = 0; i < parkingSpots.Count; i++)
        {
            if (parkingSpots[i] != null && parkingSpots[i].Vehicle.RegNumber.Equals(regNumber.ToUpper()))
            {
                Console.WriteLine($"Vehicle {regNumber.ToUpper()} found at spot {i + 1}.");
                return;
            }
        }
        Console.WriteLine("Fordonet hittades inte.");
    }

    // Metod för att skriva ut statusen för alla parkeringsrutor.
    public void PrintParkingSpots()
    {
        for (int i = 0; i < parkingSpots.Count; i++)
        {
            if (parkingSpots[i] != null)
            {
                Console.WriteLine($"Spot {i + 1}: {parkingSpots[i].Vehicle.Type} med registreringsnummer {parkingSpots[i].Vehicle.RegNumber}, Arrived: {parkingSpots[i].Vehicle.ArrivalTime}");
            }
            else
            {
                Console.WriteLine($"Spot {i + 1}: Empty");
            }
        }
    }

    // Metod för att skriva ut en sammanfattningsrapport över parkeringsplatsens status.
    public void PrintSummaryReport()
    {
        int emptySpots = 0, halfFullSpots = 0, fullSpots = 0;
        foreach (var spot in parkingSpots)
        {
            if (spot == null)
            {
                emptySpots++;
            }
            else if (spot.Vehicle.Type == "MC" && spot.Vehicle.RegNumber.Contains(","))
            {
                fullSpots++;
            }
            else
            {
                halfFullSpots++;
            }
        }
        Console.WriteLine($"Sammanfattande rapport:\nTomma platser: {emptySpots}\nHalvfulla platser: {halfFullSpots}\nFulla platser: {fullSpots}");
    }

    // Metod för att spara parkeringsdata till en JSON-fil.
    public void SaveToFile(string filePath)
    {
        var json = JsonConvert.SerializeObject(parkingSpots, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    // Metod för att ladda parkeringsdata från en JSON-fil.
    public void LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            parkingSpots = JsonConvert.DeserializeObject<List<ParkingSpot>>(json);
        }
    }
}

