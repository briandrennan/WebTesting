namespace WebTesting.Core.EFCore;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public virtual DbSet<Todo> Todos { get; set; } = null!;

    public virtual DbSet<TodoDetail> Details { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Todo>(todo =>
        {
            todo.ToTable("Todo", "Todos");

            todo.HasKey(entity => entity.TodoId)
                .HasName("PK__Todo")
                .IsClustered();

            todo.Property(entity => entity.Id)
                .HasDefaultValueSql("(NEWSEQUENTIALID())");

            todo.HasIndex(entity => entity.Id)
                .HasDatabaseName("UIX__Todos_Todo__Id")
                .IsUnique();

            todo.Property(entity => entity.Title)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            todo.Property(entity => entity.Created)
                .HasDefaultValueSql("(SYSDATETIMEOFFSET())");

            todo.Property(entity => entity.Revision)
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TodoDetail>(detail =>
        {
            detail.ToTable("Detail", "Todos");

            detail.HasKey(entity => entity.TodoDetailId)
                .HasName("PK__TodoDetail")
                .IsClustered();

            detail.Property(entity => entity.TodoId);

            detail.HasIndex(entity => entity.TodoId)
                .HasDatabaseName("IX__Todos_TodoDetail__TodoId");

            detail.Property(entity => entity.Detail)
                .IsRequired(true)
                .HasMaxLength(2000)
                .IsUnicode(false);

            detail.Property(entity => entity.Revision)
                .IsConcurrencyToken();

            detail.HasOne(entity => entity.Todo)
                .WithMany(entity => entity.Details)
                .HasConstraintName("FK__Todos_TodoDetail__TodoId")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
