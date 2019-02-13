﻿using System;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.Service.BlockchainSettings.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Core.Exceptions;

namespace Lykke.Service.BlockchainSettings.Services
{
    public class BlockchainExplorersService : IBlockchainExplorersService
    {
        private readonly IBlockchainExplorersRepository _blockchainExplorersRepository;
        private string _txHashTemplate = "{tx-hash}";

        public BlockchainExplorersService(IBlockchainExplorersRepository blockchainExplorersRepository)
        {
            _blockchainExplorersRepository = blockchainExplorersRepository;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<BlockchainExplorer>> GetAllAsync()
        {
            int take = 50;
            List< BlockchainExplorer> list = new List<BlockchainExplorer>(50);
            string continuationToken = null;
            do
            {
                var (explorers, newToken) = await _blockchainExplorersRepository.GetAllAsync(100, continuationToken);
                continuationToken = newToken;

                if (explorers != null && explorers.Any())
                    list.AddRange(explorers);

            } while (!string.IsNullOrEmpty(continuationToken));

            return list;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<BlockchainExplorer>> GetAsync(string type)
        {
            var allExplorers = await _blockchainExplorersRepository.GetAllForBlockchainAsync(type);

            return allExplorers;
        }

        ///<inheritdoc/>
        public async Task RemoveAsync(string type, string recordId)
        {
            await _blockchainExplorersRepository.RemoveAsync(type, recordId);
        }

        ///<inheritdoc/>
        public async Task CreateAsync(BlockchainExplorer explorer)
        {
            ThrowOnNotValidBlockchainExplorer(explorer);
            await _blockchainExplorersRepository.CreateAsync(explorer);
        }

        ///<inheritdoc/>
        public async Task UpdateAsync(BlockchainExplorer explorer)
        {
            ThrowOnNotValidBlockchainExplorer(explorer);
            await _blockchainExplorersRepository.UpdateAsync(explorer);
        }

        /*
            One url pattern per line
            Absolute and valid url should be specified for each pattern
            {tx-hash} placeholder is required for each url
            Zero number of patterns are allowed
            ?Empty lines are ignored
            ?Starting/trailing whitespaces in the line are ignored
         */
        private void ThrowOnNotValidBlockchainExplorer(BlockchainExplorer explorer)
        {
            if (explorer == null)
                throw new NotValidException($"Explorer should not be null");

            bool isValidUrl = Uri.TryCreate(explorer.ExplorerUrlTemplate, UriKind.Absolute, out _);

            if (!isValidUrl)
                throw new NotValidException($"{nameof(explorer.ExplorerUrlTemplate)} is not valid Url template");

            if (!explorer.ExplorerUrlTemplate.Contains(_txHashTemplate))
                throw new NotValidException($"{nameof(explorer.ExplorerUrlTemplate)} is not valid Url template");

            //var urlWithoutTempolate = explorer.ExplorerUrlTemplate.Replace(_txHashTemplate, "");
        }
    }
}