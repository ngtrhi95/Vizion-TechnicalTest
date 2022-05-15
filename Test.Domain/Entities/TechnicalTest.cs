using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Model.ViewModel;

namespace Test.Domain.Entities
{
    public class TechnicalTest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = null!;

        public string Note { get; set; }

        public string Position { get; set; } = null!;

        public string DownloadUrl { get; set; }

        public TechnicalTest (TechnicalTestViewModel viewModel)
        {
            Name = viewModel.Name;
            Note = viewModel.Note;
            Position = viewModel.Position;
        }
    }
}
