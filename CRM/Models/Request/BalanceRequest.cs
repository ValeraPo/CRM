using Marvelous.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class BalanceRequest
    {
        [Range(1, 113, ErrorMessage = "Такой валюты не существует")]
        public Currency CurrencyType { get; set; }
    }
}
