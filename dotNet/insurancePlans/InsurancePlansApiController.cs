using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.InsurancePlans;
using Sabio.Models.Requests.InsurancePlans;
using Sabio.Services;
using Sabio.Services.Interface;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/insurance/plans")]
    [ApiController]
    public class InsurancePlansApiController : BaseApiController
    {
        private IInsurancePlanService _service = null;
        private IAuthenticationService<int> _authService = null;
        public InsurancePlansApiController(IInsurancePlanService service
            , ILogger<InsurancePlansApiController> logger
            , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(InsurancePlanAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>();
                response.Item = id;
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

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(InsurancePlanUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;

                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<InsurancePlan>>> GetAllByPagination(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<InsurancePlan> page = _service.GetAllByPagination(pageIndex, pageSize);
                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<InsurancePlan>> { Item = page };
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
        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<InsurancePlan>>> GetSearchPagination(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<InsurancePlan> page = _service.GetSearchPagination(pageIndex, pageSize, query);
                if(page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<InsurancePlan>> { Item = page };
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

        [HttpGet("current")]
        public ActionResult<ItemResponse<Paged<InsurancePlan>>> GetByCreatedBy(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            int userId = _authService.GetCurrentUserId();

            try
            {
                Paged<InsurancePlan> page = _service.GetByCreatedBy(pageIndex, pageSize, userId);
                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<InsurancePlan>> { Item = page };
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
        [HttpGet("provider")]
        public ActionResult<ItemResponse<Paged<InsurancePlan>>> GetByProvider(int pageIndex, int pageSize, int providerId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<InsurancePlan> page = _service.GetByProvider(pageIndex, pageSize, providerId);
                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<InsurancePlan>> { Item = page };
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
        public ActionResult GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                InsurancePlan plan = _service.GetById(id);
                if (plan == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<InsurancePlan> { Item = plan };
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
        [HttpGet("level/type")]
        public ActionResult<ItemsResponse<PlanLevel>> GetAllPlanLevelType()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<PlanLevel> plan = _service.GetAllPlansLevelType();
                if (plan == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemsResponse<PlanLevel> { Items = plan };
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
        [HttpGet("type")]
        public ActionResult<ItemsResponse<PlanType>> GetAllPlansType()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<PlanType> plans = _service.GetAllPlansType();
                if (plans == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemsResponse<PlanType> { Items = plans };
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
        [HttpGet("status")]
        public ActionResult<ItemsResponse<PlanStatus>> GetAllPlansStatus()
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                List<PlanStatus> plans = _service.GetAllPlansStatus();
                if (plans == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                }
                else
                {
                    response = new ItemsResponse<PlanStatus> { Items = plans };
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
        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;

                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
    }
}
