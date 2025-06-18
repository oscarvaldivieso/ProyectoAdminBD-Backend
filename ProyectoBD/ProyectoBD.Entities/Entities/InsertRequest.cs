using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Entities.Entities
{
    public enum MotorBaseDatos
    {
        SqlServer,
        MySql
    }

    public class InsertRequest
    {
        public string Sql { get; set; }
        public string DatabaseName { get; set; }
        public MotorBaseDatos Motor { get; set; }
    }

    public class ConsultaTablaRequest
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public MotorBaseDatos Motor { get; set; }
    }

    public class RelacionRequest
    {
        public string DatabaseName { get; set; }
        public string TablaOrigen { get; set; }        
        public string ColumnaOrigen { get; set; }      
        public string TablaReferencia { get; set; }    
        public string ColumnaReferencia { get; set; }   
        public string NombreRelacion { get; set; }     
        public MotorBaseDatos Motor { get; set; }
    }

}
