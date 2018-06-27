﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.Service.BlockchainSettings.Core.Services;

namespace Lykke.Service.BlockchainSettings.Services
{
    public class BlockchainSettingsService : IBlockchainSettingsService
    {
        private readonly IBlockchainSettingsRepository _blockchainSettingsRepository;
        private readonly IBlockchainValidationService _blockchainValidationService;

        public BlockchainSettingsService(IBlockchainSettingsRepository blockchainSettingsRepository, 
            IBlockchainValidationService blockchainValidationService)
        {
            _blockchainSettingsRepository = blockchainSettingsRepository;
            _blockchainValidationService = blockchainValidationService;
        }

        /// <inheritdoc/>
        public async Task CreateAsync(BlockchainSetting settings)
        {
            var isValid = await _blockchainValidationService.ValidateAsync(settings.ApiUrl, settings.Type, settings.HotWalletAddress);

            if (!isValid)
                throw new NotValidException($"Blockchain integration api({settings.ApiUrl}) states that {settings.HotWalletAddress} is not a valid address");

            await _blockchainSettingsRepository.CreateAsync(settings);
        }

        public async Task<IEnumerable<BlockchainSetting>> GetAllAsync()
        {
            var collection = await _blockchainSettingsRepository.GetAllAsync();

            return collection;
        }

        public async Task<(IEnumerable<BlockchainSetting>, string continuationToken)> GetAllAsync(int take, string continuationToken = null)
        {
            var (collection, token) = await _blockchainSettingsRepository.GetAllAsync(take, continuationToken);

            return (collection, token);
        }

        public async Task<BlockchainSetting> GetAsync(string type)
        {
            var setting = await _blockchainSettingsRepository.GetAsync(type);

            return setting;
        }

        public async Task RemoveAsync(string type)
        {
            await _blockchainSettingsRepository.RemoveAsync(type);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(BlockchainSetting settings)
        {
            await _blockchainSettingsRepository.UpdateAsync(settings);
        }
    }
}
