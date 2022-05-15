using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Test.Model.ViewModel
{
    public class TechnicalTestViewModel
    {
        [Required()]
        public string Name { get; set; } = null!;

        [Required()]
        public string Position { get; set; } = null!;

        public string Note { get; set; }

        [Required()]
        public IFormFile File { get; set; }
    }
}
