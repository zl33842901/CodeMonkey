using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xLiAd.CodeMonkey.Entities
{

    public class Depart
    {
        #region 字段

        [Key]
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