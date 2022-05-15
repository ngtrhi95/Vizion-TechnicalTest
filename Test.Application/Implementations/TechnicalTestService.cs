using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Test.Application.Interfaces;
using Test.Domain.Entities;
using Test.Model.ViewModel;

namespace Test.Application.Implementations
{
    public class TechnicalTestService : ITechnicalTestService
    {
        private readonly IMongoCollection<TechnicalTest> _technicalTestCollection;
        private BlobStorageSetting _blobStorageSetting;

        public TechnicalTestService(IOptions<TechnicalTestDatabaseSetting> technicalTestDatabaseSettings,
            IOptions<BlobStorageSetting> blobStorageSetting)
        {
            var mongoClient = new MongoClient(
            technicalTestDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                technicalTestDatabaseSettings.Value.DatabaseName);

            _technicalTestCollection = mongoDatabase.GetCollection<TechnicalTest>(
                technicalTestDatabaseSettings.Value.CollectionName);

            _blobStorageSetting = blobStorageSetting.Value;
        }

        public async Task<List<TechnicalTest>> GetAsync() =>
        await _technicalTestCollection.Find(_ => true).ToListAsync();

        public async Task<TechnicalTest?> GetAsync(string id) =>
            await _technicalTestCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<TechnicalTest?> CreateAsync(TechnicalTestViewModel newTestViewModel)
        {
            TechnicalTest newTest = new TechnicalTest(newTestViewModel);

            await _technicalTestCollection.InsertOneAsync(newTest);
            await UploadFileAndUpdateDataAsync(newTestViewModel, newTest);
            return newTest;
        }


        public async Task<TechnicalTest?> UpdateAsync(string id, TechnicalTestViewModel updatedTestViewModel)
        {
            TechnicalTest technicalTest = await GetAsync(id);

            if (technicalTest != null)
            {
                await DeleteFileToBlobStorageAsync(technicalTest);

                TechnicalTest updatedTest = new TechnicalTest(updatedTestViewModel);
                updatedTest.Id = technicalTest.Id;
                await UploadFileAndUpdateDataAsync(updatedTestViewModel, updatedTest);
            }
            return technicalTest;
        }

        
        public async Task<TechnicalTest?> RemoveAsync(string id)
        {
            TechnicalTest technicalTest = await GetAsync(id);

            if (technicalTest != null)
            {
                await DeleteFileToBlobStorageAsync(technicalTest);
                await _technicalTestCollection.DeleteOneAsync(x => x.Id == id);
            }
            return technicalTest;
        }

        private async Task UploadFileAndUpdateDataAsync(TechnicalTestViewModel newTestViewModel, TechnicalTest newTest)
        {
            bool uploadFileResult = await UploadFileToBlobStorageAsync(newTestViewModel, newTest.Id);
            if (uploadFileResult)
            {
                string downloadUrl = string.Format("{0}/{1}/{2}/{3}", _blobStorageSetting.BaseStorageUrl, _blobStorageSetting.ContainerName, newTest.Id, newTestViewModel.File.FileName);
                newTest.DownloadUrl = downloadUrl;
                await _technicalTestCollection.ReplaceOneAsync(x => x.Id == newTest.Id, newTest);

            }
        }

        private async Task<bool> UploadFileToBlobStorageAsync(TechnicalTestViewModel newTestViewModel, string id)
        {
            Tuple<string, string> resultCopyFile = await CopyFileToTempFolderAsync(newTestViewModel.File, id);
            // intialize BobClient 
            BlobClient blobClient = new BlobClient(
                connectionString: _blobStorageSetting.ConnectionString,
                blobContainerName: _blobStorageSetting.ContainerName,
                blobName: string.Format("{0}/{1}", id, newTestViewModel.File.FileName)
            );

            // upload the file success then delete temp folder
            try
            {
                await blobClient.UploadAsync(resultCopyFile.Item1);
                Directory.Delete(resultCopyFile.Item2);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task DeleteFileToBlobStorageAsync(TechnicalTest technicalTest)
        {
            string fileName = technicalTest.DownloadUrl.Substring(technicalTest.DownloadUrl.LastIndexOf('/'));
            // intialize BobClient 
            BlobClient blobClient = new BlobClient(
                connectionString: _blobStorageSetting.ConnectionString,
                blobContainerName: _blobStorageSetting.ContainerName,
                blobName: string.Format("{0}/{1}", technicalTest.Id, fileName)
            );

            // delete file
            await blobClient.DeleteIfExistsAsync();
        }

        private async Task<Tuple<string, string>> CopyFileToTempFolderAsync(IFormFile file, string id)
        {
            string testFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", id);
            if (!Directory.Exists(testFolder))
            {
                Directory.CreateDirectory(testFolder);
            }
            string path = Path.Combine(testFolder, file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Tuple.Create(path, testFolder);
        }
    }
}
