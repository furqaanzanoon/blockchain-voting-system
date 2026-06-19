using Microsoft.EntityFrameworkCore;
using VotingAPI.Models.Entities;

namespace VotingAPI.Data
{
    public class VotingDbContext : DbContext
    {
        public VotingDbContext(DbContextOptions options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Voter> Voters { get; set; }
        public DbSet<VoteTransaction> VoteTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── User ────────────────────────────────────────────────────

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Ensure email uniqueness at the database level

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EthAddress)
                .IsUnique()
                .HasFilter("[EthAddress] IS NOT NULL"); // Allow multiple NULLs (users without a connected wallet)

            // User → Elections (created by this user)
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedElections)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict); // User deletes should not auto-delete elections, elections are permanent records

            // User → Voters
            modelBuilder.Entity<User>()
                .HasMany(u => u.Voters)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Voter table is already deleted via Election cascade, so User must not also cascade delete it

            // ── Election ─────────────────────────────────────────────────

            // Election → Candidates
            modelBuilder.Entity<Election>()
                .HasMany(e => e.Candidates)
                .WithOne(c => c.Election)
                .HasForeignKey(c => c.ElectionId);

            // Election → Voters
            modelBuilder.Entity<Election>()
                .HasMany(e => e.Voters)
                .WithOne(v => v.Election)
                .HasForeignKey(v => v.ElectionId);

            // Election → VoteTransactions
            modelBuilder.Entity<Election>()
                .HasMany(e => e.VoteTransactions)
                .WithOne(vt => vt.Election)
                .HasForeignKey(vt => vt.ElectionId)
                .OnDelete(DeleteBehavior.Restrict); // VoteTransactions are permanent audit logs, never auto-delete, even if election is deleted

            // ── Candidate ────────────────────────────────────────────────

            // Candidate → VoteTransactions
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.VoteTransactions)
                .WithOne(vt => vt.Candidate)
                .HasForeignKey(vt => vt.CandidateId)
                .OnDelete(DeleteBehavior.Restrict); // VoteTransactions are permanent audit logs, never auto-delete, even if candidate is deleted

            // ── Voter ────────────────────────────────────────────────────

            modelBuilder.Entity<Voter>()
                .HasIndex(v => new { v.ElectionId, v.UserId })
                .IsUnique(); // Ensure a user can only be a voter once per election (one voter per election)


            // ── Performance Indexes ───────────────────────────────────────────
            modelBuilder.Entity<Election>(entity =>
            {
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.Status, e.StartTime });
                entity.HasIndex(e => new { e.Status, e.EndTime });
            });
            
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasIndex(e => e.ElectionId);
            });
            
            modelBuilder.Entity<VoteTransaction>(entity =>
            {
                entity.HasIndex(e => e.ElectionId);
            });
        }
    }
}