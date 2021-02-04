using System;

namespace ServicesForWorkingWithCheques.Domain.Core
{
    public sealed class Cheque
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public decimal Summ { get; set; }
        public decimal Discount { get; set; }
        public string[] Articles { get; set; }
    }
}