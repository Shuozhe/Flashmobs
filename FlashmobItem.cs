using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashmobs
{
    class FlashmobItem : Flashmobs.Common.BindableBase
    {
        public string Time { get; set; }
        public string Place { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
    }
}
