using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Entities.Entities
{
    public class InsertRequest
    {
        public string TableName { get; set; }
        public string DatabaseName { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
