﻿using Questor.Vehicle.Domain.Queries;
using Questor.Vehicle.Domain.Utils.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Questor.Vehicle.Domain.Utils.Interfaces
{
    public interface IQuery<TData>
        where TData : class
    {
        Task<QueryResult<TData>> ValidateAsync(VehicleQueriesHandler handler);
        Task<QueryResult<TData>> ExecuteAsync(VehicleQueriesHandler handler);
    }
}
