using System;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using Lykke.Service.BlockchainSettings.Contract.Responses;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;
using Lykke.Service.BlockchainSettings.Shared.Attributes;
using Lykke.Service.BlockchainSettings.Shared.Cache.Interfaces;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainExplorers.Controllers
{
    [Route("api/blockchain-explorers")]
    public class BlockchainExplorersController : Controller
    {
        private readonly IBlockchainExplorersServiceCached _blockchainExplorersServiceCached;

        public BlockchainExplorersController(IBlockchainExplorersServiceCached blockchainExplorersServiceCached)
        {
            _blockchainExplorersServiceCached = blockchainExplorersServiceCached;
        }

        /// <summary>
        /// Get All of Blockchain settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [ApiKeyAuthorize(ApiKeyAccessType.Read)]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(typeof(BlockchainExplorersCollectionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAllAsync()
        {
            var settings = await _blockchainExplorersServiceCached.GetAllAsync();

            if (settings == null || !settings.Any())
                return NoContent();

            var response = new BlockchainExplorersCollectionResponse()
            {
                Collection = settings.Select(MapToResponse),
            };

            return Ok(response);
        }

        /// <summary>
        /// Get specific Blockchain setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("{type}")]
        [ApiKeyAuthorize(ApiKeyAccessType.Read)]
        [SwaggerOperation("Get")]
        [ProducesResponseType(typeof(BlockchainExplorersCollectionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetForTypeAsync([Required][FromRoute]string type)
        {
            var explorers = await _blockchainExplorersServiceCached.GetAsync(type);

            if (explorers == null || !explorers.Any())
                return NoContent();

            var response = new BlockchainExplorersCollectionResponse()
            {
                Collection = explorers.Select(MapToResponse)
            };

            return Ok(response);
        }

        /// <summary>
        /// Get specific Blockchain setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("{type}/{recordId}")]
        [ApiKeyAuthorize(ApiKeyAccessType.Read)]
        [SwaggerOperation("Get")]
        [ProducesResponseType(typeof(BlockchainExplorerResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([Required][FromRoute]string type, [Required][FromRoute] string recordId)
        {
            var explorer = await _blockchainExplorersServiceCached.GetAsync(type, recordId);

            if (explorer == null)
                return NoContent();

            var response = MapToResponse(explorer);

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
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateAsync([FromBody]BlockchainExplorerCreateRequest request)
        {
            BlockchainExplorer explorer = MapToDomain(request);

            try
            {
                await _blockchainExplorersServiceCached.CreateAsync(explorer);
            }
            catch (NotValidException e)
            {
                return CreateContentResult(StatusCodes.Status400BadRequest, e.Message);
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
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateAsync([FromBody]BlockchainExplorerUpdateRequest request)
        {
            BlockchainExplorer explorer = MapToDomain(request);

            try
            {
                await _blockchainExplorersServiceCached.UpdateAsync(explorer);
            }
            catch (NotValidException e)
            {
                return CreateContentResult(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (DoesNotExistException e)
            {
                return CreateContentResult(StatusCodes.Status400BadRequest, e.Message);
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
        [HttpDelete("{type}/{recordId}")]
        [ApiKeyAuthorize(ApiKeyAccessType.Write)]
        [SwaggerOperation("Remove")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RemoveAsync([FromRoute]string type, [FromRoute]string recordId)
        {
            try
            {
                await _blockchainExplorersServiceCached.RemoveAsync(type, recordId);
            }
            catch (DoesNotExistException e)
            {
                return NoContent();
            }

            return Ok();
        }

        #region Private

        private BlockchainExplorerResponse MapToResponse(BlockchainExplorer domainModel)
        {
            return new BlockchainExplorerResponse()
            {
                ETag = domainModel.ETag,
                BlockchainType = domainModel.BlockchainType,
                ExplorerUrlTemplate = domainModel.ExplorerUrlTemplate,
                RecordId = domainModel.RecordId
            };
        }

        private BlockchainExplorer MapToDomain(BlockchainExplorerCreateRequest request)
        {
            return new BlockchainExplorer()
            {
                BlockchainType = request.BlockchainType,
                ExplorerUrlTemplate = request.ExplorerUrlTemplate,
                RecordId = Guid.NewGuid().ToString()
            };
        }

        private BlockchainExplorer MapToDomain(BlockchainExplorerUpdateRequest request)
        {
            return new BlockchainExplorer()
            {
                ETag = request.ETag,
                BlockchainType = request.BlockchainType,
                ExplorerUrlTemplate = request.ExplorerUrlTemplate,
                RecordId = request.RecordId
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
