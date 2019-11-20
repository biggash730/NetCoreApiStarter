using GemBox.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MManager.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetCoreStarter.Utils
{
    public class ExcelHelpers
    {
        public static string GenerateRegionsUploadTemplate()
        {
            var filename = $"RegionsUploadTemplate_{DateTime.Now.ToUniversalTime().ToFileTime()}.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", filename);
            var ef = new ExcelFile();
            var sheet1 = ef.Worksheets.Add("Main");
            sheet1.Cells[0, 0].Value = "CODE";
            sheet1.Cells[0, 1].Value = "NAME";

            ef.Save(filePath);
            var bytes = File.ReadAllBytes(filePath);
            var file = Convert.ToBase64String(bytes);
            if (File.Exists(filePath)) File.Delete(filePath);
            return file;
        }

        public static void UploadRegions(string username, UploadFile data, ApplicationDbContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var fileName = data.Filename;
                fileName = StringGenerators.GenerateRandomString(16) + fileName.Replace(" ", "").ToLower();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", fileName);

                var base64Data = data.File.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", "");
                var binData = Convert.FromBase64String(base64Data);
                using (var stream = new MemoryStream(binData))
                {
                    File.WriteAllBytes(filePath, binData);
                }
                //gembox things
                var workbook = ExcelFile.Load(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == "Main");
                if (worksheet == null) throw new Exception("Invalid Upload File");
                var uploadData = new List<UploadModel>();
                var cnt = 0;
                foreach (var row in worksheet.Rows)
                {
                    cnt++;
                    if (cnt == 1) continue;
                    if (string.IsNullOrEmpty(row.Cells[0].GetFormattedValue()))
                        throw new Exception($"Please enter a valid code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[1].GetFormattedValue()))
                        throw new Exception($"Please enter a name for the record on line {cnt}");

                    var ud = new UploadModel
                    {
                        Code = row.Cells[0].GetFormattedValue(),
                        Name = row.Cells[1].GetFormattedValue()
                    };
                    uploadData.Add(ud);
                }


                foreach (var uData in uploadData)
                {
                    var existing = db.Regions.Where(x => x.Code == uData.Code || uData.Name == x.Name).FirstOrDefault();
                    if (existing != null)
                    {
                        if (existing.Code == uData.Code) throw new Exception($"There is already an existing record with this Code - {existing.Code}");
                        if (existing.Name == uData.Name) throw new Exception($"There is already an existing record with this Name - {existing.Name}");
                    }

                    // save the data
                    var newRec = new Models.Region
                    {
                        Code = uData.Code,
                        Name = uData.Name,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        CreatedBy = username,
                        ModifiedAt = DateTime.Now.ToUniversalTime(),
                        ModifiedBy = username
                    };
                    db.Regions.Add(newRec);
                    db.SaveChanges();
                }

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                transaction.Commit();
            }
        }

        public static string GenerateDistrictsUploadTemplate()
        {
            var filename = $"DistrictsUploadTemplate_{DateTime.Now.ToUniversalTime().ToFileTime()}.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", filename);
            var ef = new ExcelFile();
            var sheet1 = ef.Worksheets.Add("Main");
            sheet1.Cells[0, 0].Value = "CODE";
            sheet1.Cells[0, 1].Value = "NAME";
            sheet1.Cells[0, 2].Value = "REGION_CODE";

            ef.Save(filePath);
            var bytes = File.ReadAllBytes(filePath);
            var file = Convert.ToBase64String(bytes);
            if (File.Exists(filePath)) File.Delete(filePath);
            return file;
        }

        public static void UploadDistricts(string username, UploadFile data, ApplicationDbContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var fileName = data.Filename;
                fileName = StringGenerators.GenerateRandomString(16) + fileName.Replace(" ", "").ToLower();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", fileName);                
                
                var base64Data = data.File.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", "");
                var binData = Convert.FromBase64String(base64Data);
                using (var stream = new MemoryStream(binData))
                {
                    File.WriteAllBytes(filePath, binData);
                }
                //gembox things
                var workbook = ExcelFile.Load(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == "Main");
                if (worksheet == null) throw new Exception("Invalid Upload File");
                var uploadData = new List<UploadModel>();
                var cnt = 0;
                foreach (var row in worksheet.Rows)
                {
                    cnt++;
                    if (cnt == 1) continue;
                    if (string.IsNullOrEmpty(row.Cells[0].GetFormattedValue()))
                        throw new Exception($"Please enter a valid code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[1].GetFormattedValue()))
                        throw new Exception($"Please enter a name for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[2].GetFormattedValue()))
                        throw new Exception($"Please enter a region code for the record on line {cnt}");

                    var ud = new UploadModel
                    {
                        Code = row.Cells[0].GetFormattedValue(),
                        Name = row.Cells[1].GetFormattedValue(),
                        RegionCode = row.Cells[2].GetFormattedValue()
                    };
                    uploadData.Add(ud);
                }


                var regions = db.Regions.Where(x => !x.Hidden).ToList();
                foreach (var uData in uploadData)
                {
                    var existing = db.Districts.Where(x => (x.Code == uData.Code || uData.Name == x.Name) && x.Region.Code == uData.RegionCode).Include(x=> x.Region).FirstOrDefault();
                    if (existing != null)
                    {
                        if (existing.Code == uData.Code) throw new Exception($"There is already an existing record with this Code - {existing.Code} and \nRegion: {existing.Region?.Name}");
                        if (existing.Name == uData.Name) throw new Exception($"There is already an existing record with this Name - {existing.Name} and \nRegion: {existing.Region?.Name}");
                    }
                    var region = regions.FirstOrDefault(x => x.Code.ToLower() == uData.RegionCode.ToLower());
                    if(region == null) throw new Exception($"There is no region with this region code ({uData.RegionCode}). Please check and verify.");

                    // save the data
                    var newRec = new District
                    {
                        Code = uData.Code,
                        Name = uData.Name,
                        RegionId = region.Id,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        CreatedBy = username,
                        ModifiedAt = DateTime.Now.ToUniversalTime(),
                        ModifiedBy = username
                    };
                    db.Districts.Add(newRec);
                    //db.Entry<District>(newRec).State = EntityState.Detached;
                    db.SaveChanges();
                }

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                transaction.Commit();
            }
        }

        public static string GenerateConstituenciesUploadTemplate()
        {
            var filename = $"ConstituenciesUploadTemplate_{DateTime.Now.ToUniversalTime().ToFileTime()}.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", filename);
            var ef = new ExcelFile();
            var sheet1 = ef.Worksheets.Add("Main");
            sheet1.Cells[0, 0].Value = "CODE";
            sheet1.Cells[0, 1].Value = "NAME";
            sheet1.Cells[0, 2].Value = "DISTRICT_CODE";
            sheet1.Cells[0, 3].Value = "REGION_CODE";

            ef.Save(filePath);
            var bytes = File.ReadAllBytes(filePath);
            var file = Convert.ToBase64String(bytes);
            if (File.Exists(filePath)) File.Delete(filePath);
            return file;
        }

        public static void UploadConstituencies(string username, UploadFile data, ApplicationDbContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var fileName = data.Filename;
                fileName = StringGenerators.GenerateRandomString(16) + fileName.Replace(" ", "").ToLower();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", fileName);

                var base64Data = data.File.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", "");
                var binData = Convert.FromBase64String(base64Data);
                using (var stream = new MemoryStream(binData))
                {
                    File.WriteAllBytes(filePath, binData);
                }
                //gembox things
                var workbook = ExcelFile.Load(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == "Main");
                if (worksheet == null) throw new Exception("Invalid Upload File");
                var uploadData = new List<UploadModel>();
                var cnt = 0;
                foreach (var row in worksheet.Rows)
                {
                    cnt++;
                    if (cnt == 1) continue;
                    if (string.IsNullOrEmpty(row.Cells[0].GetFormattedValue()))
                        throw new Exception($"Please enter a valid code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[1].GetFormattedValue()))
                        throw new Exception($"Please enter a name for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[2].GetFormattedValue()))
                        throw new Exception($"Please enter a district code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[3].GetFormattedValue()))
                        throw new Exception($"Please enter a region code for the record on line {cnt}");

                    var ud = new UploadModel
                    {
                        Code = row.Cells[0].GetFormattedValue(),
                        Name = row.Cells[1].GetFormattedValue(),
                        DistrictCode = row.Cells[2].GetFormattedValue(),
                        RegionCode = row.Cells[3].GetFormattedValue()
                    };
                    uploadData.Add(ud);
                }


                foreach (var uData in uploadData)
                {
                    var existing = db.Constituencies.Where(x => (x.Code == uData.Code || uData.Name == x.Name) && x.District.Code == uData.DistrictCode && x.District.Region.Code == uData.RegionCode).Include(x => x.District.Region).FirstOrDefault();
                    if (existing != null)
                    {
                        if (existing.Code == uData.Code) throw new Exception($"There is already an existing record with this Code - {existing.Code}, \nDistrict: {existing.District?.Name} and \nRegion: {existing.District?.Region?.Name}");
                        if (existing.Name == uData.Name) throw new Exception($"There is already an existing record with this Name - {existing.Name}, \nDistrict: {existing.District?.Name} and \nRegion: {existing.District?.Region?.Name}");
                    }
                    var region = db.Regions.FirstOrDefault(x => x.Code.ToLower() == uData.RegionCode.ToLower());
                    if (region == null) throw new Exception($"There is no region with this region code ({uData.RegionCode}). Please check and verify.");
                    var district = db.Districts.FirstOrDefault(x => x.Code.ToLower() == uData.DistrictCode.ToLower() && x.RegionId == region.Id);
                    if (district == null) throw new Exception($"There is no district with this district code - ({uData.DistrictCode}) and region code - ({uData.RegionCode}). Please check and verify.");

                    // save the data
                    var newRec = new Constituency
                    {
                        Code = uData.Code,
                        Name = uData.Name,
                        DistrictId = district.Id,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        CreatedBy = username,
                        ModifiedAt = DateTime.Now.ToUniversalTime(),
                        ModifiedBy = username
                    };
                    db.Constituencies.Add(newRec);                    
                    db.SaveChanges();
                    //db.Entry<Constituency>(newRec).State = EntityState.Detached;
                }

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                transaction.Commit();
            }
        }

        public static string GenerateElectoralAreasUploadTemplate()
        {
            var filename = $"ElectoralAreasUploadTemplate_{DateTime.Now.ToUniversalTime().ToFileTime()}.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", filename);
            var ef = new ExcelFile();
            var sheet1 = ef.Worksheets.Add("Main");
            sheet1.Cells[0, 0].Value = "CODE";
            sheet1.Cells[0, 1].Value = "NAME";
            sheet1.Cells[0, 2].Value = "CONSTITUENCY_CODE";
            sheet1.Cells[0, 3].Value = "DISTRICT_CODE";
            sheet1.Cells[0, 4].Value = "REGION_CODE";

            ef.Save(filePath);
            var bytes = File.ReadAllBytes(filePath);
            var file = Convert.ToBase64String(bytes);
            if (File.Exists(filePath)) File.Delete(filePath);
            return file;
        }

        public static void UploadElectoralAreas(string username, UploadFile data, ApplicationDbContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var fileName = data.Filename;
                fileName = StringGenerators.GenerateRandomString(16) + fileName.Replace(" ", "").ToLower();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", fileName);

                var base64Data = data.File.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", "");
                var binData = Convert.FromBase64String(base64Data);
                using (var stream = new MemoryStream(binData))
                {
                    File.WriteAllBytes(filePath, binData);
                }
                //gembox things
                var workbook = ExcelFile.Load(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == "Main");
                if (worksheet == null) throw new Exception("Invalid Upload File");
                var uploadData = new List<UploadModel>();
                var cnt = 0;
                foreach (var row in worksheet.Rows)
                {
                    cnt++;
                    if (cnt == 1) continue;
                    if (string.IsNullOrEmpty(row.Cells[0].GetFormattedValue()))
                        throw new Exception($"Please enter a valid code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[1].GetFormattedValue()))
                        throw new Exception($"Please enter a name for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[2].GetFormattedValue()))
                        throw new Exception($"Please enter a constituency code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[3].GetFormattedValue()))
                        throw new Exception($"Please enter a district code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[4].GetFormattedValue()))
                        throw new Exception($"Please enter a region code for the record on line {cnt}");

                    var ud = new UploadModel
                    {
                        Code = row.Cells[0].GetFormattedValue(),
                        Name = row.Cells[1].GetFormattedValue(),
                        ConstituencyCode = row.Cells[2].GetFormattedValue(),
                        DistrictCode = row.Cells[3].GetFormattedValue(),
                        RegionCode = row.Cells[4].GetFormattedValue()
                    };
                    uploadData.Add(ud);
                }


                foreach (var uData in uploadData)
                {
                    var existing = db.ElectoralAreas.Where(x => (x.Code == uData.Code || uData.Name == x.Name) && x.Constituency.Code == uData.ConstituencyCode && x.Constituency.District.Code == uData.DistrictCode && x.Constituency.District.Region.Code == uData.RegionCode).Include(x => x.Constituency.District.Region).FirstOrDefault();
                    if (existing != null)
                    {
                        if (existing.Code == uData.Code) throw new Exception($"There is already an existing record with this Code - {existing.Code}, \nConstituency: {existing.Constituency?.Name} District: {existing.Constituency?.District?.Name} and \nRegion: {existing.Constituency?.District?.Region?.Name}");
                        if (existing.Name == uData.Name) throw new Exception($"There is already an existing record with this Name - {existing.Name}, \nConstituency: {existing.Constituency?.Name} District: {existing.Constituency?.District?.Name} and \nRegion: {existing.Constituency?.District?.Region?.Name}");
                    }
                    var region = db.Regions.FirstOrDefault(x => x.Code.ToLower() == uData.RegionCode.ToLower());
                    if (region == null) throw new Exception($"There is no region with this region code ({uData.RegionCode}). Please check and verify.");
                    var district = db.Districts.FirstOrDefault(x => x.Code.ToLower() == uData.DistrictCode.ToLower() && x.RegionId == region.Id);
                    if (district == null) throw new Exception($"There is no district with this district code - ({uData.DistrictCode}) and region code - ({uData.RegionCode}). Please check and verify.");
                    var consti = db.Constituencies.FirstOrDefault(x => x.Code.ToLower() == uData.ConstituencyCode.ToLower() && x.DistrictId == district.Id);
                    if (consti == null) throw new Exception($"There is no constituency with this constituency code - ({uData.ConstituencyCode}), district code - ({uData.DistrictCode}) and region code - ({uData.RegionCode}). Please check and verify.");

                    // save the data
                    var newRec = new ElectoralArea
                    {
                        Code = uData.Code,
                        Name = uData.Name,
                        ConstituencyId = consti.Id,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        CreatedBy = username,
                        ModifiedAt = DateTime.Now.ToUniversalTime(),
                        ModifiedBy = username
                    };
                    db.ElectoralAreas.Add(newRec);
                    db.SaveChanges();
                    //db.Entry<ElectoralArea>(newRec).State = EntityState.Detached;
                }

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                transaction.Commit();
            }
        }

        public static string GeneratePollingStationsUploadTemplate()
        {
            var filename = $"PollingStationsUploadTemplate_{DateTime.Now.ToUniversalTime().ToFileTime()}.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", filename);
            var ef = new ExcelFile();
            var sheet1 = ef.Worksheets.Add("Main");
            sheet1.Cells[0, 0].Value = "CODE";
            sheet1.Cells[0, 1].Value = "NAME";
            sheet1.Cells[0, 2].Value = "ELECTORAL_AREA_CODE";
            sheet1.Cells[0, 3].Value = "CONSTITUENCY_CODE";
            sheet1.Cells[0, 4].Value = "DISTRICT_CODE";
            sheet1.Cells[0, 5].Value = "REGION_CODE";

            ef.Save(filePath);
            var bytes = File.ReadAllBytes(filePath);
            var file = Convert.ToBase64String(bytes);
            if (File.Exists(filePath)) File.Delete(filePath);
            return file;
        }

        public static void UploadStations(string username, UploadFile data, ApplicationDbContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var fileName = data.Filename;
                fileName = StringGenerators.GenerateRandomString(16) + fileName.Replace(" ", "").ToLower();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "TempFiles", fileName);

                var base64Data = data.File.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", "");
                var binData = Convert.FromBase64String(base64Data);
                using (var stream = new MemoryStream(binData))
                {
                    File.WriteAllBytes(filePath, binData);
                }
                //gembox things
                var workbook = ExcelFile.Load(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == "Main");
                if (worksheet == null) throw new Exception("Invalid Upload File");
                var uploadData = new List<UploadModel>();
                var cnt = 0;
                foreach (var row in worksheet.Rows)
                {
                    cnt++;
                    if (cnt == 1) continue;
                    if (string.IsNullOrEmpty(row.Cells[0].GetFormattedValue()))
                        throw new Exception($"Please enter a valid code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[1].GetFormattedValue()))
                        throw new Exception($"Please enter a name for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[2].GetFormattedValue()))
                        throw new Exception($"Please enter the electoral area code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[3].GetFormattedValue()))
                        throw new Exception($"Please enter a constituency code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[4].GetFormattedValue()))
                        throw new Exception($"Please enter a district code for the record on line {cnt}");

                    if (string.IsNullOrEmpty(row.Cells[5].GetFormattedValue()))
                        throw new Exception($"Please enter a region code for the record on line {cnt}");

                    var ud = new UploadModel
                    {
                        Code = row.Cells[0].GetFormattedValue(),
                        Name = row.Cells[1].GetFormattedValue(),
                        ElectoralAreaCode = row.Cells[2].GetFormattedValue(),
                        ConstituencyCode = row.Cells[3].GetFormattedValue(),
                        DistrictCode = row.Cells[4].GetFormattedValue(),
                        RegionCode = row.Cells[5].GetFormattedValue()
                    };
                    uploadData.Add(ud);
                }

                foreach (var uData in uploadData)
                {
                    var existing = db.Stations.Where(x => (x.Code == uData.Code || uData.Name == x.Name) && x.ElectoralArea.Code == uData.ElectoralAreaCode  && x.ElectoralArea.Constituency.Code == uData.ConstituencyCode && x.ElectoralArea.Constituency.District.Code == uData.DistrictCode && x.ElectoralArea.Constituency.District.Region.Code == uData.RegionCode).Include(x => x.ElectoralArea.Constituency.District.Region).FirstOrDefault();
                    if (existing != null)
                    {
                        if (existing.Code == uData.Code) throw new Exception($"There is already an existing record with this Code - {existing.Code}, \nElectoral Area: {existing.ElectoralArea?.Name}, \nConstituency: {existing.ElectoralArea?.Constituency?.Name}, \nDistrict: {existing.ElectoralArea?.Constituency?.District?.Name} and \nRegion: {existing.ElectoralArea?.Constituency?.District?.Region?.Name}");
                        if (existing.Name == uData.Name) throw new Exception($"There is already an existing record with this Name - {existing.Name}, \nElectoral Area: {existing.ElectoralArea?.Name}, \nConstituency: {existing.ElectoralArea?.Constituency?.Name}, \nDistrict: {existing.ElectoralArea?.Constituency?.District?.Name} and \nRegion: {existing.ElectoralArea?.Constituency?.District?.Region?.Name}");
                    }
                    var region = db.Regions.FirstOrDefault(x => x.Code.ToLower() == uData.RegionCode.ToLower());
                    if (region == null) throw new Exception($"There is no region with this region code ({uData.RegionCode}). Please check and verify.");
                    var district = db.Districts.FirstOrDefault(x => x.Code.ToLower() == uData.DistrictCode.ToLower() && x.RegionId == region.Id);
                    if (district == null) throw new Exception($"There is no district with this district code - ({uData.DistrictCode}) and region code - ({uData.RegionCode}). Please check and verify.");
                    var consti = db.Constituencies.FirstOrDefault(x => x.Code.ToLower() == uData.ConstituencyCode.ToLower() && x.DistrictId == district.Id);
                    if (consti == null) throw new Exception($"There is no constituency with this constituency code - ({uData.ConstituencyCode}), district code - ({uData.DistrictCode}) and region code - ({uData.RegionCode}). Please check and verify.");
                    var ea = db.ElectoralAreas.FirstOrDefault(x => x.Code.ToLower() == uData.ElectoralAreaCode.ToLower() && x.ConstituencyId == consti.Id);
                    if (ea == null) throw new Exception($"There is no electoral area with this electoral area code - ({uData.ElectoralAreaCode}), constituency code - ({uData.ConstituencyCode}), district code - ({uData.DistrictCode}) and region code - ({uData.RegionCode}). Please check and verify.");

                    // save the data
                    var newRec = new Station
                    {
                        Code = uData.Code,
                        Name = uData.Name,
                        ElectoralAreaId = ea.Id,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        CreatedBy = username,
                        ModifiedAt = DateTime.Now.ToUniversalTime(),
                        ModifiedBy = username
                    };
                    db.Stations.Add(newRec);
                    db.SaveChanges();
                    //db.Entry<Station>(newRec).State = EntityState.Detached;
                }

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                transaction.Commit();
            }
        }
    }
}
