﻿using Microsoft.EntityFrameworkCore;
using MovieMaker.Domain.Features.Genres;
using MovieMaker.Domain.Features.Movies;
using MovieMaker.Domain.Features.Rentals;
using MovieMaker.Infra.Data.Features.Genres;
using MovieMaker.Infra.Data.Features.Movies;
using MovieMaker.Infra.Data.Features.Rentals;
using MovieMaker.Infra.Shared;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace MovieMaker.Infra.Data.Context
{

    public class MovieMakerDbContext : DbContext
    {

        public MovieMakerDbContext(
            DbContextOptions<MovieMakerDbContext> options
        ) : base(options)
        {
            RunMigrations(options);
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new MovieEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GenreEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RentalEntityConfiguration());

        }

        public void RunMigrations(DbContextOptions<MovieMakerDbContext> options)
        {

            var connection = CheckConnection();
            if (connection.HasError)
                throw connection.Error;

            var inMemory = options.Extensions.FirstOrDefault(x => x.ToString().Contains("InMemoryOptionsExtension"));

            if (inMemory == null && Database.GetPendingMigrations().Any())
            {
                Database.Migrate();
            }

        }

        public Response<Exception, bool> CheckConnection()
        {

            try
            {
                Database.OpenConnection();
                Database.CloseConnection();
            }
            catch (SqlException ex)
            {
                return ex;
            }

            return true;

        }

    }

}
