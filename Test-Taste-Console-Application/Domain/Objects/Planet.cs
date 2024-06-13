using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class Planet
    {
        public string Id { get; set; }
        public float SemiMajorAxis { get; set; }
        public ICollection<Moon> Moons { get; set; }
        public float AverageMoonGravity { get; set; }

        public Planet(PlanetDto planetDto)
        {
            Id = planetDto.Id;
            SemiMajorAxis = planetDto.SemiMajorAxis;
            Moons = new Collection<Moon>();
            if (planetDto.Moons != null)
            {
                foreach (MoonDto moonDto in planetDto.Moons)
                {
                    Moons.Add(new Moon(moonDto));
                }
            }
        }

        public Boolean HasMoons()
        {
            return (Moons != null && Moons.Count > 0);
        }

        public void CalculateAverageMoonGravity()
        {
            // Check if the planet has no moons, then return immediately
            if (Moons == null || Moons.Count == 0) return;

            // Calculate the total gravity of all moons
            double totalGravity = 0;
            foreach (var moon in Moons)
            {
                // Calculate the gravity of each moon and add it to the total
                totalGravity += moon.MassValue * Math.Pow(10, moon.MassExponent);
            }

            // Calculate the average moon gravity by dividing the total gravity by the number of moons
            AverageMoonGravity = (float)(totalGravity / Moons.Count);
        }
    }
}
