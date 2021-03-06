﻿using Microsoft.EntityFrameworkCore;
using Questor.Vehicle.Domain.Utils.Enums;
using Questor.Vehicle.Domain.Utils.Interfaces;
using Questor.Vehicle.Domain.Utils.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Questor.Vehicle.Domain.Mutations.Brands.Mutations
{
    public class UpdateBrand : IMutation
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public async Task<MutationResult> ValidateAsync(VehicleMutationsHandler handler)
        {
            if (string.IsNullOrWhiteSpace(Id)) return new MutationResult(EStatusCode.InvalidData, $"Paramter {nameof(Id)} is required");
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 200) return new MutationResult(EStatusCode.InvalidData, $"Paramter {nameof(Name)} is invalid");
            var exists = await handler.DbContext.Brands.AnyAsync(b => b.Id == Id);
            if (!exists) return new MutationResult(EStatusCode.NotFound, $"Brand with {nameof(Id)} does not exists");
            var existsName = await handler.DbContext.Brands.AnyAsync(b => b.Id != Id && b.Name == Name);
            if (existsName) return new MutationResult(EStatusCode.Conflict, $"Brandh with {nameof(Name)} already exists");
            return null;
        }

        public async Task<MutationResult> ExecuteAsync(VehicleMutationsHandler handler)
        {
            var brand = await handler.DbContext.Brands.FindAsync(Id);
            brand.SetData(name: Name);
            handler.DbContext.Brands.Update(brand);
            var rows = await handler.DbContext.SaveChangesAsync();
            return new MutationResult(rows);
        }
    }
}
