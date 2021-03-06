﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace mvc_demo.database
{
    public class FrontEndContext : DbContext, IFrontEndContext
    {
        public FrontEndContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Person>()
                .Property(p => p.Id)
                .ValueGeneratedNever();
        }
    }
}