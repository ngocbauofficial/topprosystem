﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TopProSystem.Areas.MasterSetting.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TopProSystemEntities : DbContext
    {
        public TopProSystemEntities()
            : base("name=TopProSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<MA003> MA003 { get; set; }
        public DbSet<MA004> MA004 { get; set; }
        public DbSet<MA005> MA005 { get; set; }
        public DbSet<MA010> MA010 { get; set; }
        public DbSet<MA001> MA001 { get; set; }
        public DbSet<LogError> LogErrors { get; set; }
        public DbSet<MA011> MA011 { get; set; }
        public DbSet<MA012> MA012 { get; set; }
        public DbSet<Tmp_MaterialWarehousingResult> Tmp_MaterialWarehousingResult { get; set; }
        public DbSet<INV001> INV001 { get; set; }
        public DbSet<TRA001> TRA001 { get; set; }
        public DbSet<PUH001> PUH001 { get; set; }
        public DbSet<PermissionAction> PermissionActions { get; set; }
        public DbSet<PermissionRecord> PermissionRecords { get; set; }
        public DbSet<Role_Mapping> Role_Mapping { get; set; }
        public DbSet<Role_Mapping_Action> Role_Mapping_Action { get; set; }
        public DbSet<SecurityLevel> SecurityLevels { get; set; }
        public DbSet<RawMaterialType> RawMaterialTypes { get; set; }
        public DbSet<SteelGrade> SteelGrades { get; set; }
        public DbSet<PrinterSetting> PrinterSettings { get; set; }
        public DbSet<InspectionItemNoMaterial> InspectionItemNoMaterials { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<MA007> MA007 { get; set; }
        public DbSet<MA006> MA006 { get; set; }
        public DbSet<MA009> MA009 { get; set; }
        public DbSet<MA002> MA002 { get; set; }
        public DbSet<PUR001> PUR001 { get; set; }
        public DbSet<LogUserAction> LogUserActions { get; set; }
    }
}
