using System;
using System.Collections.Generic;

namespace xLiAd.CodeMonkey.Entities.Dtos
{

    public class DepartDto
    {
        #region DTO

        public int Id { get; set; }

        public string Name { get; set; }

        public string NameChain { get; set; }

        public string BudgetDepartId { get; set; }

        public string NameChainOfBudgetDepart { get; set; }

        public int ParentId { get; set; }

        public string ManagerCode { get; set; }

        public int DepartLevel { get; set; }

        public int BusinessDirection { get; set; }

        public bool Deleted { get; set; }

        public string RelationBodies { get; set; }

        public string ShadowChain { get; set; }

        public string AllSubIds { get; set; }

        #endregion
    }
}