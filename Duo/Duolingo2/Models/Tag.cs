using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Models
{
    public class Tag
    {
        public int TagId { get; }
        public string TagName { get; }

        public Tag(int tagId, string tagName)
        {
            TagId = tagId;
            TagName = tagName;
        }
    }
}
