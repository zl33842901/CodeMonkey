using System;
using System.Collections.Generic;
using xLiAd.DapperEx.Repository;
using xLiAd.CodeMonkey.Entities;

namespace xLiAd.CodeMonkey.Infrastructure
{
    public class CycleRepository : Repository<Cycle>, ICycleRepository
    {
        public CycleRepository(IConnectionHolder conn) : base(conn) { }
    }

    public interface ICycleRepository : IRepository<Cycle>
    {

    }
}