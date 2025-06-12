using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Entities.Entities
{
    public class AlterTable
    {
        public string TableName { get; set; } = string.Empty;

        public List<ColumnAlteration> Alterations { get; set; } = new();
    }

    public class ColumnAlteration
    {
        public string Operation { get; set; } = string.Empty; // "ADD", "DROP", "ALTER"
        public string ColumnName { get; set; } = string.Empty;
        public string? DataType { get; set; }  // Solo necesario para ADD o ALTER
        public bool? IsNullable { get; set; }  // Solo para ALTER
    }
}
