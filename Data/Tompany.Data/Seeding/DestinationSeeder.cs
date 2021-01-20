using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Data.Seeding
{
    public class DestinationSeeder : ISeeder
    {
        private readonly IHostingEnvironment hosting;

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Trips.Any())
            {
                return;
            }

            var directory = Directory.GetCurrentDirectory();
            var json = File.ReadAllText(directory);
            var destinations = JsonConvert.DeserializeObject<IEnumerable<Destination>>(json);

            await dbContext.Destinations.AddRangeAsync(destinations);
        }
    }
}
