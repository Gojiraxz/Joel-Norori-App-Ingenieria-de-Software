using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class DataTableResponseDto<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsTotalFiltered { get; set; }
        public List<T> Data { get; set; } = [];
    }
}
