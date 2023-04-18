using System;
using System.Data;
using Greek_Pot_Recognition.Tables.Items;
using Greek_Pot_Recognition.Tables.Repository.Interfaces;
using MongoDB.Driver;

namespace Greek_Pot_Recognition.Tables.Repository
{
    public class UploadRepository : IUploadRepository
    {
        private readonly IMongoCollection<UploadedFile> _fileCollection;

        public UploadRepository(IMongoDatabase mongoDatabase)
        {
            _fileCollection = mongoDatabase.GetCollection<UploadedFile>("uploads");
        }
        // CRUD Operations:
        #region Create
        public async Task CreateNewFileAsync(UploadedFile newfile)
        {
            await _fileCollection.InsertOneAsync(newfile);
        }
        #endregion Create
        #region Read
        public async Task<List<UploadedFile>> GetAllAsync()
        {
            return await _fileCollection.Find(_ => true).ToListAsync();
        }
        public async Task<UploadedFile> GetByBsonIdAsync(string id)
        {
            return await _fileCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<UploadedFile> GetByFileGuidAsync(string guid)
        {
            return await _fileCollection.Find(_ => _.UploadGuid == guid).FirstOrDefaultAsync();
        }
        public async Task<List<UploadedFile>> GetByLinqAsync(System.Linq.Expressions.Expression<Func<UploadedFile, bool>> filter, FindOptions? options = null)
        {
            return await _fileCollection.Find(filter, options).ToListAsync();
        }
        #endregion Read
        #region Update
        public async Task UpdateFileAsync(UploadedFile userToUpdate)
        {
            await _fileCollection.ReplaceOneAsync(x => x.Id == userToUpdate.Id, userToUpdate);
        }
        #endregion Update
        #region Delete
        public async Task DeleteFileAsync(string id)
        {
            await _fileCollection.DeleteOneAsync(x => x.Id == id);
        }
        #endregion Delete
    }
}

