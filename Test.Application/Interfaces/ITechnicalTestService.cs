using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Domain.Entities;
using Test.Model.ViewModel;

namespace Test.Application.Interfaces
{
    public interface ITechnicalTestService
    {
        Task<List<TechnicalTest>> GetAsync();

        Task<TechnicalTest?> GetAsync(string id);

        Task<TechnicalTest?> CreateAsync(TechnicalTestViewModel newTest);

        Task<TechnicalTest?> UpdateAsync(string id, TechnicalTestViewModel newTestViewModel);

        Task<TechnicalTest?> RemoveAsync(string id);
    }
}
