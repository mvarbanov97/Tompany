using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Data.Seeding
{
    public class CitiesSeeder : ISeeder
    {
        private readonly IHostingEnvironment hosting;


        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Trips.Any())
            {
                return;
            }

            var filePath = Path.Combine(this.hosting.ContentRootPath, "Seeding/cities.json");
            var json = File.ReadAllText(filePath);
            var cities = JsonConvert.DeserializeObject<IEnumerable<Trip>>(json);

            await dbContext.Trips.AddRangeAsync(cities);
        }
    }
}
