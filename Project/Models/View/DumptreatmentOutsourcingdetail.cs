namespace Project.Models.View
{
    public class DumptreatmentOutsourcingdetail
    {
        public DTransportation DTransportation { get; set; }
        public List<DIntervalusetotal> DIntervalusetotals_DateTime { get; set; }
        public List<DIntervalusetotal> DIntervalusetotals_StartTime { get; set; }
        public List<DIntervalusetotal> DIntervalusetotals_EndTime { get; set; }
        public List<DIntervalusetotal> DIntervalusetotals_Num { get; set; }
        public DIntervalusetotal Unit { get; set; }

        public DIntervalusetotal SelMonth { get; set; }

        public DDatasource DDatasource { get; set; }
    }
}
