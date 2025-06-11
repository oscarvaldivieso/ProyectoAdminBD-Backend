using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Entities
{
    public class CreateTable
    {
        public string TableName { get; set; }
        public List<ColumnDefinition> Columns { get; set; }
        public CreateTable()
        {
            Columns = new List<ColumnDefinition>();
        }
    }

    public class ColumnDefinition
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
    }
}
