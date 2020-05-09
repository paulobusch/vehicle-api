﻿using Questor.Vehicle.Domain.Queries.Announcements;
using Questor.Vehicle.Domain.Queries.Announcements.ViewModels;
using Questor.Vehicle.Domain.Utils.Enums;
using Questor.Vehicle.Domain.Utils.Random;
using Questor.Vehicle.Domain.Utils.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vehicle.IntegrationTests.Tests.Announcements
{
    public class ListAnnouncementTest : BaseTests
    {
        public ListAnnouncementTest(VehicleFixture fixture) : base(fixture, "/announcements") { }

        public static IEnumerable<object[]> ListAnnouncementData()
        {
            yield return new object[] { EStatusCode.Success, new ListAnnouncement { } };
            yield return new object[] { EStatusCode.InvalidData, new ListAnnouncement { Page = -1, Limit = 0 } };
            yield return new object[] { EStatusCode.InvalidData, new ListAnnouncement { Page = 0, Limit = 10, SortAsc = EOrder.Desc, SortColumn = RandomId.NewId() } };
            yield return new object[] { EStatusCode.Success, new ListAnnouncement { Page = 0, Limit = 10, SortAsc = EOrder.Asc, SortColumn = "date_sale" } };
            yield return new object[] { EStatusCode.Success, new ListAnnouncement { Page = 0, Limit = 10, SortAsc = EOrder.Asc, SortColumn = "date_sale", BrandId = RandomId.NewId(), DataSale = DateTime.Now, ModelId = RandomId.NewId(), Year = 2010 } };
            yield return new object[] { EStatusCode.Success, new ListAnnouncement { Page = 0, Limit = 10, SortAsc = EOrder.Asc, SortColumn = "date_sale", BrandId = RandomId.NewId(), DataSale = DateTime.Now, ModelId = RandomId.NewId(), Year = 2010, OnlyUnsold = true } };
            yield return new object[] { EStatusCode.Success, new ListAnnouncement { Page = 0, Limit = 10, SortAsc = EOrder.Asc, SortColumn = "date_sale", BrandId = RandomId.NewId(), DataSale = DateTime.Now, ModelId = RandomId.NewId(), Year = 2010, OnlyUnsold = true }, true };
        }

        [Theory]
        [MemberData(nameof(ListAnnouncementData))]
        public async void ListAnnouncement(
            EStatusCode expectedStatus,
            ListAnnouncement query,
            bool? exactlyAnnouncement = false
        ) {
            var vehicle = EntitiesFactory.NewVehicle(
                brandId: query.BrandId, 
                modelId: query.ModelId, 
                year: query.Year
            ).Save();
            var announcement = EntitiesFactory.NewAnnouncement(
                vehicle: vehicle, 
                dateSale: query.OnlyUnsold ? null : query.DataSale
            ).Save();

            var (status, result) = await Request.Get<QueryResultList<AnnouncementList>>(Uri, query);
            Assert.Equal(expectedStatus, status);
            if (expectedStatus == EStatusCode.Success) { 
                Assert.True(result.TotalRows > 0); 
                Assert.True(result.TotalRows <= query.Limit); 
            }
            if (exactlyAnnouncement.Value)
            {
                var listAnnouncement = result.Data.ToList();
                var announcementResult = listAnnouncement
                    .Where(a => a.Id == announcement.Id)
                    .FirstOrDefault();
                Assert.NotNull(announcementResult);
                Assert.Equal(announcement.PricePurchase, announcementResult.PricePurchase);
                Assert.Equal(announcement.PriceSale, announcementResult.PriceSale);
                Assert.Equal(announcement.DateSale, announcementResult.DateSale);
                Assert.Equal(announcement.Vehicle.Year, announcementResult.VehicleYear);
                Assert.Equal(announcement.Vehicle.Brand.Name, announcementResult.VehicleBrand);
                Assert.Equal(announcement.Vehicle.Model.Name, announcementResult.VehicleModel);
            }
        }
    }
}
