using System;
using System.Collections.Generic;

namespace test_lester02.Models
{
    public partial class TblRfidCodiCaptEmbarque
    {
        public int Id { get; set; }
        public string Codebar { get; set; } = null!;
        public string? Acronimo { get; set; }
        public DateTime? FechaLectura { get; set; }
        public string? ObjReferencia { get; set; }
        public int Tipo { get; set; }
        public string? Viaje { get; set; }
    }
}
