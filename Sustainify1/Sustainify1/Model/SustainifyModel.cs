using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainify1.Model
{
    public class SustainifyModel
    {
        public int Id { get; set; }
        public string ActionCode { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImpactLevel { get; set; }
        public string ImpactLevelDescription { get; set; }
        public string Frequency { get; set; }
        public int NumberOfCheckboxes { get; set; }
    }
}
