using System;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using tusdotnet.Interfaces;

namespace Greek_Pot_Recognition.Tables.Repository.Interfaces
{
	public interface IFilesRepository
	{
        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="file"The file to upload</param>
        /// <returns></returns>
        Task<ObjectId> UploadAsync(ITusFile file, CancellationToken token);
        /// <summary>
        /// Get a file by name
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>Byte array with file contents</returns>
        Task<byte[]> GetFileByNameAsync(string fileName);
        /// <summary>
        /// Get a file by ID
        /// </summary>
        /// <param name="fileName">File ID</param>
        /// <returns></returns>
        Task<byte[]> GetFileByIdAsync(string fileId);
    }
}

