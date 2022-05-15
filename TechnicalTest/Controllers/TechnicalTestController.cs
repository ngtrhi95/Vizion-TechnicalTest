using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Application.Interfaces;
using Test.Domain.Enums;
using Test.Model.ViewModel;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechnicalTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechnicalTestController : ControllerBase
    {
        private readonly ITechnicalTestService _technicalTestService;


        public TechnicalTestController(ITechnicalTestService technicalTestService) =>
            _technicalTestService = technicalTestService;

        [HttpGet]
        public async Task<List<Test.Domain.Entities.TechnicalTest>> Get()
        {
            return await _technicalTestService.GetAsync();
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Test.Domain.Entities.TechnicalTest>> Get(string id)
        {
            var technicalTest = await _technicalTestService.GetAsync(id);

            if (technicalTest is null)
            {
                return NotFound();
            }

            return technicalTest;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] TechnicalTestViewModel newTechnicalTest)
        {
            newTechnicalTest.Position = newTechnicalTest.Position.ToUpper();
            string invalidPosition = ValidatePositionValue(newTechnicalTest.Position);
            if (!string.IsNullOrEmpty(invalidPosition))
            {
                return BadRequest(invalidPosition);
            }

            string invalidUploadedFile = ValidatePositionValue(newTechnicalTest.Position);
            if (!string.IsNullOrEmpty(invalidUploadedFile))
            {
                return BadRequest(invalidUploadedFile);
            }

            Test.Domain.Entities.TechnicalTest technicalTest = await _technicalTestService.CreateAsync(newTechnicalTest);

            return CreatedAtAction(nameof(Get), new { id = technicalTest.Id }, technicalTest);
        }

        [HttpPut("{id:length(24)}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(string id, [FromForm] TechnicalTestViewModel newTechnicalTest)
        {
            newTechnicalTest.Position = newTechnicalTest.Position.ToUpper();
            string invalidPosition = ValidatePositionValue(newTechnicalTest.Position);
            if (!string.IsNullOrEmpty(invalidPosition))
            {
                return BadRequest(invalidPosition);
            }

            string invalidUploadedFile = ValidatePositionValue(newTechnicalTest.Position);
            if (!string.IsNullOrEmpty(invalidUploadedFile))
            {
                return BadRequest(invalidUploadedFile);
            }

            Test.Domain.Entities.TechnicalTest technicalTest = await _technicalTestService.UpdateAsync(id, newTechnicalTest);

            if (technicalTest is null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var technicalTest = await _technicalTestService.RemoveAsync(id);

            if (technicalTest is null)
            {
                return NotFound();
            }
            return NoContent();
        }

        private string ValidateUploadedFile(IFormFile uploadedFile)
        {
            int MaxContentLength = 1024 * 1024 * 5; //Size = 5 MB

            IList<string> AllowedFileExtensions = new List<string> { ".doc", ".docx", ".txt", ".pdf" };
            var ext = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));
            var extension = ext.ToLower();
            if (!AllowedFileExtensions.Contains(extension))
            {
                return "Please Upload file of type .doc,.docx,.txt, .pdf";
            }

            if (uploadedFile.Length > MaxContentLength)
            {
                return "Please Upload a file upto 5 mb.";
            }
            return null;
        }

        private string ValidatePositionValue(string position)
        {
            if (!Enum.GetNames(typeof(PositionValues)).ToList().Contains(position))
            {
                return "Postion either FRONTEND or BACKEND";
            }
            return null;
        }
    }
}
