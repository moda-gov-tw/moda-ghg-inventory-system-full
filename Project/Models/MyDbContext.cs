using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basicdata> Basicdatas { get; set; }

    public virtual DbSet<BasicdataFactoryaddress> BasicdataFactoryaddresses { get; set; }

    public virtual DbSet<Coefficient> Coefficients { get; set; }

    public virtual DbSet<DDatasource> DDatasources { get; set; }

    public virtual DbSet<DIntervalusetotal> DIntervalusetotals { get; set; }

    public virtual DbSet<DTransportation> DTransportations { get; set; }

    public virtual DbSet<DumptreatmentOutsourcing> DumptreatmentOutsourcings { get; set; }

    public virtual DbSet<Energyuse> Energyuses { get; set; }

    public virtual DbSet<Evidencefilemanage> Evidencefilemanages { get; set; }

    public virtual DbSet<Fireequipment> Fireequipments { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Organize> Organizes { get; set; }

    public virtual DbSet<Passwordhistory> Passwordhistories { get; set; }

    public virtual DbSet<RefrigerantHave> RefrigerantHaves { get; set; }

    public virtual DbSet<RefrigerantNone> RefrigerantNones { get; set; }

    public virtual DbSet<Resourceuse> Resourceuses { get; set; }

    public virtual DbSet<Selectdatum> Selectdata { get; set; }

    public virtual DbSet<Suppliermanage> Suppliermanages { get; set; }

    public virtual DbSet<VerifyCode> VerifyCodes { get; set; }

    public virtual DbSet<Workinghour> Workinghours { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=moda1;uid=demo;pwd=12345678", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.28-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Basicdata>(entity =>
        {
            entity.HasKey(e => e.BasicId).HasName("PRIMARY");

            entity.ToTable("basicdatas");

            entity.Property(e => e.BasicId).HasColumnType("int(11)");
            entity.Property(e => e.Account).HasMaxLength(200);
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .HasComment("連絡人信箱");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(30)
                .HasComment("聯絡人名稱");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(30)
                .HasComment("聯絡人電話");
            entity.Property(e => e.EndTime)
                .HasComment("結束時間")
                .HasColumnType("datetime");
            entity.Property(e => e.OrganName)
                .HasMaxLength(50)
                .HasComment("機關名稱");
            entity.Property(e => e.OrganNumber)
                .HasMaxLength(50)
                .HasComment("機關代號");
            entity.Property(e => e.StartTime)
                .HasComment("開始時間")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BasicdataFactoryaddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("basicdata_factoryaddress");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasComment("地址");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("名稱");
            entity.Property(e => e.ShiftMethod)
                .HasMaxLength(30)
                .HasComment("輪班方式");
            entity.Property(e => e.Sort)
                .HasColumnType("int(11)")
                .HasColumnName("sort");
            entity.Property(e => e.WherePlace)
                .HasMaxLength(50)
                .HasComment("盤查哪裡");
        });

        modelBuilder.Entity<Coefficient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("coefficient");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Category)
                .HasMaxLength(30)
                .HasComment("範疇別");
            entity.Property(e => e.Ch4coefficient)
                .HasPrecision(18, 6)
                .HasColumnName("CH4Coefficient");
            entity.Property(e => e.Ch4gwp)
                .HasPrecision(18, 6)
                .HasColumnName("CH4GWP");
            entity.Property(e => e.Ch4unit)
                .HasMaxLength(30)
                .HasColumnName("CH4Unit");
            entity.Property(e => e.Co2coefficient)
                .HasPrecision(18, 6)
                .HasColumnName("CO2Coefficient");
            entity.Property(e => e.Co2unit)
                .HasMaxLength(30)
                .HasColumnName("CO2Unit");
            entity.Property(e => e.EmissionSource)
                .HasMaxLength(50)
                .HasComment("排放源別");
            entity.Property(e => e.GreenhouseGases).HasMaxLength(50);
            entity.Property(e => e.HfcsGwp)
                .HasPrecision(18, 6)
                .HasColumnName("HFCsGWP");
            entity.Property(e => e.N2ocoefficient)
                .HasPrecision(18, 6)
                .HasColumnName("N2OCoefficient");
            entity.Property(e => e.N2ogwp)
                .HasPrecision(18, 6)
                .HasColumnName("N2OGWP");
            entity.Property(e => e.N2ounit)
                .HasMaxLength(30)
                .HasColumnName("N2OUnit");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasComment("名稱");
            entity.Property(e => e.PfcsGwp)
                .HasPrecision(18, 6)
                .HasColumnName("PFCsGWP");
            entity.Property(e => e.Sf6gwp)
                .HasPrecision(18, 6)
                .HasColumnName("SF6GWP");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .HasComment("單位");
            entity.Property(e => e.WasteMethod)
                .HasMaxLength(50)
                .HasComment("處理方法");
        });

        modelBuilder.Entity<DDatasource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("d_datasources");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.BindId).HasColumnType("int(11)");
            entity.Property(e => e.BindWhere).HasMaxLength(50);
            entity.Property(e => e.Datasource)
                .HasMaxLength(50)
                .HasComment("數據來源");
            entity.Property(e => e.EvidenceFile)
                .HasMaxLength(50)
                .HasComment("佐證資料");
            entity.Property(e => e.Management)
                .HasMaxLength(50)
                .HasComment("管理單位")
                .HasColumnName("management");
            entity.Property(e => e.Principal)
                .HasMaxLength(50)
                .HasComment("負責人員");
        });

        modelBuilder.Entity<DIntervalusetotal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("d_intervalusetotal");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.ArraySort)
                .HasComment("陣列取值用")
                .HasColumnType("int(11)");
            entity.Property(e => e.BindId)
                .HasComment("綁定表id")
                .HasColumnType("int(11)");
            entity.Property(e => e.BindWhere)
                .HasMaxLength(50)
                .HasComment("綁定哪個表");
            entity.Property(e => e.Num)
                .HasMaxLength(50)
                .HasComment("數值");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasComment("類型");
        });

        modelBuilder.Entity<DTransportation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("d_transportation");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.Air)
                .HasPrecision(11, 2)
                .HasComment("空運");
            entity.Property(e => e.BindId)
                .HasComment("綁定表Id")
                .HasColumnType("int(11)");
            entity.Property(e => e.BindWhere)
                .HasMaxLength(50)
                .HasComment("綁定哪個表");
            entity.Property(e => e.Car)
                .HasMaxLength(11)
                .HasComment("車種");
            entity.Property(e => e.EndLocation)
                .HasMaxLength(100)
                .HasComment("終點");
            entity.Property(e => e.Fuel)
                .HasMaxLength(50)
                .HasComment("燃料");
            entity.Property(e => e.Land)
                .HasPrecision(11, 2)
                .HasComment("陸運");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasComment("運輸方式");
            entity.Property(e => e.Sea)
                .HasPrecision(11, 2)
                .HasComment("海運");
            entity.Property(e => e.StartLocation)
                .HasMaxLength(100)
                .HasComment("起點");
            entity.Property(e => e.Tonnes)
                .HasPrecision(11, 2)
                .HasComment("噸數");
        });

        modelBuilder.Entity<DumptreatmentOutsourcing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("dumptreatment_outsourcing");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.AfertTotal)
                .HasPrecision(18, 4)
                .HasComment("總量");
            entity.Property(e => e.AfertUnit)
                .HasMaxLength(30)
                .HasComment("單位");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.BeforeTotal)
                .HasPrecision(18, 4)
                .HasComment("總量");
            entity.Property(e => e.BeforeUnit)
                .HasMaxLength(30)
                .HasComment("單位");
            entity.Property(e => e.CleanerName)
                .HasMaxLength(100)
                .HasComment("清運商名稱");
            entity.Property(e => e.ConvertNum)
                .HasPrecision(18, 4)
                .HasComment("單位轉換輛");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配比率");
            entity.Property(e => e.FactoryName)
                .HasMaxLength(100)
                .HasComment("廠址");
            entity.Property(e => e.FinalAddress)
                .HasMaxLength(200)
                .HasComment("最終處理地址");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .HasComment("備註");
            entity.Property(e => e.WasteId).HasColumnType("int(11)");
            entity.Property(e => e.WasteMethod)
                .HasMaxLength(50)
                .HasComment("空水廢處理方法");
            entity.Property(e => e.WasteName)
                .HasMaxLength(100)
                .HasComment("空水廢名稱");
        });

        modelBuilder.Entity<Energyuse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("energyuse");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.Account).HasMaxLength(200);
            entity.Property(e => e.AfertTotal)
                .HasPrecision(18, 4)
                .HasComment("轉換後總量");
            entity.Property(e => e.AfertUnit)
                .HasMaxLength(30)
                .HasComment("轉換後單位");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.BeforeTotal)
                .HasPrecision(18, 4)
                .HasComment("轉換前總量");
            entity.Property(e => e.BeforeUnit)
                .HasMaxLength(30)
                .HasComment("轉換前單位");
            entity.Property(e => e.ConvertNum)
                .HasPrecision(18, 4)
                .HasComment("單位轉換之值");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配比率");
            entity.Property(e => e.EditTime).HasColumnType("datetime");
            entity.Property(e => e.EnergyName)
                .HasMaxLength(20)
                .HasComment("能源名稱");
            entity.Property(e => e.EquipmentLocation)
                .HasMaxLength(100)
                .HasComment("設備位置");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasComment("設備名稱");
            entity.Property(e => e.FactoryName)
                .HasMaxLength(100)
                .HasComment("廠址");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .HasComment("備註");
            entity.Property(e => e.SupplierAddress)
                .HasMaxLength(200)
                .HasComment("供應商地址");
            entity.Property(e => e.SupplierId).HasColumnType("int(11)");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(200)
                .HasComment("供應商名稱");
        });

        modelBuilder.Entity<Evidencefilemanage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("evidencefilemanage");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.FileName)
                .HasMaxLength(200)
                .HasComment("檔案名稱");
            entity.Property(e => e.ItemId)
                .HasComment("哪筆資料")
                .HasColumnType("int(11)");
            entity.Property(e => e.Time)
                .HasComment("時間")
                .HasColumnType("datetime");
            entity.Property(e => e.WhereForm)
                .HasMaxLength(50)
                .HasComment("哪個表的檔案");
        });

        modelBuilder.Entity<Fireequipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fireequipment");

            entity.Property(e => e.Id)
                .HasComment("主鍵	")
                .HasColumnType("int(11)");
            entity.Property(e => e.AfertTotal)
                .HasPrecision(18, 4)
                .HasComment("轉換後總量");
            entity.Property(e => e.AfertUnit)
                .HasMaxLength(30)
                .HasComment("轉換後單位");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號	")
                .HasColumnType("int(11)");
            entity.Property(e => e.BeforeTotal)
                .HasPrecision(18, 4)
                .HasComment("轉換前總量");
            entity.Property(e => e.BeforeUnit)
                .HasMaxLength(30)
                .HasComment("轉換前單位");
            entity.Property(e => e.ConvertNum)
                .HasPrecision(18, 4)
                .HasComment("單位轉換之值");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配比率");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasComment("設備名稱");
            entity.Property(e => e.FactoryName)
                .HasMaxLength(100)
                .HasComment("廠址	");
            entity.Property(e => e.Ghgtype)
                .HasMaxLength(50)
                .HasComment("溫室氣體種類")
                .HasColumnName("GHGType");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .HasComment("備註");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("log");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DateTime)
                .HasComment("時間")
                .HasColumnType("datetime");
            entity.Property(e => e.Exception)
                .HasComment("錯誤內容")
                .HasColumnType("mediumtext");
            entity.Property(e => e.WhereFunction)
                .HasMaxLength(100)
                .HasComment("哪個方法發生錯誤");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("login");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.Account)
                .HasMaxLength(200)
                .HasComment("帳號");
            entity.Property(e => e.LoginDate)
                .HasComment("時間")
                .HasColumnType("datetime");
            entity.Property(e => e.LoginFailures)
                .HasComment("登入失敗次數")
                .HasColumnType("int(20)");
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .HasComment("狀態");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PRIMARY");

            entity.ToTable("member");

            entity.Property(e => e.MemberId)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.Account)
                .HasMaxLength(200)
                .HasComment("帳號");
            entity.Property(e => e.Addr)
                .HasMaxLength(200)
                .HasComment("聯絡地址");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasComment("公司名稱");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasComment("部門");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasComment("電子信箱");
            entity.Property(e => e.LoginType)
                .HasMaxLength(20)
                .HasComment("登入類型");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("名稱");
            entity.Property(e => e.OfficeLocation)
                .HasMaxLength(50)
                .HasComment("科別");
            entity.Property(e => e.Passwd)
                .HasMaxLength(50)
                .HasComment("密碼");
            entity.Property(e => e.Permissions)
                .HasMaxLength(10)
                .HasComment("權限")
                .HasColumnName("permissions");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .HasComment("職稱");
            entity.Property(e => e.Tel)
                .HasMaxLength(20)
                .HasComment("聯絡電話");
            entity.Property(e => e.UserType)
                .HasMaxLength(10)
                .HasComment("用戶類型");
        });

        modelBuilder.Entity<Organize>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("organize");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.Account)
                .HasMaxLength(200)
                .HasComment("帳號綁定");
            entity.Property(e => e.BasicId).HasColumnType("int(11)");
            entity.Property(e => e.ContactName)
                .HasMaxLength(50)
                .HasComment("聯絡窗口");
            entity.Property(e => e.EditLog).HasColumnType("mediumtext");
            entity.Property(e => e.EndTime)
                .HasComment("盤查結束時間")
                .HasColumnType("datetime");
            entity.Property(e => e.Inventory)
                .HasMaxLength(50)
                .HasComment("盤查表");
            entity.Property(e => e.StartTime)
                .HasComment("盤查開始時間")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("狀態");
            entity.Property(e => e.UpdateTime)
                .HasComment("更新時間")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Passwordhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("passwordhistory");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.Account)
                .HasMaxLength(200)
                .HasComment("帳號");
            entity.Property(e => e.CreateTime)
                .HasComment("創建時間")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasComment("密碼");
        });

        modelBuilder.Entity<RefrigerantHave>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("refrigerant_have");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.Datasource)
                .HasMaxLength(50)
                .HasComment("數據來源");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配比率");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasComment("設備名稱");
            entity.Property(e => e.EquipmentType)
                .HasMaxLength(30)
                .HasComment("設備類型");
            entity.Property(e => e.FactoryName)
                .HasMaxLength(100)
                .HasComment("廠址");
            entity.Property(e => e.Management)
                .HasMaxLength(50)
                .HasComment("管理單位");
            entity.Property(e => e.Principal)
                .HasMaxLength(50)
                .HasComment("負責人員");
            entity.Property(e => e.RefrigerantType)
                .HasMaxLength(50)
                .HasComment("冷媒種類");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .HasComment("備註");
            entity.Property(e => e.Total)
                .HasPrecision(18, 6)
                .HasComment("總量");
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .HasComment("單位");
        });

        modelBuilder.Entity<RefrigerantNone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("refrigerant_none");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配比率");
            entity.Property(e => e.EquipmentLocation)
                .HasMaxLength(100)
                .HasComment("設備位置");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasComment("設備名稱");
            entity.Property(e => e.EquipmentType)
                .HasMaxLength(30)
                .HasComment("設備類型");
            entity.Property(e => e.FactoryName)
                .HasMaxLength(100)
                .HasComment("廠址");
            entity.Property(e => e.MachineQuantity)
                .HasComment("機台數量")
                .HasColumnType("int(11)");
            entity.Property(e => e.Management)
                .HasMaxLength(50)
                .HasComment("管理單位");
            entity.Property(e => e.Manufacturers)
                .HasMaxLength(100)
                .HasComment("廠商or廠牌");
            entity.Property(e => e.Principal)
                .HasMaxLength(50)
                .HasComment("負責人員");
            entity.Property(e => e.RefrigerantType)
                .HasMaxLength(50)
                .HasComment("冷媒種類");
            entity.Property(e => e.RefrigerantWeight)
                .HasPrecision(18, 6)
                .HasComment("冷媒重量or容量");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .HasComment("備註");
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .HasComment("單位");
        });

        modelBuilder.Entity<Resourceuse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("resourceuse");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.AfertTotal)
                .HasPrecision(18, 4)
                .HasComment("總量	");
            entity.Property(e => e.AfertUnit)
                .HasMaxLength(30)
                .HasComment("單位	");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.BeforeTotal)
                .HasPrecision(18, 4)
                .HasComment("總量	");
            entity.Property(e => e.BeforeUnit)
                .HasMaxLength(30)
                .HasComment("單位	");
            entity.Property(e => e.ConvertNum)
                .HasPrecision(18, 4)
                .HasComment("單位轉換量	");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配比率");
            entity.Property(e => e.EnergyName)
                .HasMaxLength(20)
                .HasComment("能源名稱	");
            entity.Property(e => e.EquipmentLocation)
                .HasMaxLength(100)
                .HasComment("設備位置	");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasComment("設備名稱	");
            entity.Property(e => e.FactoryName)
                .HasMaxLength(100)
                .HasComment("廠址");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .HasComment("備註	");
            entity.Property(e => e.SupplierAddress)
                .HasMaxLength(200)
                .HasComment("供應商地址	");
            entity.Property(e => e.SupplierId).HasColumnType("int(11)");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(200)
                .HasComment("供應商名稱	");
        });

        modelBuilder.Entity<Selectdatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("selectdata")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasComment("編號");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Sort)
                .HasComment("排序")
                .HasColumnType("int(11)");
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Suppliermanage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("suppliermanage");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Account)
                .HasMaxLength(200)
                .HasComment("帳號");
            entity.Property(e => e.SupplierAddress)
                .HasMaxLength(200)
                .HasComment("供應商地址");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .HasComment("供應商名稱");
        });

        modelBuilder.Entity<VerifyCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("verify_code", tb => tb.HasComment("驗證表"))
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .HasComment("主鍵")
                .HasColumnType("int(11)");
            entity.Property(e => e.CreateTime)
                .HasComment("創建時間")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpireTime)
                .HasComment("過期時間")
                .HasColumnType("datetime");
            entity.Property(e => e.IsVerify)
                .HasDefaultValueSql("b'0'")
                .HasComment("是否已驗證")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .HasComment("驗證號碼");
            entity.Property(e => e.VerifyCode1)
                .HasMaxLength(50)
                .HasComment("驗證碼")
                .HasColumnName("VerifyCode");
            entity.Property(e => e.VerifyTime)
                .HasComment("驗證時間")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Workinghour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("workinghours");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.BasicId)
                .HasComment("基本資料編號")
                .HasColumnType("int(11)");
            entity.Property(e => e.Datasource)
                .HasMaxLength(50)
                .HasComment("數據來源");
            entity.Property(e => e.DistributeRatio)
                .HasPrecision(18, 4)
                .HasComment("分配方式");
            entity.Property(e => e.FactoryId)
                .HasComment("工廠id")
                .HasColumnType("int(11)");
            entity.Property(e => e.Item)
                .HasMaxLength(30)
                .HasComment("項目");
            entity.Property(e => e.Management)
                .HasMaxLength(50)
                .HasComment("管理部門")
                .HasColumnName("management");
            entity.Property(e => e.Principal)
                .HasMaxLength(50)
                .HasComment("負責人員");
            entity.Property(e => e.TotalWorkHour)
                .HasPrecision(18, 2)
                .HasComment("總工時");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
