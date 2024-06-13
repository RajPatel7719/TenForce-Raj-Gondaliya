using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            #region Alternative solution
                        
            //AverageMoonGravity = (float)Moons.Average(m => m.MassValue * Math.Pow(10, m.MassExponent));

            // Benefits:
            //  - Less error-prone, as it eliminates the need for manual iteration and calculation
            //  - Can be more efficient, as LINQ can optimize the calculation

            // Drawbacks:
            //  - May be less readable for developers not familiar with LINQ
            //  - Can be slower for very large collections of moons, as it creates an iterator

            #endregion
        }
    }
}
