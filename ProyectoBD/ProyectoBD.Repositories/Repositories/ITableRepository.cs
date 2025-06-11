using ProyectoBD.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Repositories
{
    public interface ITableRepository
    {
        Task CreateTableAsync(CreateTable table);
    }
}
