using Civitta.Task1.Api.Endpoints.GetCountriesList;
using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FakeItEasy;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace test1
{
    public class CountriesTest
    {

        [Fact]
        public async void GetCountryList()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
            using var context = new DatabaseContext(options);

            context.Countries.Add(new Country { Id = 1, Code = "ltu", DataFromDate = DateTime.UtcNow, DataToDate = DateTime.UtcNow, Name = "Lithuania"  });
            context.SaveChanges();



            var endpoint = Factory.Create<GetCountriesListHandler>(
                context,
                A.Fake<IMemoryCache>(),
                A.Fake<IEnricoService>());

            await endpoint.HandleAsync(default);

            var response = endpoint.Response;

            Assert.NotNull(response);
            Assert.True(response.FirstOrDefault().CountryCode == "ltu");
            ;
        }

    }
}