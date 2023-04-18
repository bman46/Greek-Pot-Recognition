using Greek_Pot_Recognition.Services;
using Greek_Pot_Recognition.Tables.Repository;
using Greek_Pot_Recognition.Tables.Repository.Interfaces;
using MongoDB.Driver;
using tusdotnet;
using tusdotnet.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IMongoDatabase>(options =>
{
    var config = new ConfigHandlingService();
    var client = new MongoClient(config.MongoDBConnectionString);
    return client.GetDatabase("greekImages");
});
builder.Services.AddSingleton<IUploadRepository, UploadRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// TUS:
app.MapTus("/upload", async httpContext => new()
{
    // This method is called on each request so different configurations can be returned per user, domain, path etc.
    // Return null to disable tusdotnet for the current request.

    // Where to store data?
    MaxAllowedUploadSizeInBytesLong = 10000000,
    Store = new tusdotnet.Stores.TusDiskStore(Environment.GetEnvironmentVariable("UPLOAD_LOCATION")),
    Events = new()
    {
        // What to do when file is completely uploaded?
        OnFileCompleteAsync = async eventContext =>
        {
            tusdotnet.Interfaces.ITusFile file = await eventContext.GetFileAsync();
            Dictionary<string, tusdotnet.Models.Metadata> metadata = await file.GetMetadataAsync(eventContext.CancellationToken);
            using (Stream content = await file.GetContentAsync(eventContext.CancellationToken)) {
                // TODO: Implement
                //await DoSomeProcessing(content, metadata);
                Console.WriteLine(metadata);
            }
        },
        OnBeforeCreateAsync = ctx =>
        {
            if (!ctx.Metadata.ContainsKey("guid"))
            {
                ctx.FailRequest("File must have a GUID.");
            }
            return Task.CompletedTask;
        }
    }
}); ;

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

