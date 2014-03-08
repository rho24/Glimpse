using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.WebApi.Model
{
    public class ConfigurationModel
    {
        public List<KeyValuePair<object, object>> Properties { get; set; }

        public IEnumerable<FilterModel> Filters { get; set; }

        public List<FormatterModel> Formatters { get; set; }
    }
}
