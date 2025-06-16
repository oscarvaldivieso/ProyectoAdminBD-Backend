using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Entities.Entities
{
        public class CreateViewRequest
        {
            public string ViewName { get; set; } = string.Empty;

            public List<string> SelectedTables { get; set; } = new();

            public List<SelectedColumn> SelectedColumns { get; set; } = new();

            public List<JoinCondition>? Joins { get; set; }

            public List<FilterCondition>? Filters { get; set; }
        }

        public class SelectedColumn
        {
            public string Table { get; set; } = string.Empty;
            public string Column { get; set; } = string.Empty;
            public string? Alias { get; set; }
        }

        public class JoinCondition
        {
            public string LeftTable { get; set; } = string.Empty;
            public string RightTable { get; set; } = string.Empty;
            public string LeftColumn { get; set; } = string.Empty;
            public string RightColumn { get; set; } = string.Empty;
        }

        public class FilterCondition
        {
            public string Table { get; set; } = string.Empty;
            public string Column { get; set; } = string.Empty;
            public string Operator { get; set; } = "="; // =, >, <, LIKE, etc.
            public string Value { get; set; } = string.Empty;
        }
    }
