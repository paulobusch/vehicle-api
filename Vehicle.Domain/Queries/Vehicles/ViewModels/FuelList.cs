﻿using Questor.Vehicle.Domain.Mutations.Vehicles.Entities.Enums;
using Questor.Vehicle.Domain.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questor.Vehicle.Domain.Queries.Vehicles.ViewModels
{
    public class FuelList : IViewModel
    {
        public EFuel Id { get; set; }
        public string Name { get; set; }
    }
}
