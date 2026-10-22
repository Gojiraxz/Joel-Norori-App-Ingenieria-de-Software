using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class DatoaTablesRequestDto
    {
       public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearchDto? Search { get; set; }
        public List<DataTablesOrderDto> Order { get; set; }
        public List<DataTablesColumnDto> Columns { get; set; }


    }
    public class DataTableSearchDto
    {
        public string? Value { get; set; }
        public bool Regex { get; set; }
    }
    public class DataTablesOrderDto
    {
        public int Column { get; set; }
        public string Dir { get; set; } = "asc";
    }
    public class DataTablesColumnDto
    {
        public string Data { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTableSearchDto? Search { get; set; }
    }
}
