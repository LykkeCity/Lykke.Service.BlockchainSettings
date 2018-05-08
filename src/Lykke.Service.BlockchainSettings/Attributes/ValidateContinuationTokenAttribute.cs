using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.BlockchainSettings.Attributes
{
    public class ValidateContinuationTokenAttribute : ValidationAttribute
    {
        public ValidateContinuationTokenAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string continuationToken = (string)validationContext.ObjectInstance;

            if (!string.IsNullOrEmpty(continuationToken))
            {
                try
                {
                    JsonConvert.DeserializeObject<TableContinuationToken>(Utils.HexToString(continuationToken));
                }
                catch
                {
                    return new ValidationResult($"Continuation Token - {continuationToken} is not valid", new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success;
        }
    }
}
