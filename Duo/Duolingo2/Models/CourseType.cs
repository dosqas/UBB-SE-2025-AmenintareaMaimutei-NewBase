using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Models
{
    public class CourseType
    {
        public int TypeId { get; }
        public string TypeName { get; }
        public int Price { get; }

        public CourseType(int typeId, string typeName, int price)
        {
            TypeId = typeId;
            TypeName = typeName;
            Price = price;
        }
    }
}
