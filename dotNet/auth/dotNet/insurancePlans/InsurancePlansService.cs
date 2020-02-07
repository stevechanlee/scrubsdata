using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.InsurancePlans;
using Sabio.Models.Requests.InsurancePlans;
using Sabio.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Sabio.Services
{
    public class InsurancePlansService : IInsurancePlanService
    {
        IDataProvider _data = null;
        public InsurancePlansService(IDataProvider data)
        {
            _data = data;
        }
        public int Add(InsurancePlanAddRequest model, int userId)
        {
            int id = 0;
            string procName = "dbo.InsurancePlans_Insert_V2";

            _data.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@InsuranceProviderId", model.InsuranceProviderId);
                    col.AddWithValue("@Name", model.Name);
                    col.AddWithValue("@PlanTypeId", model.PlanTypeId);
                    col.AddWithValue("@PlanLevelId", model.PlanLevelId);
                    col.AddWithValue("@Code", model.Code);
                    col.AddWithValue("@PlanStatusId", model.PlanStatusId);
                    col.AddWithValue("@MinAge", model.MinAge);
                    col.AddWithValue("@MaxAge", model.MaxAge);
                    col.AddWithValue("@CreatedBy", userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                }, delegate (SqlParameterCollection returnCollection)
                {
                    object returnId = returnCollection["@Id"].Value;
                    Int32.TryParse(returnId.ToString(), out id);
                });
            return id;
        }
        public void Update(InsurancePlanUpdateRequest model, int userId)
        {
            _data.ExecuteNonQuery(
                "dbo.InsurancePlans_Update_V2",
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", model.Id);
                    col.AddWithValue("@InsuranceProviderId", model.InsuranceProviderId);
                    col.AddWithValue("@Name", model.Name);
                    col.AddWithValue("@PlanTypeId", model.PlanTypeId);
                    col.AddWithValue("@PlanLevelId", model.PlanLevelId);
                    col.AddWithValue("@Code", model.Code);
                    col.AddWithValue("@PlanStatusId", model.PlanStatusId);
                    col.AddWithValue("@MinAge", model.MinAge);
                    col.AddWithValue("@MaxAge", model.MaxAge);
                    col.AddWithValue("@ModifiedBy", userId);
                }, null);
        }
        public Paged<InsurancePlan> GetAllByPagination(int pageIndex, int pageSize)
        {
            Paged<InsurancePlan> pagedResults = null;
            List<InsurancePlan> result = null;
            InsurancePlan plan = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.InsurancePlans_SelectAll_V2"
                , delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                },
                delegate (IDataReader reader, short set)
                {
                    int index = 0;

                    plan = PlanMapper(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }
                    if (result == null)
                    {
                        result = new List<InsurancePlan>();
                    }
                    result.Add(plan);

                });
            if (result != null)
            {
                pagedResults = new Paged<InsurancePlan>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResults;
        }
        public Paged<InsurancePlan> GetSearchPagination(int pageIndex, int pageSize, string query)
        {
            Paged<InsurancePlan> pagedResults = null;
            List<InsurancePlan> list = null;
            InsurancePlan plan = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.InsurancePlans_Search",
               delegate (SqlParameterCollection col)
               {
                   col.AddWithValue("@PageIndex", pageIndex);
                   col.AddWithValue("@PageSize", pageSize);
                   col.AddWithValue("@Query", query);
               },
               delegate (IDataReader reader, short set)
               {
                   int index = 0;

                   plan = PlanMapper(reader, ref index);

                   if (totalCount == 0)
                   {
                       totalCount = reader.GetSafeInt32(index++);
                   }
                   if (list == null)
                   {
                       list = new List<InsurancePlan>();
                   }
                   list.Add(plan);
               });
            if (list != null)
            {
                pagedResults = new Paged<InsurancePlan>(list, pageIndex, pageSize, totalCount);
            }
            return pagedResults;
        }
        public InsurancePlan GetById(int id)
        {
            InsurancePlan plan = null;

            _data.ExecuteCmd("dbo.InsurancePlans_Select_ById_V2",
                delegate (SqlParameterCollection col)
                {

                    col.AddWithValue("@Id", id);

                }, delegate (IDataReader reader, short set)
                {
                    int index = 0;

                    plan = PlanMapper(reader, ref index);
                });
            return plan;
        }
        public Paged<InsurancePlan> GetByCreatedBy(int pageIndex, int pageSize, int id)
        {
            Paged<InsurancePlan> pagedResults = null;
            List<InsurancePlan> result = null;
            InsurancePlan plan = null;

            int totalCount = 0;

            _data.ExecuteCmd("dbo.InsurancePlans_Select_ByCreatedBy_V2",
               delegate (SqlParameterCollection col)
               {
                   col.AddWithValue("@PageIndex", pageIndex);
                   col.AddWithValue("@PageSize", pageSize);
                   col.AddWithValue("@CreatedBy", id);
               },
               delegate (IDataReader reader, short set)
               {
                   int index = 0;

                   plan = PlanMapper(reader, ref index);

                   if (totalCount == 0)
                   {
                       totalCount = reader.GetSafeInt32(index++);
                   }
                   if (result == null)
                   {
                       result = new List<InsurancePlan>();
                   }
                   result.Add(plan);

               });
            if (result != null)
            {
                pagedResults = new Paged<InsurancePlan>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResults;
        }
        public Paged<InsurancePlan> GetByProvider(int pageIndex, int pageSize, int providerId)
        {
            Paged<InsurancePlan> pagedResults = null;
            List<InsurancePlan> result = null;
            InsurancePlan plan = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.InsurancePlans_Select_ByProviderId_V2",
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                    col.AddWithValue("@ProviderId", providerId);
                },
                delegate (IDataReader reader, short set)
                {
                    int index = 0;

                    plan = PlanMapper(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }
                    if (result == null)
                    {
                        result = new List<InsurancePlan>();
                    }
                    result.Add(plan);
                });
            if (result != null)
            {
                pagedResults = new Paged<InsurancePlan>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResults;
        }
        public List<PlanLevel> GetAllPlansLevelType()
        {
            List<PlanLevel> list = null;
      
            _data.ExecuteCmd("dbo.InsurancePlansLevelTypes_SelectAll",
                null,
                delegate (IDataReader reader, short set)
                {
                    PlanLevel plan = new PlanLevel();

                    int index = 0;

                    plan.Id = reader.GetSafeInt32(index++);
                    plan.Name = reader.GetSafeString(index++);

                    if(list == null)
                    {
                        list = new List<PlanLevel>();
                    }
                    list.Add(plan);
                });
            return list;
        }
        public List<PlanType> GetAllPlansType()
        {
            List<PlanType> list = null;

            _data.ExecuteCmd("dbo.InsurancePlansTypes_SelectAll",
                null,
                delegate (IDataReader reader, short set)
                {
                    PlanType plan = new PlanType();

                    int index = 0;

                    plan.Id = reader.GetSafeInt32(index++);
                    plan.Name = reader.GetSafeString(index++);
                    plan.Code = reader.GetSafeString(index++);

                    if (list == null)
                    {
                        list = new List<PlanType>();
                    }

                    list.Add(plan);
                });
            return list;
        }
        public List<PlanStatus> GetAllPlansStatus()
        {
            List<PlanStatus> list = null;

            _data.ExecuteCmd("dbo.InsurancePlansStatus_SelectAll",
                null,
                delegate (IDataReader reader, short set)
                {
                    PlanStatus plan = new PlanStatus();
                    int index = 0;

                    plan.Id = reader.GetSafeInt32(index++);
                    plan.Name = reader.GetSafeString(index++);

                    if (list == null)
                    {
                        list = new List<PlanStatus>();
                    }

                    list.Add(plan);
                });
            return list;
        }

        public void Delete(int id)
        {
            _data.ExecuteNonQuery("[dbo].[InsurancePlans_Delete_ById]",
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                });
        }
        private static InsurancePlan PlanMapper(IDataReader reader, ref int index)
        {
            InsurancePlan plan = new InsurancePlan();
        plan.Id = reader.GetSafeInt32(index++);
            plan.Name = reader.GetSafeString(index++);
            plan.Code = reader.GetSafeString(index++);
            plan.MinAge = reader.GetSafeInt32(index++);
            plan.MaxAge = reader.GetSafeInt32(index++);
            plan.CreatedBy = reader.GetSafeInt32(index++);
            plan.ModifiedBy = reader.GetSafeInt32(index++);
            plan.DateCreated = reader.GetSafeDateTime(index++);
            plan.DateModified = reader.GetSafeDateTime(index++);

            plan.InsuranceProvider = new InsuranceProviderBase();
            plan.InsuranceProvider.Id = reader.GetSafeInt32(index++);
            plan.InsuranceProvider.Name = reader.GetSafeString(index++);
            plan.InsuranceProvider.SiteUrl = reader.GetSafeString(index++);
  
            plan.PlanLevel = new PlanLevel();
            plan.PlanLevel.Id = reader.GetSafeInt32(index++);
            plan.PlanLevel.Name = reader.GetSafeString(index++);
            
            plan.PlanType = new PlanType();
            plan.PlanType.Id = reader.GetSafeInt32(index++);
            plan.PlanType.Name = reader.GetSafeString(index++);
            plan.PlanType.Code = reader.GetSafeString(index++);

            plan.PlanStatus = new PlanStatus();
            plan.PlanStatus.Id = reader.GetSafeInt32(index++);
            plan.PlanStatus.Name = reader.GetSafeString(index++);
            return plan;
        }
    }
}
