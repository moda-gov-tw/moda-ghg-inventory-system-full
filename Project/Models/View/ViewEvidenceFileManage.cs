namespace Project.Models.View
{
    public class ViewEvidenceFileManage
    {
        public List<Coefficient> Coefficient { get; set; }
        public List<Energyuse> Energyuse { get; set; }

        public List<Evidencefilemanage> EvidenceFileManage { get; set; }

        public List<Resourceuse>? Resourceuse { get; set; }

        public List<Fireequipment> Fireequipment { get; set; }
        //public List<Goodsdistribution> Goodsdistribution { get; set; }
        //public List<UseanddisposalUse> UseanddisposalUse { get; set; }
        //public List<UseanddisposalDisposal> UseanddisposalDisposal { get; set; }
        public List<DumptreatmentOutsourcing> DumptreatmentOutsourcing { get; set; }
        //public List<DumptreatmentFactoryinput> DumptreatmentFactoryinput { get; set; }
        public List<RefrigerantHave> RefrigerantHave { get; set; }
        public List<RefrigerantNone> RefrigerantNone { get; set; }
        
        public List<BasicdataFactoryaddress> FactoryName { get; set; }
        public List<Workinghour> Workinghours { get; set; }
        public List<Workinghour> Workinghours_Item_0 { get; set; }
        public List<Workinghour> Workinghours_Item_1 { get; set; }
        public List<Workinghour> Workinghours_Item_2 { get; set; }
        public List<Workinghour> Workinghours_Item_3 { get; set; }
        public List<Workinghour> Workinghours_Item_4 { get; set; }

        public List<BasicdataFactoryaddress> BasicdataFactoryaddresses { get; set; }
    }
}
