﻿using ClosedXML.Excel;
using MHRSLite_BLL.Contracts;
using MHRSLite_EL.Enums;
using MHRSLite_EL.IdentityModels;
using MHRSLite_EL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.CreateDefaultData
{
    public static class CreateData
    {
        public static void Create(UserManager<AppUser> userManager,
                                  RoleManager<AppRole> roleManager,
                                  IUnitOfWork unitOfWork,
                                  IConfiguration configuration,
                                  IWebHostEnvironment environment)
        {
            //Eklenmesini istediğimiz verileri ekleyecek metotları çağıralım.
            CheckRoles(roleManager);
            CreateCities(environment, unitOfWork);
            CreateClinics(environment, unitOfWork);
        }

        private static void CreateClinics(IWebHostEnvironment environment, IUnitOfWork unitOfWork)
        {
            try
            {
                var clinicList = unitOfWork.ClinicRepository.GetAll().ToList();
                //Provide a path for excel file
                //Excel dosyasının bulunduğu yolu aldık
                string path = Path.Combine(environment.WebRootPath, "Excels");
                string fileName = Path.GetFileName("Clinics.xlsx");
                string filePath = Path.Combine(path, fileName);
                using (var excelBook = new XLWorkbook(filePath))
                {
                    var rows = excelBook.Worksheet(1).RowsUsed();
                    foreach (var item in rows)
                    {
                        if (item.RowNumber() > 1 && item.RowNumber() <= rows.Count())
                        {

                            var cell = item.Cell(1).Value;
                            Clinic clinic = new Clinic()
                            {
                                CreatedDate = DateTime.Now,
                                ClinicName = cell.ToString()
                            };

                            if (clinicList
                                .Count(x=> x.ClinicName.ToLower() == cell.ToString().ToLower()) == 0)
                            {
                                unitOfWork.ClinicRepository.Add(clinic);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static void CheckRoles(RoleManager<AppRole> roleManager)
        {
            var allRoles = Enum.GetNames(typeof(RoleNames));
            foreach (var item in allRoles)
            {
                if (!roleManager.RoleExistsAsync(item).Result)
                {
                    var result = roleManager.CreateAsync(new AppRole()
                    {
                        Name = item,
                        Description = item
                    }).Result;
                }
            }
        }
        private static void CreateCities(IWebHostEnvironment environment, IUnitOfWork unitOfWork)
        {
            try
            {
                //Provide a path for excel file
                //Excel dosyasının bulunduğu yolu aldık
                string path = Path.Combine(environment.WebRootPath, "Excels");
                string fileName = Path.GetFileName("Cities.xlsx");
                string filePath = Path.Combine(path, fileName);
                using (var excelBook = new XLWorkbook(filePath))
                {
                    var rows = excelBook.Worksheet(1).RowsUsed();
                    foreach (var item in rows)
                    {
                        if (item.RowNumber() > 1 && item.RowNumber() <= rows.Count())
                        {
                            var cell = item.Cell(1).Value;//İstanbul
                            var plateCode = item.Cell(2).Value;
                            City city = new City()
                            {
                                CreatedDate = DateTime.Now,
                                CityName = cell.ToString(),
                                PlateCode = Convert.ToByte(plateCode)
                            };
                            unitOfWork.CityRepository.Add(city);
                            //buraya dönülecek
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}

