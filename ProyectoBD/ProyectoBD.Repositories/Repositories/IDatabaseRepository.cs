﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Repositories
{
    public interface IDatabaseRepository
    {
        Task CreateDatabaseAsync(string name);
    }
}
