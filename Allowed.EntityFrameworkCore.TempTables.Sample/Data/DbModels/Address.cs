using Allowed.EntityFrameworkCore.TempTables.Sample.Enums;
using System.ComponentModel.DataAnnotations;

namespace Allowed.EntityFrameworkCore.TempTables.Sample.Data.DbModels
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public AddressType Type { get; set; }
    }
}
