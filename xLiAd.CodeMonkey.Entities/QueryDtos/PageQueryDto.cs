using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLiAd.CodeMonkey.Entities.QueryDtos
{
    public class PageQueryDto : CommonQueryDto
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
