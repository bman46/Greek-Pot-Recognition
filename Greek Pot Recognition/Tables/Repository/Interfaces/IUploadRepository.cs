using System;
using MongoDB.Driver;
using System.Data;
using Greek_Pot_Recognition.Tables.Items;

namespace Greek_Pot_Recognition.Tables.Repository.Interfaces
{
	public interface IUploadRepository
	{
        /// <summary>
        /// Create new File entry in DB
        /// </summary>
        /// <param name="newFile"></param>
        /// <returns></returns>
        Task CreateNewFileAsync(UploadedFile newFile);
        /// <summary>
        /// Get all Files
        /// </summary>
        /// <returns></returns>
        Task<List<UploadedFile>> GetAllAsync();
        /// <summary>
        /// Get File by Bson ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UploadedFile> GetByBsonIdAsync(string id);
        /// <summary>
        /// Get File by guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UploadedFile> GetByFileGuidAsync(string guid);
        /// <summary>
        /// Get Files with a linq filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<List<UploadedFile>> GetByLinqAsync(System.Linq.Expressions.Expression<Func<UploadedFile, bool>> filter, FindOptions? options = null);
        /// <summary>
        /// Update File in DB
        /// </summary>
        /// <param name="FileToUpdate"></param>
        /// <returns></returns>
        Task UpdateFileAsync(UploadedFile FileToUpdate);
        /// <summary>
        /// Delete File from DB using BSON id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteFileAsync(string id);
    }
}

