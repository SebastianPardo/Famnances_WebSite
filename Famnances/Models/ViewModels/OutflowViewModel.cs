using Famnances.DataCore.Entities;

namespace Famnances.Models.ViewModels
{
    public class OutflowViewModel
    {
        public Outflow Outflow { get; set; }
        public OverspentViewModel? OverSpent { get; set; }
    }

    public class OverspentViewModel
    {
        public decimal FullValue { get; set; }
        public decimal OverSpentValue { get; set; }
        public bool IsSaving { get; set; }
        public bool IsFull { get; set; }
        public Guid IdSelected { get; set; }


    }
}
