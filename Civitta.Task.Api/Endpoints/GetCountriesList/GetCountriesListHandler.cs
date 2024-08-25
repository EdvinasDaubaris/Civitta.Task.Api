using FastEndpoints;

namespace Civitta.Task.Api.Endpoints.GetCountriesList
{
    public class GetCountriesListHandler : EndpointWithoutRequest<List<GetCountriesListResponse>>
    {
        public override void Configure()
        {
            Get("/api/countries/list");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {


            var person = await dbContext.GetFirstPersonAsync();
        }
    }
}
