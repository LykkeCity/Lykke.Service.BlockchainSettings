﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Attributes;
using Lykke.Service.BlockchainSettings.Cache;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Models.Responses;
using Lykke.Service.BlockchainSettings.Settings.ServiceSettings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Lykke.Service.BlockchainSettings.Controllers
{
    [Route("api/blockchainSettings")]
    public class BlockchainSettingsController : Controller
    {
        private readonly IBlockchainSettingsServiceCached _blockchainSettingsService;

        public BlockchainSettingsController(IBlockchainSettingsServiceCached blockchainSettingsService)
        {
            _blockchainSettingsService = blockchainSettingsService;
        }

        /// <summary>
        /// Get All of Blockchain settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [ApiKeyAuthorize(ApiKeyAccessType.Read)]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(typeof(BlockchainSettingsCollectionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync()
        {
            var settings = await _blockchainSettingsService.GetAllAsync();

            if (settings == null || !settings.Any())
                return NoContent();

            var response = new BlockchainSettingsCollectionResponse()
            {
                Collection = settings.Select(MapToResponse),
            };

            return Ok(response);
        }

        /// <summary>
        /// Get specific Blockchain setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyAuthorize(ApiKeyAccessType.Read)]
        [SwaggerOperation("Get")]
        [ProducesResponseType(typeof(BlockchainSettingsCreateRequest), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAsync([Required][FromQuery]string type)
        {
            var setting = await _blockchainSettingsService.GetAsync(type);

            if (setting == null)
                return NoContent();

            var response = MapToResponse(setting);

            return Ok(response);
        }

        /// <summary>
        /// Create Blockchain settings
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyAuthorize(ApiKeyAccessType.Write)]
        [SwaggerOperation("Create")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody]BlockchainSettingsCreateRequest request)
        {
            BlockchainSetting settings = MapToDomain(request);

            try
            {
                await _blockchainSettingsService.CreateAsync(settings);
            }
            catch (AlreadyExistsException e)
            {
                return CreateContentResult(StatusCodes.Status409Conflict, e.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Update Blockchain settings
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ApiKeyAuthorize(ApiKeyAccessType.Write)]
        [SwaggerOperation("Update")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromBody]BlockchainSettingsUpdateRequest request)
        {
            BlockchainSetting settings = MapToDomain(request);

            try
            {
                await _blockchainSettingsService.UpdateAsync(settings);
            }
            catch (AlreadyUpdatedException e)
            {
                return CreateContentResult(StatusCodes.Status409Conflict, e.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Remove Blockchain settings
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ApiKeyAuthorize(ApiKeyAccessType.Write)]
        [SwaggerOperation("Remove")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveAsync([FromQuery]string type)
        {
            try
            {
                await _blockchainSettingsService.RemoveAsync(type);
            }
            catch (DoesNotExistException e)
            {
                return NoContent();
            }

            return Ok();
        }

        #region Private

        private BlockchainSettingsResponse MapToResponse(BlockchainSetting domainModel)
        {
            return new BlockchainSettingsResponse()
            {
                ETag = domainModel.ETag,
                Type = domainModel.Type,
                HotWalletAddress = domainModel.HotWalletAddress,
                ApiUrl = domainModel.ApiUrl,
                SignServiceUrl = domainModel.SignServiceUrl
            };
        }

        private BlockchainSetting MapToDomain(BlockchainSettingsCreateRequest request)
        {
            return new BlockchainSetting()
            {
                ETag = request.ETag,
                Type = request.Type,
                HotWalletAddress = request.HotWalletAddress,
                ApiUrl = request.ApiUrl,
                SignServiceUrl = request.SignServiceUrl
            };
        }

        private ContentResult CreateContentResult(int statusCode, string errorMessage)
        {
            var errorResponse = new ErrorResponse()
            {
                ErrorMessage = errorMessage,
            };

            return new ContentResult()
            {
                StatusCode = statusCode,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse)
            };
        }

        #endregion
    }
}
