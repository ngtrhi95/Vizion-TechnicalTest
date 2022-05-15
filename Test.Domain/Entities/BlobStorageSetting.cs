using System;
namespace Test.Domain.Entities
{
    public class BlobStorageSetting
    {
        public string AccountName { get; set; } = null!;

        public string Key { get; set; } = null!;

        public string ConnectionString { get; set; } = null!;

        public string ContainerName { get; set; }

        public string BaseStorageUrl { get; set; }
    }
}
