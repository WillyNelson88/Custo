using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custo
{
    class Operacoes
    {
        public int id { get; set; }

        public string Data { get; set; }

        public string Operacao { get; set; }

        public decimal Custo { get; set; }

        public string Observacoes { get; set; }

        public int Freq { get; set; }

        public string Fase { get; set; }
         
        public Decimal Total { get; set; }

        public Decimal Percentual { get; set; }
    }
}
