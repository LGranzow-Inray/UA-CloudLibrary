﻿/* ========================================================================
 * Copyright (c) 2005-2021 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

namespace UACloudLibrary
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.Extensions.Configuration;
    using System.IO;
    using UACloudLibrary.DbContextModels;

    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options)
        : base(options)
        {
        }

        // Needed for design-time DB migration
        public AppDbContext()
        {
        }

        public static IModel GetInstance()
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(PostgreSQLDB.CreateConnectionString());
            using AppDbContext context = new AppDbContext(builder.Options);
            return context.Model;
        }

        // Needed for design-time DB migration
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

                string connectionString = "Please set connection string here during design time migration as env variables are not available!";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        // map to our tables
        public DbSet<DatatypeModel> datatype { get; set; }

        public DbSet<MetadataModel> metadata { get; set; }

        public DbSet<ObjecttypeModel> objecttype { get; set; }

        public DbSet<ReferencetypeModel> referencetype { get; set; }

        public DbSet<VariabletypeModel> variabletype { get; set; }

        public DbSet<AddressSpaceModel> addressSpace { get; set; }
        
        public DbSet<Organisation> organisation { get; set; }

        public DbSet<AddressSpaceCategory> category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DatatypeModel>();
            modelBuilder.Entity<MetadataModel>();
            modelBuilder.Entity<ObjecttypeModel>();
            modelBuilder.Entity<ReferencetypeModel>();
            modelBuilder.Entity<VariabletypeModel>();

            modelBuilder.Entity<Organisation>().ToTable("organisation");
            modelBuilder.Entity<AddressSpaceCategory>().ToTable("category");
            modelBuilder.Entity<AddressSpaceModel>()
                .Ignore(e => e.AdditionalProperties)
                .Ignore(e => e.Nodeset)
                .ToTable("addressspace");
            modelBuilder.Entity<AddressSpaceModel>().HasOne(e => e.Contributor).WithMany();
            modelBuilder.Entity<AddressSpaceModel>().HasOne(e => e.Category).WithMany();
        }
    }
}
