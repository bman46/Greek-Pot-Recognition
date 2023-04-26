using Greek_Pot_Recognition.Services;
using Greek_Pot_Recognition.Services.ML;
using Greek_Pot_Recognition.Tables.Items;
using Greek_Pot_Recognition.Tables.Repository;
using Greek_Pot_Recognition.Tables.Repository.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
builder.Services.AddSingleton<GridFSBucket>(opts =>
{
    var config = new ConfigHandlingService();
    var client = new MongoClient(config.MongoDBConnectionString);
    var db = client.GetDatabase("greekImages");
    var options = new GridFSBucketOptions
    {
        BucketName = "user_files_bucket",
        ChunkSizeBytes = 255 * 1024 //255 MB is the default value
    };
    return new GridFSBucket(db, options);
});
builder.Services.AddSingleton<IUploadRepository, UploadRepository>();
builder.Services.AddSingleton<IFilesRepository, FileRepository>();
builder.Services.AddSingleton<ImageRecognizer>(opts =>
{
    var config = new ConfigHandlingService();
    return new ImageRecognizer(config.Endpoint, config.Key, config.ProjectId, config.ProjectName);
});

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
            tusdotnet.Models.Metadata guid;
            tusdotnet.Models.Metadata filetype;
            if (metadata.TryGetValue("guid", out guid) && metadata.TryGetValue("filetype", out filetype))
            {
                // Upload to GridFS:
                IFilesRepository gridFSBucket = app.Services.GetRequiredService<IFilesRepository>();
                var result = await gridFSBucket.UploadAsync(file, eventContext.CancellationToken);

                UploadedFile userFile = new UploadedFile();
                userFile.UploadGuid = guid.GetString(System.Text.Encoding.Default);
                userFile.FileID = result.ToString();
                userFile.MimeType = filetype.GetString(System.Text.Encoding.Default);

                // Process ML
                try
                {
                    ImageRecognizer rec = app.Services.GetRequiredService<ImageRecognizer>();
                    using(var content = await file.GetContentAsync(eventContext.CancellationToken))
                    {
                        userFile.FileResult = rec.Classify(content);
                        Console.WriteLine("Prediction: " + userFile.FileResult.First());
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    userFile.FileResult = null;
                }

                // Add file to DB:
                IUploadRepository uploadRepository = app.Services.GetRequiredService<IUploadRepository>();
                await uploadRepository.CreateNewFileAsync(userFile);
            }
            else
            {
                Console.WriteLine("Failed to write GUID.");
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

