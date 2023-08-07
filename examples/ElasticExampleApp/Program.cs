using ElasticExampleApp;
using Microsoft.AspNetCore.Mvc;

var connectionString = "http://localhost:9200"; // Elasticsearch bağlantı adresi

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddSingleton<IRepository<Message>>(new ElasticsearchRepository<Message>(connectionString, indexName));
builder.Services.AddSingleton<IMessageRepository>(new MessageRepository(connectionString, indexName: "messages"));
builder.Services.AddSingleton<IECommerceRepository>(new ECommerceRepository(connectionString, indexName: "kibana_sample_data_ecommerce"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true);
    });
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseHttpsRedirection();

app.UseHttpsRedirection();

app.MapGet("/GetAllMessage", async ([FromServices] IMessageRepository messageRepository) =>
{
    var allMessages = await messageRepository.GetAllAsync();

    return allMessages;
})
.WithName("GetAllMessage")
.WithOpenApi();

app.MapGet("/GetMessageById", async (int id, [FromServices] IMessageRepository messageRepository) =>
{
    var message = await messageRepository.GetAsync(id);

    return message;
})
.WithName("GetMessageById")
.WithOpenApi();

app.MapPost("/AddMessage", async ([FromBody] Message message, [FromServices] IMessageRepository messageRepository) =>
{
    var isAddedData = await messageRepository.AddAsync(message);

    return isAddedData;
})
.WithName("AddMessage")
.WithOpenApi();

app.MapPut("/UpdateMessage", async ([FromBody] Message message, [FromServices] IMessageRepository messageRepository) =>
{
    var isUpdatedData = await messageRepository.UpdateAsync(message);

    return isUpdatedData;
})
.WithName("UpdateMessage")
.WithOpenApi();

app.MapDelete("/DeleteMessage", async (int id, [FromServices] IMessageRepository messageRepository) =>
{
    var isDeletedData = await messageRepository.DeleteAsync(id);

    return isDeletedData;
})
.WithName("DeleteMessage")
.WithOpenApi();

app.MapGet("/GetAllECommerceWithCustomerFirstName", async (string customerFirstName, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var allECommerce = await eCommerceRepository.TermQuerySearchWithCustomerFirstName(customerFirstName);

    return allECommerce;
})
.WithName("GetAllECommerceWithCustomerFirstName")
.WithOpenApi();

app.MapGet("/GetAllECommercePrefixCustomerFullName", async (string customerFirstNamePrefix, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var allECommerce = await eCommerceRepository.PrefixQuerySearchWithCustomerFullName(customerFirstNamePrefix);

    return allECommerce;
})
.WithName("GetAllECommercePrefixCustomerFullName")
.WithOpenApi();

app.MapGet("/GetAllECommerceRangeQuerySearchWithTaxfulPrice", async (double from, double to, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var allECommerce = await eCommerceRepository.RangeQuerySearchWithTaxfulPrice(from, to);

    return allECommerce;
})
.WithName("GetAllECommerceRangeQuerySearchWithTaxfulPrice")
.WithOpenApi();

app.MapGet("/GetAllECommerceFuzzyQuerySearchWithCustomerFirstName", async (string customerFirstNameFuzzy, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var allECommerce = await eCommerceRepository.FuzzyQuerySearchWithCustomerFirstName(customerFirstNameFuzzy);

    return allECommerce;
})
.WithName("GetAllECommerceFuzzyQuerySearchWithCustomerFirstName")
.WithOpenApi();

app.MapGet("/GetAllECommerceMatchPhraseWithCustomerFullName", async (string customerFullName, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var allECommerce = await eCommerceRepository.MatchPhraseWithCustomerFullName(customerFullName);

    return allECommerce;
})
.WithName("GetAllECommerceMatchPhraseWithCustomerFullName")
.WithOpenApi();

app.MapPost("/GetAllECommerceCompoundQuery", async ([FromBody] EcommerceSearchDTO ecommerceSearch, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var allECommerce = await eCommerceRepository.CompoundQuery(ecommerceSearch);

    return allECommerce;
})
.WithName("GetAllECommerceCompoundQuery")
.WithOpenApi();

app.MapPost("/GetAllECommerceCustomQuery", async ([FromBody] EcommerceSearchDTO ecommerceSearch, [FromServices] IECommerceRepository eCommerceRepository) =>
{
    var (list, totalCount) = await eCommerceRepository.CustomQuery(ecommerceSearch);

    return new PagingResponseDTO<List<ECommerce>> { Response = list, TotalCount = totalCount };
})
.WithName("GetAllECommerceCustomQuery")
.WithOpenApi();

app.Run();