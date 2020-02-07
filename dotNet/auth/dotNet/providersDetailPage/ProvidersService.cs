using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Providers;
using Sabio.Models.Domain.TypeTables;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Providers;
using Sabio.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sabio.Models.Domain.InsurancePlans;
using Sabio.Models.Requests.Locations;
using Sabio.Models.Requests.Practice;
using Sabio.Models.Requests.Files;
using Sabio.Models.Requests.Affiliations;
using Sabio.Models.Requests.Licenses;
using Sabio.Models.Requests.Specializations;
using Sabio.Models.Providers;

namespace Sabio.Services
{
    public class ProvidersService : IProvidersService
    {
        IDataProvider _data = null;

        public ProvidersService(IDataProvider data)
        {
            _data = data;
        }

        public void UpdateCertifications(CertificationsUpdateRequest model, int providerId)
        {
            string proc = "[dbo].[ProviderCertifications_Update]";
                       
            _data.ExecuteNonQuery(proc, paramCol =>
            {
                DataTable dat = null;

                if (model.CertificationsList.Count > 0)
                {
                    dat = new DataTable();
                    dat.Columns.Add("CertificationsList", typeof(int));

                    foreach (int i in model.CertificationsList)
                    {
                        DataRow dr = dat.NewRow();
                        dr[0] = i;
                        dat.Rows.Add(dr);
                    }
                }
                paramCol.AddWithValue("@ProviderId", providerId);
                paramCol.AddWithValue("@CertificationsIdList", dat);
            });
        }

        public void UpdateLanguages(LanguagesUpdateRequest model, int providerId)
        {
            string proc = "[dbo].[ProviderLanguages_Update]";
          

            _data.ExecuteNonQuery(proc, paramCol =>
            {
                DataTable dat = null;
                if (model.LanguagesList.Count > 0)
                {
                    dat = new DataTable();
                    dat.Columns.Add("LanguagesList", typeof(int));

                    foreach(int i in model.LanguagesList)
                    {
                        DataRow dr = dat.NewRow();
                        dr[0] =i;
                        dat.Rows.Add(dr);
                    }

                }

                paramCol.AddWithValue("@ProviderId", providerId);
                paramCol.AddWithValue("@LanguagesIdList", dat);
            });
        }

        public void UpdateExpertise(ExpertiseUpdateRequest model, int providerId)
        {
            string proc = "dbo.ProviderExpertise_Update";
            DataTable dat = null;

            _data.ExecuteNonQuery(proc, paramCol =>
            {             

                if (model.ExpertiseList.Count > 0)
                {
                    dat = new DataTable();
                    dat.Columns.Add("ExpertiseList", typeof(int));

                    foreach (int i in model.ExpertiseList)
                    {
                        DataRow dr = dat.NewRow();
                        dr[0] = i;
                        dat.Rows.Add(dr);
                    }

                }
                paramCol.AddWithValue("@ProviderId", providerId);
                paramCol.AddWithValue("@ExpertiseIdList", dat);
            });
        }

        public void UpdateSpecialization(SpecializationsUpdateRequest model, int providerId, int userId)
        {
            string proc = "dbo.ProviderSpecialization_Update";

            _data.ExecuteNonQuery(proc, paramCol =>
            {
                DataTable dat = null;

                if (model.SpecializationsList.Count > 0)
                {
                    dat = new DataTable();
                  
                    dat.Columns.Add("SpecializationId", typeof(int));
                    dat.Columns.Add("IsPrimary", typeof(int));


                    foreach (SpecializationsBase sb in model.SpecializationsList)
                    {
                       
                        dat.Rows.Add(sb.SpecializationId, sb.IsPrimary);

                    }

                }
                paramCol.AddWithValue("@ProviderId", providerId);
                paramCol.AddWithValue("@UserId", userId);
                paramCol.AddWithValue("@SpecializationsList", dat);
            });
        }


        public void UpdateAffiliations(AffiliationUpdateRequest model, int providerId, int userId)
        {


            string proc = "dbo.ProviderAffiliations_Update";


            _data.ExecuteNonQuery(proc, paramCol =>
            {

                DataTable dat = null;

                if (model.AffiliationsList.Count > 0)
                {
                    dat = new DataTable();

                    dat.Columns.Add("Name", typeof(string));
                    dat.Columns.Add("AffiliationTypeId", typeof(int));

                    foreach (AffiliationsBase ab  in model.AffiliationsList)
                    {
                        dat.Rows.Add(ab.Name, ab.AffiliationTypeId);
                    }

                }


                paramCol.AddWithValue("@ProviderId", providerId);
                paramCol.AddWithValue("@UserId", userId);
                paramCol.AddWithValue("@AffiliationsList", dat);
            });
        }

        public void UpdateLicenses(LicensesUpdateRequest model, int providerId, int userId)
        {
            string proc = "dbo.ProviderLicenses_Update";
            DataTable dat = new DataTable();

            if (model.LicensesList.Count > 0)
            {

                dat.Columns.Add("LicenseStateId", typeof(int));
                dat.Columns.Add("LicenseNumber", typeof(string));
                dat.Columns.Add("DateExpires", typeof(DateTime));

                foreach (LicensesBase lb in model.LicensesList)
                {
                    DataRow dr = dat.NewRow();
                    dr[0] = lb;
                    dat.Rows.Add(dr);
                }


            }

            _data.ExecuteNonQuery(proc, paramCol =>
            {
                paramCol.AddWithValue("@ProviderId", providerId);
                paramCol.AddWithValue("@UserId", userId);
                paramCol.AddWithValue("@LicensesList", dat);
            });
        }

        public void DeleteById(int providerId)
        {
            string procName = "[dbo].[Providers_Delete_ById]";

            _data.ExecuteNonQuery(procName, paramCol =>
           {
               paramCol.AddWithValue("@Id", providerId);
           });
        }

        public int InsertV2(ProviderAddRequest provider, int userId)
        {
            string procName = "[dbo].[Providers_InsertV2]";
            int providerId = 0;
            string password = provider.User.Password;
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, salt);

            _data.ExecuteNonQuery(procName, paramCol =>
            {
                SqlParameter sqlParam = new SqlParameter("@ProviderId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                ModifyInsertParams(paramCol, provider);
                paramCol.AddWithValue("@HashedPassword", hashedPassword);
                paramCol.AddWithValue("@Salt", salt);
                paramCol.AddWithValue("@CreatedBy", userId);
                paramCol.Add(sqlParam);

            }, returnParams =>
            {
                Object oid = returnParams["@ProviderId"].Value;
                Int32.TryParse(oid.ToString(), out providerId);

            });

            return providerId;
        }

        public int InsertMultiple(ProvidersAddRequest2 provider, int userId)
        {
            string procName = "[dbo].[Providers_MultiPracticeInsert]";
            int providerId = 0;



            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                ProviderMultiInsertMapper(provider, userId, col);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);

                DataTable locationTable = null;
                if (provider.locations.Count > 0)
                {
                    locationTable = LocationsMultiInsertMapper();

                    foreach (var newLocation in provider.locations)
                    {
                        HydrateMultiInsertLocations
                        (locationTable, newLocation);
                    }
                }
                col.AddWithValue("@LocationList", locationTable);

                DataTable practiceTable = null;
                if (provider.practices.Count > 0)
                {
                    practiceTable = PracticesMultiInsertMapper();

                    foreach (var newPractice in provider.practices)
                    {
                        HydrateMultiInsertPractices(practiceTable, newPractice);

                    }
                }
                col.AddWithValue("@PracticeList", practiceTable);

            }, returnParameters: delegate (SqlParameterCollection paramCol)
            {
                Object oid = paramCol["@Id"].Value;
                int.TryParse(oid.ToString(), out providerId);
            });

            return providerId;
        }

        public int Add(ProviderDetailsAddRequest provider, int userId)
        {
            string procName = "[dbo].[Providers_InsertAllV2]";
            int providerId = 0;

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                SqlParameter idOut = new SqlParameter("@ProviderId", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
                col.AddWithValue("@UserId", userId);

                col.AddWithValue("@UserProfileId", provider.UserProfile.Id);
                col.AddWithValue("@FirstName", provider.UserProfile.FirstName);
                col.AddWithValue("@LastName", provider.UserProfile.LastName);
                col.AddWithValue("@Mi", provider.UserProfile.Mi);
                col.AddWithValue("@AvatarUrl", provider.UserProfile.AvatarUrl);

                col.AddWithValue("@TitleTypeId", provider.TitleTypeId);
                col.AddWithValue("@GenderTypeId", provider.GenderTypeId);
                col.AddWithValue("@Phone", provider.Phone);
                col.AddWithValue("@Fax", provider.Fax);

                col.AddWithValue("@NPI", provider.ProfessionalDetails.NPI);
                col.AddWithValue("@GenderAccepted", provider.ProfessionalDetails.GenderAccepted);

                DataTable practiceDataTable = PracticeDataTable(provider.Practices);
                col.AddWithValue("@PracticeList", practiceDataTable);

                DataTable locationDataTable = LocationDataTable(provider.Locations);
                col.AddWithValue("@LocationList", locationDataTable);

                DataTable affiliationDataTable = AffiliationDataTable(provider.Affiliations);
                col.AddWithValue("@AffiliationsList", affiliationDataTable);

                DataTable certificationsDataTable = IdsDataTable(provider.Certifications);
                col.AddWithValue("@CertificationsIdList", certificationsDataTable);

                DataTable expertiseDataTable = IdsDataTable(provider.Expertise);
                col.AddWithValue("@ExpertiseIdList", expertiseDataTable);

                DataTable languagesDataTable = IdsDataTable(provider.Languages);
                col.AddWithValue("@LanguagesIdList", languagesDataTable);

                DataTable licensesDataTable = LicensesDataTable(provider.Licenses);
                col.AddWithValue("@LicensesList", licensesDataTable);

                DataTable specializationDataTable = SpecializationDataTable(provider.Specializations);
                col.AddWithValue("@SpecializationsList", specializationDataTable);

            }, returnParameters: delegate (SqlParameterCollection paramCol)
            {
                Object oid = paramCol["@ProviderId"].Value;
                int.TryParse(oid.ToString(), out providerId);
            });

            return providerId;
        }

        private static DataTable SpecializationDataTable(List<SpecializationAddRequest> specializations)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SpecializationId", typeof(int));
            dt.Columns.Add("isPrimary", typeof(bool));

            if (specializations == null)
            {
                return dt;
            }

            for (int i = 0; i < specializations.Count; i++)
            {
                dt.Rows.Add(specializations[i].SpecializationId, specializations[i].IsPrimary);
            }

            return dt;
        }

        private static DataTable LicensesDataTable(List<BaseLicenseAddRequest> licenses)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LicenseStateId", typeof(int));
            dt.Columns.Add("LicenseNumber", typeof(string));
            dt.Columns.Add("DateExpires", typeof(DateTime));

            if (licenses == null)
            {
                return dt;
            }

            for (int i = 0; i < licenses.Count; i++)
            {
                //DateTime? dateExpires = null;
                //if(licenses[i].DateExpires != null) {
                //    dateExpires = licenses[i].DateExpires;
                //}

                dt.Rows.Add(licenses[i].LicenseStateId,
                            licenses[i].LicenseNumber,
                            licenses[i].DateExpires);
            }

            return dt;
        }

        private static DataTable IdsDataTable(List<int> ids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            if (ids == null)
            {
                return dt;
            }

            for (int i = 0; i < ids.Count; i++)
            {
                dt.Rows.Add(ids[i]);
            }

            return dt;
        }

        private static DataTable AffiliationDataTable(List<AffiliationAddRequest> affiliations)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("AffiliationTypeId", typeof(int));

            if (affiliations == null)
            {
                return dt;
            }

            for (int i = 0; i < affiliations.Count; i++)
            {
                dt.Rows.Add(affiliations[i].Name, affiliations[i].AffiliationTypeId);
            }

            return dt;
        }

        private static DataTable LocationDataTable(List<LocationAddRequest2> locations)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LocationTypeId", typeof(int));
            dt.Columns.Add("LineOne", typeof(string));
            dt.Columns.Add("LineTwo", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("Zip", typeof(string));
            dt.Columns.Add("StateId", typeof(int));
            dt.Columns.Add("Latitude", typeof(double));
            dt.Columns.Add("Longitude", typeof(double));
            dt.Columns.Add("TempId", typeof(int));

            if (locations == null)
            {
                return dt;
            }

            for (int i = 0; i < locations.Count; i++)
            {
                dt.Rows.Add(locations[i].LocationTypeId,
                            locations[i].LineOne,
                            locations[i].LineTwo,
                            locations[i].City,
                            locations[i].Zip,
                            locations[i].StateId,
                            locations[i].Latitude,
                            locations[i].Longitude,
                            locations[i].TempId);
            }

            return dt;
        }

        private static DataTable PracticeDataTable(List<PracticeTempAddRequest> practices)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TempId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Fax", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("SiteUrl", typeof(string));
            dt.Columns.Add("FacilityTypeId", typeof(int));
            dt.Columns.Add("ADA_Accessible", typeof(bool));
            dt.Columns.Add("GenderAccepted", typeof(int));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("Url", typeof(string));
            dt.Columns.Add("FileTypeId", typeof(int));

            if (practices == null)
            {
                return dt;
            }

            for (int i = 0; i < practices.Count; i++)
            {
                int? fileTypeId = null;
                if (practices[i].PrimaryImage != null)
                {
                    fileTypeId = practices[i].PrimaryImage.FileTypeId;
                }
                string url = practices[i].PrimaryImage == null ? null : practices[i].PrimaryImage.Url;
                string fileName = practices[i].PrimaryImage == null ? null : practices[i].PrimaryImage.FileName;

                dt.Rows.Add(practices[i].TempId,
                            practices[i].Name,
                            practices[i].Phone,
                            practices[i].Fax,
                            practices[i].Email,
                            practices[i].SiteUrl,
                            practices[i].FacilityTypeId,
                            practices[i].IsAdaAccessible,
                            practices[i].GenderAccepted,
                            fileName,
                            url,
                            fileTypeId);
            }

            return dt;
        }

        private static DataRow HydrateMultiInsertPractices(DataTable practiceTable, PracticeAddRequest2 newPractice)
        {
            return practiceTable.Rows.Add(newPractice.TempId, newPractice.Name, newPractice.Phone, newPractice.Fax, newPractice.Email,
                                        newPractice.SiteUrl, newPractice.FacilityTypeId, newPractice.ScheduleId, newPractice.IsAdaAccessible, newPractice.GenderAccepted, newPractice.PrimaryImageId);

        }

        private static DataRow HydrateMultiInsertLocations(DataTable locationTable, LocationAddRequest2 newLocation)
        {
            return locationTable.Rows.Add(newLocation.LocationTypeId, newLocation.LineOne, newLocation.LineTwo, newLocation.City, newLocation.Zip,
                                        newLocation.StateId, newLocation.Latitude, newLocation.Longitude, newLocation.TempId);
        }

        private static DataTable PracticesMultiInsertMapper()
        {
            DataTable practiceTable = new DataTable();
            practiceTable.Columns.Add("TempId", typeof(int));
            practiceTable.Columns.Add("Name", typeof(string));
            practiceTable.Columns.Add("Phone", typeof(string));
            practiceTable.Columns.Add("Fax", typeof(string));
            practiceTable.Columns.Add("Email", typeof(string));
            practiceTable.Columns.Add("SiteUrl", typeof(string));
            practiceTable.Columns.Add("FacilityTypeId", typeof(int));
            practiceTable.Columns.Add("ScheduleId", typeof(int));
            practiceTable.Columns.Add("ADA_Accessible", typeof(bool));
            practiceTable.Columns.Add("GenderAccepted", typeof(int));
            practiceTable.Columns.Add("PrimaryImageId", typeof(string));
            return practiceTable;
        }

        private static DataTable LocationsMultiInsertMapper()
        {
            DataTable locationTable = new DataTable();
            locationTable.Columns.Add("LocationTypeId", typeof(int));
            locationTable.Columns.Add("LineOne", typeof(string));
            locationTable.Columns.Add("LineTwo", typeof(string));
            locationTable.Columns.Add("City", typeof(string));
            locationTable.Columns.Add("Zip", typeof(string));
            locationTable.Columns.Add("StateId", typeof(int));
            locationTable.Columns.Add("Latitude", typeof(double));
            locationTable.Columns.Add("Longitude", typeof(double));
            locationTable.Columns.Add("TempId", typeof(int));
            return locationTable;
        }

        private static void ProviderMultiInsertMapper(ProvidersAddRequest2 provider, int userId, SqlParameterCollection col)
        {
            col.AddWithValue("@TitleTypeId", provider.TitleTypeId);
            col.AddWithValue("@UserProfileId", provider.UserProfileId);
            col.AddWithValue("@GenderTypeId", provider.GenderTypeId);
            col.AddWithValue("@Phone", provider.Phone);
            col.AddWithValue("@Fax", provider.Fax);
            col.AddWithValue("@CreatedBy", userId);
        }

        public MemoryStream ReportSelectAll(ProviderDetailCategories categories, int userId)
        {

            string procName;
            if (userId == 0)
            {
                procName = "[dbo].[ProvidersReport_SelectAll_Details]";
            }
            else
            {
                procName = "[dbo].[ProvidersReport_SelectAll_DetailsV2]";

            }

            List<ProviderReport> list = null;

            _data.ExecuteCmd(procName, paramCol =>
            {
                if (userId == 0)
                {
                    ModifyReportParams(paramCol, categories);
                }
                else
                {
                    ModifyReportParamsV2(paramCol, categories, userId);
                }
            }, (reader, set) =>
            {

                ProviderReport provider = HydrateProvidersReport(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

            });


            //this dict will keep track of the longest count across each category
            Dictionary<string, int> countDict = new Dictionary<string, int>();

            bool catIsSelected;

            //filter categories by truthy, and add an entries to the dictionary "countDict" 
            PropertyInfo[] truthyCategories = categories.GetType()
                .GetProperties()
                .Where(prop =>
                {
                    bool.TryParse(prop.GetValue(categories).ToString(), out catIsSelected);
                    return catIsSelected;
                }).ToArray();


            foreach (PropertyInfo propertyInfo in truthyCategories)
            {
                countDict.Add(propertyInfo.Name, 0);
            }


            //get all properties of type List that are truthy within categories, and store them in arrayProperties for future access
            Type type = typeof(ProviderReport);
            PropertyInfo[] allProperties = type.GetProperties();

            PropertyInfo[] arrayProperties = allProperties
                .Where(property => property.PropertyType.IsGenericType &&
                    countDict.ContainsKey(property.Name)).ToArray();


            //iterate over each provider and update counts in dict
            foreach (ProviderReport providerReport in list)
            {
                foreach (PropertyInfo property in arrayProperties)
                {
                    //nullcheck current prop
                    ICollection propCollection = GetPropValue<ICollection>(providerReport, property.Name);
                    if (propCollection != null)
                    {
                        //compare this particular instanced count to the count in the dict
                        if (countDict[property.Name] < propCollection.Count)
                        {
                            countDict[property.Name] = propCollection.Count;
                        }

                    }
                }
            }

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {

                #region concatenated details worksheet
                //create new worksheet with base info
                ExcelWorksheet concatenated = package.Workbook.Worksheets.Add("Concatenated Details");

                ModifyBaseExcelHeaders(concatenated, out int concatenatedHeaderIndex);

                if (categories.Professional)
                {
                    concatenated.Cells[1, concatenatedHeaderIndex++].Value = "NPI";
                    concatenated.Cells[1, concatenatedHeaderIndex++].Value = "Genders Accepted";
                }


                //sort arrayProperties to ensure consistent excel column ordering between worksheets
                PropertyInfo[] sortedArrayProperties = arrayProperties.OrderBy(p => p.Name).ToArray();

                //add cell to worksheet with value of property name
                for (int i = 0; i < sortedArrayProperties.Length; i++)
                {
                    concatenated.Cells[1, concatenatedHeaderIndex++].Value = sortedArrayProperties[i].Name;
                }

                //call this before filling concatenated w/s         
                using (var r = concatenated.Cells[1, 1, 1, concatenatedHeaderIndex - 1])
                {
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }


                //fill w/s with data
                int concatenatedRowIndex = 2;
                foreach (ProviderReport providerReport in list)
                {

                    PopulateBaseExcelCells(concatenated, providerReport, concatenatedRowIndex, out int concatenatedColIndex);

                    if (categories.Professional)
                    {
                        concatenated.Cells[concatenatedRowIndex, concatenatedColIndex].Value
                        = providerReport.Professional?.NPI;
                        concatenatedColIndex++;

                        if (providerReport.Professional?.GendersAccepted != null)
                        {
                            concatenated.Cells[concatenatedRowIndex, concatenatedColIndex].Value
                            = providerReport.Professional.GendersAccepted;
                        }
                        concatenatedColIndex++;
                    }

                    for (int i = 0; i < sortedArrayProperties.Length; i++)
                    {
                        ICollection propCollection = GetPropValue<ICollection>(providerReport, sortedArrayProperties[i].Name);

                        if (propCollection != null)
                        {
                            string concatProp = "";
                            foreach (var prop in propCollection)
                            {
                                if (concatProp != "")
                                {
                                    concatProp += $"; ";
                                }
                                string propName = "";

                                if (sortedArrayProperties[i].Name == "Licenses")
                                {
                                    propName = GetPropValue<string>(prop, "State");
                                }
                                else
                                {
                                    propName = GetPropValue<string>(prop, "Name");
                                }

                                concatProp += $"{propName}";
                            }
                            concatenated.Cells[concatenatedRowIndex, concatenatedColIndex++].Value
                            = concatProp;
                        }
                        else
                        {
                            concatenatedColIndex++;
                        }
                    }
                    concatenatedRowIndex++;
                }

                concatenated.Cells.AutoFitColumns(0, 30);


                #endregion


                #region expanded details worksheet
                ExcelWorksheet expanded = package.Workbook.Worksheets.Add("Expanded Details");

                ModifyBaseExcelHeaders(expanded, out int expandedHeaderIndex);

                if (categories.Professional)
                {
                    expanded.Cells[1, expandedHeaderIndex++].Value = "NPI";
                    expanded.Column(expandedHeaderIndex).Style.Numberformat.Format = "#";
                    expanded.Cells[1, expandedHeaderIndex++].Value = "Genders Accepted";
                }

                for (int i = 0; i < sortedArrayProperties.Length; i++)
                {
                    for (int j = 0; j < countDict[sortedArrayProperties[i].Name]; j++)
                    {
                        string truncatedName = sortedArrayProperties[i].Name.EndsWith("s")
                        ? sortedArrayProperties[i].Name.Substring(0, sortedArrayProperties[i].Name.Length - 1)
                        : sortedArrayProperties[i].Name;
                        expanded.Cells[1, expandedHeaderIndex++].Value = $"{truncatedName}{j + 1}";
                    }

                }


                using (var r = expanded.Cells[1, 1, 1, expandedHeaderIndex - 1])
                {
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }


                //fill w/s with data
                int expandedRowIndex = 2;
                foreach (ProviderReport providerReport in list)
                {
                    PopulateBaseExcelCells(expanded, providerReport, expandedRowIndex, out int expandedColIndex);

                    if (categories.Professional)
                    {

                        expanded.Cells[concatenatedRowIndex, expandedColIndex].Value
                        = providerReport.Professional?.NPI;

                        expandedColIndex++;

                        if (providerReport.Professional?.GendersAccepted != null)
                        {
                            expanded.Cells[concatenatedRowIndex, expandedColIndex].Value
                            = providerReport.Professional.GendersAccepted;
                        }
                        expandedColIndex++;
                    }

                    for (int i = 0; i < sortedArrayProperties.Length; i++)
                    {
                        ICollection propCollection = GetPropValue<ICollection>(providerReport, sortedArrayProperties[i].Name);
                        int dictCount = countDict[sortedArrayProperties[i].Name];

                        if (propCollection != null)
                        {
                            foreach (var prop in propCollection)
                            {
                                string propName = "";

                                if (sortedArrayProperties[i].Name == "Licenses")
                                {
                                    propName = GetPropValue<string>(prop, "State");
                                }
                                else
                                {
                                    propName = GetPropValue<string>(prop, "Name");
                                }

                                expanded.Cells[expandedRowIndex, expandedColIndex++].Value = propName;
                            }
                            int propCount = propCollection.Count;

                            if (propCount < dictCount)
                            {
                                expandedColIndex += dictCount - propCount;
                            }
                        }
                        else
                        {
                            expandedColIndex += dictCount;
                        }
                    }
                    expandedRowIndex++;
                }

                expanded.Cells.AutoFitColumns(0, 40);

                #endregion


                #region individual detail worksheets

                Dictionary<string, ExcelWorksheet> worksheets = new Dictionary<string, ExcelWorksheet>();

                //creates appropriate worksheet detail names
                foreach (PropertyInfo propertyInfo in truthyCategories)
                {
                    string friendlyWsName = propertyInfo.Name.EndsWith("s")
                    ? propertyInfo.Name.Substring(0, propertyInfo.Name.Length - 1)
                    : propertyInfo.Name;
                    worksheets.Add(propertyInfo.Name, package.Workbook.Worksheets.Add(friendlyWsName + " Details"));
                }


                //populate all of the worksheets with shared headers
                foreach (var kvp in worksheets)
                {
                    //ExcelWorksheet sharedWs = GetPropValue<ExcelWorksheet>(worksheets, key);
                    ExcelWorksheet sharedWs = kvp.Value;

                    int sharedHeaderIndex = 1;
                    sharedWs.Cells[1, sharedHeaderIndex++].Value = "ID";
                    sharedWs.Cells[1, sharedHeaderIndex++].Value = "First Name";
                    sharedWs.Cells[1, sharedHeaderIndex++].Value = "Last Name";

                    if (kvp.Key == "Professional")
                    {
                        sharedWs.Cells[1, sharedHeaderIndex++].Value = "NPI";
                        sharedWs.Cells[1, sharedHeaderIndex++].Value = "Genders Accepted";

                    }
                    else
                    {
                        string truncatedName = kvp.Key.EndsWith("s")
                       ? kvp.Key.Substring(0, kvp.Key.Length - 1)
                       : kvp.Key;
                        sharedWs.Cells[1, sharedHeaderIndex++].Value = $"{truncatedName}";
                    }


                    using (var r = sharedWs.Cells[1, 1, 1, sharedHeaderIndex - 1])
                    {
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    //add data to w/s
                    int sharedWsRowIndex = 2;
                    foreach (ProviderReport providerReport in list)
                    {
                        if (kvp.Key == "Professional")
                        {
                            PopulateSharedBaseExcelCells(sharedWs, providerReport, sharedWsRowIndex, out int sharedColIndex);


                            sharedWs.Cells[sharedWsRowIndex, sharedColIndex].Value = providerReport.Professional?.NPI;
                            sharedColIndex++;

                            if (providerReport.Professional?.GendersAccepted != null)
                            {
                                sharedWs.Cells[sharedWsRowIndex, sharedColIndex].Value = providerReport.Professional.GendersAccepted;
                            }
                            sharedColIndex++;

                            sharedWsRowIndex++;
                        }
                        else
                        {
                            IList prop = GetPropValue<IList>(providerReport, kvp.Key);
                            if (prop != null)
                            {
                                for (int i = 0; i < prop.Count; i++)
                                {

                                    PopulateSharedBaseExcelCells(sharedWs, providerReport, sharedWsRowIndex, out int sharedColIndex);

                                    if (kvp.Key == "Licenses")
                                    {
                                        sharedWs.Cells[sharedWsRowIndex, sharedColIndex++].Value = GetPropValue<string>(prop[i], "State");

                                    }
                                    else
                                    {
                                        sharedWs.Cells[sharedWsRowIndex, sharedColIndex++].Value = GetPropValue<string>(prop[i], "Name");

                                        if (kvp.Key == "Specializations")
                                        {
                                            bool isPrimarySpec = GetStructValue<bool>(prop[i], "IsPrimary");

                                            if (isPrimarySpec)
                                            {
                                                sharedWs.Cells[sharedWsRowIndex, sharedColIndex - 1].Style.Font.Bold = true;
                                            }
                                        }
                                    }
                                    sharedWsRowIndex++;
                                }
                            }
                        }
                    }
                    sharedWs.Cells.AutoFitColumns(0);
                }

                #endregion


                // set some document properties
                package.Workbook.Properties.Title = "Providers Report";
                package.Workbook.Properties.Author = "Scrubs Data";

                // save new workbook in the output directory 
                package.Save();

            }
            return stream;

        }

        public List<ProviderReport> ReportSelectAllPdf(ProviderDetailCategories categories, int id)
        {
            string procName;
            if (id != 0)
            {
                procName = "[dbo].[ProvidersReport_SelectAll_DetailsV2]";
            }
            else
            {
                procName = "[dbo].[ProvidersReport_SelectAll_Details]";
            }
            List<ProviderReport> list = null;
            _data.ExecuteCmd(procName, paramCol =>
            {
                if (id != 0)
                {
                    ModifyReportParamsV2(paramCol, categories, id);
                }
                else
                {
                    ModifyReportParams(paramCol, categories);
                }
            }, (reader, set) =>
            {
                ProviderReport provider = HydrateProvidersReport(reader, out int lastIndex);
                if (list == null)
                {
                    list = new List<ProviderReport>();
                }
                list.Add(provider);
            });
            return list;
        }

        public Paged<ProviderReport> ReportSelectAllPaged(int pageIndex, int pageSize, ProviderDetailCategories categories)
        {
            string procName = "[dbo].[ProvidersReport_Select_DetailsV2]";
            List<ProviderReport> list = null;
            Paged<ProviderReport> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                ModifyReportParamsV3(paramCol, categories, pageIndex, pageSize);
            }, (reader, set) =>
            {

                ProviderReport provider = HydrateProvidersReport(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderReport>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<ProviderReport> ReportSelectPaged(int pageIndex, int pageSize, int userId, ProviderDetailCategories categories)
        {
            string procName = "[dbo].[ProvidersReport_Select_DetailsV3]";
            List<ProviderReport> list = null;
            Paged<ProviderReport> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                ModifyReportParamsV4(paramCol, categories, userId, pageIndex, pageSize);
            }, (reader, set) =>
            {

                ProviderReport provider = HydrateProvidersReport(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderReport>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<ProviderReport> SearchReportSelectAllPaged(int pageIndex, int pageSize, ProviderDetailCategories categories, string query)
        {
            string procName = "[dbo].[ProvidersReport_Search_DetailsV2]";
            List<ProviderReport> list = null;
            Paged<ProviderReport> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                ModifySearchAllReportParams(paramCol, categories, query, pageIndex, pageSize);
            }, (reader, set) =>
            {

                ProviderReport provider = HydrateProvidersReport(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderReport>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<ProviderReport> SearchReportSelectPaged(int pageIndex, int pageSize, ProviderDetailCategories categories, string query, int userId)
        {
            string procName = "[dbo].[ProvidersReport_Search_DetailsV3]";
            List<ProviderReport> list = null;
            Paged<ProviderReport> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                ModifySearchReportParams(paramCol, categories, userId, query, pageIndex, pageSize);
            }, (reader, set) =>
            {

                ProviderReport provider = HydrateProvidersReport(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderReport>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<Provider> Search(string q, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Search]";

            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@query", q);
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
            }, (reader, set) =>
            {
                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

                list.Add(provider);

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<ProviderReport> SearchProviderList(string q, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_SearchV2]";

            List<ProviderReport> list = null;
            Paged<ProviderReport> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@query", q);
                paramCollection.AddWithValue("@pageIndex", pageIndex);
                paramCollection.AddWithValue("@pageSize", pageSize);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                ProviderReport provider = null;
                provider = MapProviderList(reader, out int index);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index);
                }

                list.Add(provider);
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderReport>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;
        }

        public Paged<ProviderBase> SelectAll(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_SelectAll]";
            List<ProviderBase> list = null;
            Paged<ProviderBase> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
            }, (reader, set) =>
            {

                ProviderBase provider = HydrateProvider<ProviderBase>(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderBase>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderBase>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<Provider> SelectAllDetails(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_SelectAllDetails]";
            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
            }, (reader, set) =>
            {

                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;
        }

        public Paged<ProviderNonCompliant> SelectAllNonCompliant(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_SelectAll_NonCompliantV3]";
            List<ProviderNonCompliant> list = null;
            Paged<ProviderNonCompliant> PagedList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
            }, (reader, set) =>
            {
                int index = 0;

                ProviderNonCompliant providerNonCompliant = HydrateNonCompliantDetails(reader, out index);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index++);
                }
                if (list == null)
                {
                    list = new List<ProviderNonCompliant>();
                }

                list.Add(providerNonCompliant);
            });

            if (list != null)
            {
                PagedList = new Paged<ProviderNonCompliant>(list, pageIndex, pageSize, totalCount);
            }

            return PagedList;
        }

        public Paged<ProviderNonCompliant> SelectNonCompliant(int pageIndex, int pageSize, int userId)
        {
            string procName = "[dbo].[Providers_SelectAll_NonCompliantV5]";
            List<ProviderNonCompliant> list = null;
            Paged<ProviderNonCompliant> PagedList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@userId", userId);
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
            }, (reader, set) =>
            {
                int index = 0;

                ProviderNonCompliant providerNonCompliant = HydrateNonCompliantDetails(reader, out index);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index++);
                }
                if (list == null)
                {
                    list = new List<ProviderNonCompliant>();
                }

                list.Add(providerNonCompliant);
            });

            if (list != null)
            {
                PagedList = new Paged<ProviderNonCompliant>(list, pageIndex, pageSize, totalCount);
            }

            return PagedList;
        }

        public Paged<Provider> SelectByCreatedBy(int createdById, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Select_ByCreatedBy]";

            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
                paramCol.AddWithValue("@creatorId", createdById);
            }, (reader, set) =>
            {
                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<Provider> SelectByExpertise(int expertiseId, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Select_byExpertise]";

            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
                paramCol.AddWithValue("@expertiseId", expertiseId);
            }, (reader, set) =>
            {
                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<ProviderReport> SelectProviderList(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_SelectAllDetailsV3]";
            Paged<ProviderReport> pagedResults = null;
            List<ProviderReport> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@pageIndex", pageIndex);
                paramCollection.AddWithValue("@pageSize", pageSize);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                ProviderReport provider = MapProviderList(reader, out int index);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index);
                }

            });

            if (list != null)
            {
                pagedResults = new Paged<ProviderReport>(list, pageSize, pageIndex, totalCount);
            }

            return pagedResults;
        }

        public List<ProviderReport> SelectAllProviderList(int userId)
        {
            string procName = "[dbo].[Providers_SelectList_Details]";
            List<ProviderReport> list = null;


            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserId", userId);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                ProviderReport provider = MapProviderList(reader, out int index);

                if (list == null)
                {
                    list = new List<ProviderReport>();
                }

                list.Add(provider);

            });

            return list;
        }

        public int SelectProviderByUserId(int Id)
        {
            string procName = "[dbo].[Provider_Select_ByUserProfileId]";
            int providerId = 0;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", Id);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                providerId = reader.GetSafeInt32(startingIndex++);
            });
            return providerId;
        }

        public ProviderBase SelectById(int id)
        {
            string procName = "[dbo].[Providers_Select_ById]";
            ProviderBase provider = null;
            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@Id", id);
            }, (reader, set) =>
            {

                provider = HydrateProvider<ProviderBase>(reader, out int index);
            }
            );

            return provider;
        }

        public Paged<SysProvider> SelectSysProvider(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_SelectAllDetails_V2]";
            Paged<SysProvider> pagedResults = null;
            List<SysProvider> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@pageIndex", pageIndex);
                col.AddWithValue("@pageSize", pageSize);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                SysProvider aProvider = new SysProvider();
                int startingIndex = 0;

                aProvider.Id = reader.GetSafeInt32(startingIndex++);
                aProvider.Title = reader.GetSafeString(startingIndex++);
                aProvider.FirstName = reader.GetSafeString(startingIndex++);
                aProvider.LastName = reader.GetSafeString(startingIndex++);
                aProvider.Gender = reader.GetSafeString(startingIndex++);
                aProvider.Email = reader.GetSafeString(startingIndex++);

                if (list == null)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                    list = new List<SysProvider>();
                }

                list.Add(aProvider);
            });
            if (list != null)
            {
                pagedResults = new Paged<SysProvider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedResults;
        }

        public Paged<Provider> SelectByInsurancePlan(int insurancePlanId, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Select_byInsurancePlan]";

            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
                paramCol.AddWithValue("@insurancePlanId", insurancePlanId);
            }, (reader, set) =>
            {

                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<Provider> SelectBySpecialization(int specializationId, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Select_bySpecialization]";

            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
                paramCol.AddWithValue("@specializationId", specializationId);
            }, (reader, set) =>
            {
                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;

        }

        public Paged<Provider> SelectByState(int stateId, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Select_byState]";

            List<Provider> list = null;
            Paged<Provider> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@pageIndex", pageIndex);
                paramCol.AddWithValue("@pageSize", pageSize);
                paramCol.AddWithValue("@stateId", stateId);
            }, (reader, set) =>
            {
                Provider provider = HydrateProviderDetails(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<Provider>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }

            });

            if (list != null)
            {
                pagedItems = new Paged<Provider>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;
        }

        public Provider SelectDetailsById(int id)
        {
            string procName = "[dbo].[Providers_SelectDetails_ById]";
            Provider provider = null;
            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@Id", id);
            }, (reader, set) =>
            {
                provider = new Provider();
                int index = 0;

                provider.Id = reader.GetSafeInt32(index++);
                provider.LastAttested = reader.GetSafeInt32(index++);
                provider.Phone = reader.GetSafeString(index++);
                provider.Fax = reader.GetSafeString(index++);
                provider.Gender = reader.GetSafeJSON<TypeTableBase>(index++);
                provider.Title = reader.GetSafeJSON<TypeTableBase>(index++);           
                provider.UserProfile = reader.GetSafeJSON<UserProfile>(index++);
                provider.ProfessionalDetails = reader.GetSafeJSON<ProfessionalDetails>(index++);
                provider.Practices = reader.GetSafeJSON<List<Practice>>(index++);
                provider.Affiliations = reader.GetSafeJSON<List<Affiliation>>(index++);
                provider.Certifications = reader.GetSafeJSON<List<Certification>>(index++);
                provider.Expertise = reader.GetSafeJSON<List<Expertise>>(index++);
                provider.Languages = reader.GetSafeJSON<List<Language>>(index++);
                provider.Licenses = reader.GetSafeJSON<List<ProviderLicense>>(index++);
                provider.Specializations = reader.GetSafeJSON<List<Specialization>>(index++);
                provider.InsurancePlans = reader.GetSafeJSON<List<InsurancePlan>>(index++);
                provider.CreatedBy = reader.GetSafeInt32(index++);
                provider.ModifiedBy = reader.GetSafeInt32(index++);
                provider.DateCreated = reader.GetSafeDateTime(index++);
                provider.DateModified = reader.GetSafeDateTime(index++);

            });
            return provider;
        }

        public Provider SelectDetailsById(int id, int userId)
        {
            string procName = "[dbo].[Providers_SelectDetails_ById_WithCheckV2]";
            Provider provider = null;
            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@Id", id);
                paramCol.AddWithValue("@userId", userId);
            }, (reader, set) =>
            {
                provider = new Provider();
                int index = 0;

                provider.Id = reader.GetSafeInt32(index++);
                provider.LastAttested = reader.GetSafeInt32(index++);
                provider.Phone = reader.GetSafeString(index++);
                provider.Fax = reader.GetSafeString(index++);
                provider.Gender = reader.GetSafeJSON<TypeTableBase>(index++);
                provider.Title = reader.GetSafeJSON<TypeTableBase>(index++);
                provider.UserProfile = reader.GetSafeJSON<UserProfile>(index++);
                provider.ProfessionalDetails = reader.GetSafeJSON<ProfessionalDetails>(index++);
                provider.Practices = reader.GetSafeJSON<List<Practice>>(index++);
                provider.Affiliations = reader.GetSafeJSON<List<Affiliation>>(index++);
                provider.Certifications = reader.GetSafeJSON<List<Certification>>(index++);
                provider.Expertise = reader.GetSafeJSON<List<Expertise>>(index++);
                provider.Languages = reader.GetSafeJSON<List<Language>>(index++);
                provider.Licenses = reader.GetSafeJSON<List<ProviderLicense>>(index++);
                provider.Specializations = reader.GetSafeJSON<List<Specialization>>(index++);
                provider.InsurancePlans = reader.GetSafeJSON<List<InsurancePlan>>(index++);
                provider.CreatedBy = reader.GetSafeInt32(index++);
                provider.ModifiedBy = reader.GetSafeInt32(index++);
                provider.DateCreated = reader.GetSafeDateTime(index++);
                provider.DateModified = reader.GetSafeDateTime(index++);

            });
            return provider;
        }

        public void UpdateV2(ProviderUpdateRequest provider, int userId)
        {
            string procName = "[dbo].[Providers_Update_V2]";

            _data.ExecuteNonQuery(procName, paramCol =>
            {

                ModifyUpdateParams(paramCol, provider);
                paramCol.AddWithValue("@ModifiedBy", userId);
            });
        }

        public void UpdateLastAttested(int id) {
            string procName = "dbo.Providers_UpdateDateAttested";
            _data.ExecuteNonQuery(procName, paramCol =>
            {
                paramCol.AddWithValue("@id", id);
            });
        }
        
        private static T HydrateProvider<T>(IDataReader reader, out int index) where T : ProviderBase, new()
        {
            T provider = new T();
            index = 0;
            provider.Id = reader.GetSafeInt32(index++);
            provider.TitleTypeId = reader.GetSafeInt32(index++);
            provider.UserProfileId = reader.GetSafeInt32(index++);
            provider.GenderTypeId = reader.GetSafeInt32(index++);
            provider.Phone = reader.GetSafeString(index++);
            provider.Fax = reader.GetSafeString(index++);
            provider.CreatedBy = reader.GetSafeInt32(index++);
            provider.ModifiedBy = reader.GetSafeInt32(index++);
            provider.DateCreated = reader.GetSafeDateTime(index++);
            provider.DateModified = reader.GetSafeDateTime(index++);

            //if any provider procs are modified, the index might not like that

            return provider;

        }

        private static ProviderReport MapProviderList(IDataReader reader, out int index)
        {
            ProviderReport providerList = new ProviderReport();
            index = 0;
            providerList.Id = reader.GetSafeInt32(index++);
            providerList.Title = reader.GetSafeString(index++);
            providerList.FirstName = reader.GetSafeString(index++);
            providerList.Mi = reader.GetSafeString(index++);
            providerList.LastName = reader.GetSafeString(index++);
            providerList.Gender = reader.GetSafeString(index++);
            providerList.Phone = reader.GetSafeString(index++);
            providerList.Fax = reader.GetSafeString(index++);
            providerList.Email = reader.GetSafeString(index++);

            return providerList;
        }

        private static ProviderReport HydrateProvidersReport(IDataReader reader, out int index)
        {
            ProviderReport providerReport = new ProviderReport();
            index = 0;
            providerReport.Id = reader.GetSafeInt32(index++);
            providerReport.Title = reader.GetSafeString(index++);
            providerReport.FirstName = reader.GetSafeString(index++);
            providerReport.Mi = reader.GetSafeString(index++);
            providerReport.LastName = reader.GetSafeString(index++);
            providerReport.Gender = reader.GetSafeString(index++);
            providerReport.Phone = reader.GetSafeString(index++);
            providerReport.Fax = reader.GetSafeString(index++);
            providerReport.Email = reader.GetSafeString(index++);
            providerReport.UserId = reader.GetSafeInt32(index++);
            providerReport.DateAttested = reader.GetSafeDateTime(index++);
            providerReport.Compliant = reader.GetSafeInt32(index++);
            //refactor this code with hydrateproviderdetails after hector gives guidance
            providerReport.Professional = reader.GetSafeJSON<ProfessionalDetailsReport>(index++);
            providerReport.Practices = reader.GetSafeJSON<List<PracticeReport>>(index++);
            providerReport.Affiliations = reader.GetSafeJSON<List<AffiliationReport>>(index++);
            providerReport.Certifications = reader.GetSafeJSON<List<TypeTableBase>>(index++);
            providerReport.Expertise = reader.GetSafeJSON<List<TypeTableBase>>(index++);
            providerReport.Languages = reader.GetSafeJSON<List<TypeTableBase>>(index++);
            providerReport.Licenses = reader.GetSafeJSON<List<ProviderLicenseReport>>(index++);
            providerReport.Specializations = reader.GetSafeJSON<List<SpecializationReport>>(index++);

            return providerReport;
        }

        private static Provider HydrateProviderDetails(IDataReader reader, out int detailsIndex)
        {

            Provider provider = HydrateProvider<Provider>(reader, out int index);
            detailsIndex = index;
            provider.UserProfile = reader.GetSafeJSON<UserProfile>(detailsIndex++);
            provider.ProfessionalDetails = reader.GetSafeJSON<ProfessionalDetails>(detailsIndex++);
            provider.Practices = reader.GetSafeJSON<List<Practice>>(detailsIndex++);
            provider.Affiliations = reader.GetSafeJSON<List<Affiliation>>(detailsIndex++);
            provider.Certifications = reader.GetSafeJSON<List<Certification>>(detailsIndex++);
            provider.Expertise = reader.GetSafeJSON<List<Expertise>>(detailsIndex++);
            provider.Languages = reader.GetSafeJSON<List<Language>>(detailsIndex++);
            provider.Licenses = reader.GetSafeJSON<List<ProviderLicense>>(detailsIndex++);
            provider.Specializations = reader.GetSafeJSON<List<Specialization>>(detailsIndex++);
            return provider;
        }

        private static void ModifyInsertParams(SqlParameterCollection paramCol, ProviderAddRequest provider)
        {
            paramCol.AddWithValue("@TitleTypeId", provider.TitleTypeId);
            paramCol.AddWithValue("@GenderTypeId", provider.GenderTypeId);
            paramCol.AddWithValue("@Phone", provider.Phone);
            paramCol.AddWithValue("@Fax", provider.Fax);
            paramCol.AddWithValue("@FirstName", provider.UserProfile.FirstName);
            paramCol.AddWithValue("@LastName", provider.UserProfile.LastName);
            paramCol.AddWithValue("@Mi", provider.UserProfile.Mi);
            paramCol.AddWithValue("@AvatarUrl", provider.UserProfile.AvatarUrl);
            paramCol.AddWithValue("@NPI", provider.ProfessionalDetails.NPI);
            paramCol.AddWithValue("@GenderAccepted", provider.ProfessionalDetails.GenderAccepted);
            paramCol.AddWithValue("@Email", provider.User.Email);

        }

        private static void ModifyUpdateParams(SqlParameterCollection paramCol, ProviderUpdateRequest provider)
        {
            paramCol.AddWithValue("@ProviderId", provider.Id);
            paramCol.AddWithValue("@UserProfileId", provider.UserProfile.Id);
            paramCol.AddWithValue("@UserId", provider.UserProfile.UserId);
            paramCol.AddWithValue("@ProfessionalDetailsId", provider.ProfessionalDetails.Id);
            paramCol.AddWithValue("@TitleTypeId", provider.TitleTypeId);
            paramCol.AddWithValue("@GenderTypeId", provider.GenderTypeId);
            paramCol.AddWithValue("@Phone", provider.Phone);
            paramCol.AddWithValue("@Fax", provider.Fax);
            paramCol.AddWithValue("@FirstName", provider.UserProfile.FirstName);
            paramCol.AddWithValue("@LastName", provider.UserProfile.LastName);
            paramCol.AddWithValue("@Mi", provider.UserProfile.Mi);
            paramCol.AddWithValue("@AvatarUrl", provider.UserProfile.AvatarUrl);
            paramCol.AddWithValue("@NPI", provider.ProfessionalDetails.NPI);
            paramCol.AddWithValue("@GenderAccepted", provider.ProfessionalDetails.GenderAccepted);
        }

        private static void ModifyReportParams(SqlParameterCollection paramCol, ProviderDetailCategories categories)
        {
            paramCol.AddWithValue("@affiliations", categories.Affiliations);
            paramCol.AddWithValue("@certifications", categories.Certifications);
            paramCol.AddWithValue("@expertise", categories.Expertise);
            paramCol.AddWithValue("@languages", categories.Languages);
            paramCol.AddWithValue("@licenses", categories.Licenses);
            paramCol.AddWithValue("@practices", categories.Practices);
            paramCol.AddWithValue("@professionalDetails", categories.Professional);
            paramCol.AddWithValue("@specializations", categories.Specializations);
        }

        private static void ModifyReportParamsV2(SqlParameterCollection paramCol, ProviderDetailCategories categories, int id)
        {
            paramCol.AddWithValue("@UserId", id);
            paramCol.AddWithValue("@affiliations", categories.Affiliations);
            paramCol.AddWithValue("@certifications", categories.Certifications);
            paramCol.AddWithValue("@expertise", categories.Expertise);
            paramCol.AddWithValue("@languages", categories.Languages);
            paramCol.AddWithValue("@licenses", categories.Licenses);
            paramCol.AddWithValue("@practices", categories.Practices);
            paramCol.AddWithValue("@professionalDetails", categories.Professional);
            paramCol.AddWithValue("@specializations", categories.Specializations);
        }

        private static void ModifyReportParamsV3(SqlParameterCollection paramCol, ProviderDetailCategories categories, int pageIndex, int pageSize)
        {

            paramCol.AddWithValue("@pageIndex", pageIndex);
            paramCol.AddWithValue("@pageSize", pageSize);
            paramCol.AddWithValue("@affiliations", categories.Affiliations);
            paramCol.AddWithValue("@certifications", categories.Certifications);
            paramCol.AddWithValue("@expertise", categories.Expertise);
            paramCol.AddWithValue("@languages", categories.Languages);
            paramCol.AddWithValue("@licenses", categories.Licenses);
            paramCol.AddWithValue("@practices", categories.Practices);
            paramCol.AddWithValue("@professionalDetails", categories.Professional);
            paramCol.AddWithValue("@specializations", categories.Specializations);
        }

        private static void ModifyReportParamsV4(SqlParameterCollection paramCol, ProviderDetailCategories categories, int userId, int pageIndex, int pageSize)
        {

            paramCol.AddWithValue("@userId", userId);
            paramCol.AddWithValue("@pageIndex", pageIndex);
            paramCol.AddWithValue("@pageSize", pageSize);
            paramCol.AddWithValue("@affiliations", categories.Affiliations);
            paramCol.AddWithValue("@certifications", categories.Certifications);
            paramCol.AddWithValue("@expertise", categories.Expertise);
            paramCol.AddWithValue("@languages", categories.Languages);
            paramCol.AddWithValue("@licenses", categories.Licenses);
            paramCol.AddWithValue("@practices", categories.Practices);
            paramCol.AddWithValue("@professionalDetails", categories.Professional);
            paramCol.AddWithValue("@specializations", categories.Specializations);
        }

        private static void ModifySearchReportParams(SqlParameterCollection paramCol, ProviderDetailCategories categories, int userId, string query, int pageIndex, int pageSize)
        {
            paramCol.AddWithValue("@userId", userId);
            paramCol.AddWithValue("@query", query);
            paramCol.AddWithValue("@pageIndex", pageIndex);
            paramCol.AddWithValue("@pageSize", pageSize);
            paramCol.AddWithValue("@affiliations", categories.Affiliations);
            paramCol.AddWithValue("@certifications", categories.Certifications);
            paramCol.AddWithValue("@expertise", categories.Expertise);
            paramCol.AddWithValue("@languages", categories.Languages);
            paramCol.AddWithValue("@licenses", categories.Licenses);
            paramCol.AddWithValue("@practices", categories.Practices);
            paramCol.AddWithValue("@professionalDetails", categories.Professional);
            paramCol.AddWithValue("@specializations", categories.Specializations);
        }

        private static void ModifySearchAllReportParams(SqlParameterCollection paramCol, ProviderDetailCategories categories, string query, int pageIndex, int pageSize)
        {

            paramCol.AddWithValue("@query", query);
            paramCol.AddWithValue("@pageIndex", pageIndex);
            paramCol.AddWithValue("@pageSize", pageSize);
            paramCol.AddWithValue("@affiliations", categories.Affiliations);
            paramCol.AddWithValue("@certifications", categories.Certifications);
            paramCol.AddWithValue("@expertise", categories.Expertise);
            paramCol.AddWithValue("@languages", categories.Languages);
            paramCol.AddWithValue("@licenses", categories.Licenses);
            paramCol.AddWithValue("@practices", categories.Practices);
            paramCol.AddWithValue("@professionalDetails", categories.Professional);
            paramCol.AddWithValue("@specializations", categories.Specializations);
        }

        private static void ModifyBaseExcelHeaders(ExcelWorksheet worksheet, out int index)
        {
            index = 1;
            worksheet.Cells[1, index++].Value = "ID";
            worksheet.Cells[1, index++].Value = "Title";
            worksheet.Cells[1, index++].Value = "First Name";
            worksheet.Cells[1, index++].Value = "M.I.";
            worksheet.Cells[1, index++].Value = "Last Name";
            worksheet.Cells[1, index++].Value = "Gender";
            worksheet.Cells[1, index++].Value = "Phone";
            worksheet.Cells[1, index++].Value = "Fax";
            worksheet.Cells[1, index++].Value = "Email";
            worksheet.Cells[1, index++].Value = "Date Attested";
            worksheet.Column(index - 1).Style.Numberformat.Format = "dd-MM-yyyy";
            worksheet.Cells[1, index++].Value = "Compliant?";
        }

        private static void PopulateBaseExcelCells(ExcelWorksheet worksheet, ProviderReport providerReport, int rowIndex, out int colIndex)
        {
            colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Id;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Title;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.FirstName;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Mi;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.LastName;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Gender;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Phone;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Fax;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Email;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.DateAttested;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Compliant == 1 ? "Compliant" : "Noncompliant";
        }

        private static ProviderNonCompliant HydrateNonCompliantDetails(IDataReader reader, out int index)
        {
            ProviderNonCompliant providerNonCompliant = new ProviderNonCompliant();
            index = 0;
            providerNonCompliant.Id = reader.GetSafeInt32(index++);
            providerNonCompliant.Title = reader.GetSafeString(index++);
            providerNonCompliant.FirstName = reader.GetSafeString(index++);
            providerNonCompliant.Mi = reader.GetSafeString(index++);
            providerNonCompliant.LastName = reader.GetSafeString(index++);
            providerNonCompliant.Gender = reader.GetSafeString(index++);
            providerNonCompliant.Phone = reader.GetSafeString(index++);
            providerNonCompliant.Fax = reader.GetSafeString(index++);
            providerNonCompliant.Email = reader.GetSafeString(index++);
            providerNonCompliant.DateAttested = reader.GetSafeDateTime(index++);




            return providerNonCompliant;
        }

        private static void PopulateSharedBaseExcelCells(ExcelWorksheet worksheet, ProviderReport providerReport, int rowIndex, out int colIndex)
        {
            colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.Id;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.FirstName;
            worksheet.Cells[rowIndex, colIndex++].Value = providerReport.LastName;
        }

        private static Object GetPropValue(Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(Object obj, String name) where T : class
        {
            Object retVal = GetPropValue(obj, name);
            if (retVal == null) { return default(T); }

            //brings retVal back as null if type conversion is impossible
            //reference types can be null
            //"as" operator yields null upon unsucessful conversion
            return retVal as T;
        }

        public static T GetStructValue<T>(Object obj, String name) where T : struct
        {
            Object retVal = GetPropValue(obj, name);
            if (retVal == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            //value types can't be null
            return (T)retVal;
        }

        public Paged<ProviderBase> SelectByUserId(int userId, int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Providers_Select_ByUserId]";
            List<ProviderBase> list = null;
            Paged<ProviderBase> pagedItems = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, paramCol =>
            {
                paramCol.AddWithValue("@UserId", userId);
                paramCol.AddWithValue("@PageIndex", pageIndex);
                paramCol.AddWithValue("@PageSize", pageSize);
            }, (reader, set) =>
            {

                ProviderBase provider = HydrateProvider<ProviderBase>(reader, out int lastIndex);

                if (list == null)
                {
                    list = new List<ProviderBase>();
                }

                list.Add(provider);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(lastIndex);
                }
            });

            if (list != null)
            {
                pagedItems = new Paged<ProviderBase>(list, pageIndex, pageSize, totalCount);
            }

            return pagedItems;
        }
    }
}
