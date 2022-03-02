using System;
using System.Collections.Generic;
using xLiAd.DapperEx.Repository;
using xLiAd.CodeMonkey.Entities;

namespace xLiAd.CodeMonkey.Infrastructure
{
    public class DepartRepository : Repository<Depart>, IDepartRepository
    {
        public DepartRepository(IConnectionHolder conn) : base(conn) { }
    }

    public interface IDepartRepository : IRepository<Depart>
    {

    }
}