using System;
using MongoDB.Bson;
using System.Net.Sockets;
using MongoDB.Driver.GridFS;
using tusdotnet.Models.Configuration;
using tusdotnet.Interfaces;
using MongoDB.Driver;

namespace Greek_Pot_Recognition.Tables.Repository.Interfaces
{
	public class FileRepository : IFilesRepository
	{
		private readonly GridFSBucket _GridFSBucket;

        public FileRepository(GridFSBucket gridFSBucket)
		{
			_GridFSBucket = gridFSBucket;
        }
        public async Task<ObjectId> UploadAsync(ITusFile file, CancellationToken token)
        {
            Dictionary<string, tusdotnet.Models.Metadata> metadata = await file.GetMetadataAsync(token);
            using (Stream content = await file.GetContentAsync(token))
            {
                tusdotnet.Models.Metadata? guid;
                tusdotnet.Models.Metadata? filetype;
                if (metadata.TryGetValue("guid", out guid) && metadata.TryGetValue("filetype", out filetype))
                {
                    var options = new GridFSUploadOptions
                    {
                        Metadata = new BsonDocument { { "FileName", guid.GetString(System.Text.Encoding.Default) }, { "Type", filetype.GetString(System.Text.Encoding.Default) } }
                    };

                    using var stream = await _GridFSBucket.OpenUploadStreamAsync(guid.GetString(System.Text.Encoding.Default), options); // Open the output stream
                    var id = stream.Id; // Unique Id of the file
                    content.CopyTo(stream); // Copy the contents to the stream
                    await stream.CloseAsync();
                    return id;
                }
                else
                {
                    throw new ArgumentNullException("Missing required file attributes.");
                }
            }
        }
        public async Task<byte[]> GetFileByNameAsync(string fileName)
        {
            return await _GridFSBucket.DownloadAsBytesByNameAsync(fileName);

        }
        public async Task<byte[]> GetFileByIdAsync(string fileId)
        {
            var fileInfo = await FindFile(fileId);
            return await _GridFSBucket.DownloadAsBytesAsync(fileInfo.Id);
        }
        private async Task<GridFSFileInfo> FindFile(string fileName)
        {
            var options = new GridFSFindOptions
            {
                Limit = 1
            };
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
            using var cursor = await _GridFSBucket.FindAsync(filter, options);
            return (await cursor.ToListAsync()).FirstOrDefault();
        }
    }
}

