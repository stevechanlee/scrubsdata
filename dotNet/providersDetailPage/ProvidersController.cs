using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Providers;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Locations;
using Sabio.Models.Requests.Practice;
using Sabio.Models.Requests.Providers;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Sabio.Web.Api.Controllers
{

    [Route("api/providers")]
    [ApiController]
    public class ProvidersController : BaseApiController
    {

        private IProvidersService _service = null;
        private IAuthenticationService<int> _auth = null;
        private IEmailService _emailService = null;
        public ProvidersController(IProvidersService service, IEmailService emailService
            , ILogger<ProvidersController> logger
            , IAuthenticationService<int> auth) : base(logger)
        {
            _service = service;
            _auth = auth;
            _emailService = emailService;
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> DeleteById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.DeleteById(id);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                response = new ErrorResponse(ex.Message);
                code = 500;
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpPost("new")]
        public ActionResult<ItemResponse<int>> InsertV2(ProviderAddRequest provider)
        {
            ObjectResult result = null;

            try
            {
                int userId = _auth.GetCurrentUserId();
                int newId = _service.InsertV2(provider, userId);
                ItemResponse<int> response = new ItemResponse<int> { Item = newId };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPost("report/full")]
        public FileStreamResult ReportSelectAll(ProviderDetailCategories categories)
        {

            BaseResponse response;
            try
            {                
                IUserAuthData user = _auth.GetCurrentUser();
                MemoryStream stream = null;
                if (user.Roles.Contains("Office Manager")||(user.Roles.Contains("SysAdmin")))
                {
                    if (user.Roles.Contains("SysAdmin"))
                    {
                        stream = _service.ReportSelectAll(categories, 0);
                    } else
                    {
                        stream = _service.ReportSelectAll(categories, user.Id);
                    }
                }
              

                if (stream == null)
                {
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    stream.Position = 0;
                    var contentType = "application/octet-stream";
                    var fileName = "ProviderReport.xlsx";
                    return File(stream, contentType, fileName);

                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());

                response = new ErrorResponse(ex.Message);
            }
            return null;
        }

        [HttpPost("report/pdf")]
        public ActionResult<ItemsResponse<List<ProviderReport>>> ReportSelectAllPdf(ProviderDetailCategories categories)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                IUserAuthData user = _auth.GetCurrentUser();
                List<ProviderReport> list = null;
                if (user.Roles.Contains("Office Manager") || (user.Roles.Contains("SysAdmin")))
                {
                    if (user.Roles.Contains("SysAdmin"))
                    {
                        list = _service.ReportSelectAllPdf(categories, 0);
                    }
                    else
                    {
                        list = _service.ReportSelectAllPdf(categories, user.Id);
                    }

                    if (list == null)
                    {
                        code = 404;
                        response = new ErrorResponse("App resource not found.");
                    }
                    else
                    {
                        response = new ItemsResponse<ProviderReport>() { Items = list };
                    }
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> InsertMultiple(ProvidersAddRequest2 provider)
        {
            ObjectResult result = null;
            try
            {
                int userId = _auth.GetCurrentUserId();

                int id = _service.InsertMultiple(provider, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPost("report")]
        public ActionResult<ItemResponse<Paged<ProviderReport>>> ReportSelectPaged(int pageIndex, int pageSize, ProviderDetailCategories categories)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                IUserAuthData user = _auth.GetCurrentUser();
                if (user.Roles.Contains("SysAdmin") || user.Roles.Contains("Office Manager"))
                {
                    Paged<ProviderReport> pagedItems = null;
                    if (user.Roles.Contains("SysAdmin"))
                    {
                        pagedItems = _service.ReportSelectAllPaged(pageIndex, pageSize, categories);
                    } else
                    {
                        pagedItems = _service.ReportSelectPaged(pageIndex, pageSize, user.Id, categories);
                    }
                    
                    if (pagedItems == null)
                    {
                        code = 404;
                        response = new ErrorResponse("App resource not found.");
                    }
                    else
                    {
                        response = new ItemResponse<Paged<ProviderReport>> { Item = pagedItems };
                    }
                } else
                {
                    code = 500;
                    response = new ErrorResponse("You do not meet the requirements to access this resource.");
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<Provider>>> Search(string q, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Provider> providerSearch = _service.Search(q, pageIndex, pageSize);

                if (providerSearch == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = providerSearch };
                }

            }
            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpGet("search/provider/list")]
        public ActionResult<ItemResponse<Paged<ProviderReport>>> SearchProviderList(string q, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<ProviderReport> providers = _service.SearchProviderList(q, pageIndex, pageSize);

                if (providers == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ProviderReport>>() { Item = providers };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPost("report/search")]
        public ActionResult<ItemResponse<Paged<ProviderReport>>> SearchProviderReportList(int pageIndex, int pageSize, string query, ProviderDetailCategories categories)

        {
            int code = 200;
            BaseResponse response;
            try
            {

                IUserAuthData user = _auth.GetCurrentUser();
                Paged<ProviderReport> pagedItems = null;
                if (user.Roles.Contains("SysAdmin") || user.Roles.Contains("Office Manager"))
                {
                   
                    if (user.Roles.Contains("SysAdmin"))
                    {
                        pagedItems = _service.SearchReportSelectAllPaged(pageIndex, pageSize, categories, query);
                    }
                    else
                    {
                        pagedItems = _service.SearchReportSelectPaged(pageIndex, pageSize, categories, query, user.Id);
                    }
                }
                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ProviderReport>>() { Item = pagedItems };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet]
        public ActionResult<ItemResponse<Paged<ProviderBase>>> SelectAll(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<ProviderBase> pagedItems = _service.SelectAll(pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ProviderBase>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("noncompliant")]
        public ActionResult<ItemResponse<Paged<ProviderNonCompliant>>> SelectAllNonCompliant(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                IUserAuthData user = _auth.GetCurrentUser();
                Paged<ProviderNonCompliant> pagedItems = null;
                if (user.Roles.Contains("SysAdmin") || user.Roles.Contains("Office Manager"))
                {

                    if (user.Roles.Contains("SysAdmin"))
                    {
                        pagedItems = _service.SelectAllNonCompliant(pageIndex, pageSize);
                    }
                    else
                    {
                        pagedItems = _service.SelectNonCompliant(pageIndex, pageSize, user.Id);
                    }
                }                

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ProviderNonCompliant>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("details")]
        public ActionResult<ItemResponse<Paged<Provider>>> SelectAllDetails(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Provider> pagedItems = _service.SelectAllDetails(pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("current")]
        public ActionResult<ItemResponse<Paged<Provider>>> SelectByCreatedBy(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _auth.GetCurrentUserId();
                Paged<Provider> pagedItems = _service.SelectByCreatedBy(userId, pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("expertise/{id:int}")]
        public ActionResult<ItemResponse<Paged<Provider>>> SelectByExpertise(int id, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Provider> pagedItems = _service.SelectByExpertise(id, pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("provider/{id:int}")]
        public ActionResult<ItemResponse<int>> SelectProviderByUserId(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int provider = _service.SelectProviderByUserId(id);
                if (provider == 0)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<int>() { Item = provider };

                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<ProviderBase>> SelectById(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                ProviderBase provider = _service.SelectById(id);
                if (provider == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<ProviderBase>() { Item = provider };

                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpGet("system")]
        public ActionResult<ItemResponse<Paged<SysProvider>>> SelectSysProvider(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<SysProvider> paged = _service.SelectSysProvider(pageIndex, pageSize);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<SysProvider>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("insuranceplans/{id:int}")]
        public ActionResult<ItemResponse<Paged<Provider>>> SelectByInsurancePlan(int id, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Provider> pagedItems = _service.SelectByInsurancePlan(id, pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("specializations/{id:int}")]
        public ActionResult<ItemResponse<Paged<Provider>>> SelectBySpecialization(int id, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Provider> pagedItems = _service.SelectBySpecialization(id, pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("states/{id:int}")]
        public ActionResult<ItemResponse<Paged<Provider>>> SelectByState(int id, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Provider> pagedItems = _service.SelectByState(id, pageIndex, pageSize);

                if (pagedItems == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Provider>> { Item = pagedItems };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("{id:int}/details")]
        public ActionResult<ItemResponse<Provider>> SelectDetailsById(int id)
        {
            int code = 200;
            BaseResponse response;
            try
            {
                IUserAuthData user = _auth.GetCurrentUser();
                Provider provider = null;
               
                    if (user.Roles.Contains("SysAdmin"))
                    {
                        provider = _service.SelectDetailsById(id);
                    }
                if (!user.Roles.Contains("Consumer") && !user.Roles.Contains("SysAdmin"))
                {
                    if(user.Roles.Contains("Provider"))
                        //provider is making request
                    {
                        provider = _service.SelectDetailsById(id);
                    } 
                    else
                    //office manager or provider assistant is making request
                    {
                        provider = _service.SelectDetailsById(id, user.Id);
                    }
                    
                }
               
                if (provider == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Provider>() { Item = provider };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateV2(ProviderUpdateRequest provider)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateV2(provider, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }


        [HttpPut("affiliations/update/{providerId:int}")]
        public ActionResult<ItemResponse<int>> UpdateAffiliations(AffiliationUpdateRequest model, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateAffiliations(model, providerId, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPut("licenses/update/{providerId:int}")]
        public ActionResult<ItemResponse<int>> UpdateLicenses(LicensesUpdateRequest model, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateLicenses(model, providerId, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPut("specializations/update/{providerId:int}")]
        public ActionResult<ItemResponse<int>> UpdateSpecializations(SpecializationsUpdateRequest model, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateSpecialization(model, providerId, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }


        [HttpPut("certifications/update/{providerId:int}")]
        public ActionResult<ItemResponse<int>> UpdateCertifications(CertificationsUpdateRequest model, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
             
                _service.UpdateCertifications(model, providerId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPut("expertise/update/{providerId:int}")]
        public ActionResult<ItemResponse<int>> UpdateExpertise(ExpertiseUpdateRequest model, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateExpertise(model, providerId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpPut("languages/update/{providerId:int}")]
        public ActionResult<ItemResponse<int>> UpdateLanguages(LanguagesUpdateRequest model, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateLanguages(model, providerId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }


        [HttpGet("{id:int}/paginate")]
        public ActionResult<ItemResponse<Paged<ProviderBase>>> SelectByUserId(int id, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<ProviderBase> providers = _service.SelectByUserId(id, pageIndex, pageSize);
                if (providers == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ProviderBase>>() { Item = providers };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPost("noncompliance")]
        public async Task<ActionResult<SuccessResponse>> SendNonComplianceEmail(string email)
        {
            BaseResponse response;
            int code = 200;
            try
            {
                await _emailService.NonCompliantEmail(email);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpPost("noncompliance/emails")]
        public async Task<ActionResult<SuccessResponse>> SendNonComplianceEmail(List<string> emails)
        {
            BaseResponse response;
            int code = 200;
            try
            {
                await _emailService.NonCompliantEmails(emails);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("list")]
        public ActionResult<ItemResponse<Paged<ProviderReport>>> SelectAllProviderList(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<ProviderReport> providers = _service.SelectProviderList(pageIndex, pageSize);
                if (providers == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ProviderReport>>() { Item = providers };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("providerlicense")]
        public ActionResult<ItemsResponse<List<ProviderReport>>> SelectProviderLicenseList()
        {

            int code = 200;
            BaseResponse response = null;
            try
            {
                IUserAuthData user = _auth.GetCurrentUser();
                if (user.Roles.Contains("Office Manager"))
                {
                    List<ProviderReport> list = _service.SelectAllProviderList(user.Id);
                    if (list == null)
                    {
                        code = 404;
                        response = new ErrorResponse("Resource not found");
                    }
                    else
                    {
                        response = new ItemsResponse<ProviderReport>() { Items = list };
                    }

                }
            } catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPost("create/details")]
        public ActionResult<ItemResponse<int>> Add(ProviderDetailsAddRequest provider)
        {
            ObjectResult result = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                int id = _service.Add(provider, userId);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }
        [HttpPut("lastattested/{id:int}")]
        public ActionResult<ItemsResponse<int>> UpdateLastAttested(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _auth.GetCurrentUserId();
                _service.UpdateLastAttested(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }
    } 
        
    
}
