using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Entities.Entities
{
    public class StoredProcedures
    {
        public string Name {  get; set; } = string.Empty;
        public List<Parameters> parameters { get; set; } = new();
        public string body {  get; set; } = string.Empty;

    }
    public class Parameters
    {
        public string Name { get; set; } = string.Empty;
        public string datatype { get; set; } = string.Empty;
    }
}
