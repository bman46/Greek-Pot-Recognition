using Greek_Pot_Recognition.Extensions;
using Greek_Pot_Recognition.Services;
using Greek_Pot_Recognition.Tables.Items;
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
            using (Stream content = await file.GetContentAsync(eventContext.CancellationToken))
            {
                try
                {
                    // TODO: Implement
                    //await DoSomeProcessing(content, metadata);
                    // Create file object:
                    tusdotnet.Models.Metadata guid;
                    tusdotnet.Models.Metadata filetype;
                    if (metadata.TryGetValue("guid", out guid) && metadata.TryGetValue("filetype", out filetype))
                    {
                        UploadedFile userFile = new UploadedFile();
                        userFile.UploadGuid = guid.GetString(System.Text.Encoding.Default);
                        userFile.FileBase64 = content.ConvertToBase64();
                        userFile.MimeType = filetype.GetString(System.Text.Encoding.Default);
                        // Add file to DB:
                        IUploadRepository uploadRepository = app.Services.GetRequiredService<IUploadRepository>();
                        await uploadRepository.CreateNewFileAsync(userFile);
                    }
                    else
                    {
                        Console.WriteLine("Failed to write GUID.");
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("Error: " + e);
                }
            }
            var terminationStore = (ITusTerminationStore)eventContext.Store;
            await terminationStore.DeleteFileAsync(file.Id, eventContext.CancellationToken);
        },
        OnBeforeCreateAsync = ctx =>
        {
            if (!ctx.Metadata.ContainsKey("guid"))
            {
                ctx.FailRequest("File must have a GUID.");
            }
            if (!ctx.Metadata.ContainsKey("filetype"))
            {
                ctx.FailRequest("filetype metadata must be specified. ");
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

