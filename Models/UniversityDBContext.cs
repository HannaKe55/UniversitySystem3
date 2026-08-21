using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystem3.Models;

public partial class UniversityDBContext : DbContext
{
    public UniversityDBContext()
    {
    }

    public UniversityDBContext(DbContextOptions<UniversityDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassLocation> ClassLocations { get; set; }

    public virtual DbSet<CourseReg> CourseRegs { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<Paticipation> Paticipations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Term> Terms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=UniversitySystem;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263C30EC7858");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.AttStatus).HasMaxLength(5);
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Class).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_Attendance_Class");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Attendance_Student");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassCode).HasMaxLength(50);
            entity.Property(e => e.ClassLocationId).HasColumnName("ClassLocationID");
            entity.Property(e => e.DateTime).HasMaxLength(50);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FinalExamDate).HasColumnType("datetime");
            entity.Property(e => e.ForEnteranceYearId).HasColumnName("ForEnteranceYearID");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.TermId).HasColumnName("TermID");

            entity.HasOne(d => d.ClassLocation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.ClassLocationId)
                .HasConstraintName("FK_Class_ClassLocation");

            entity.HasOne(d => d.Employee).WithMany(p => p.Classes)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Class_Employee");

            entity.HasOne(d => d.ForEnteranceYear).WithMany(p => p.ClassForEnteranceYears)
                .HasForeignKey(d => d.ForEnteranceYearId)
                .HasConstraintName("FK_Class_Term1");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Classes)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("FK_Class_Lesson");

            entity.HasOne(d => d.LessonType).WithMany(p => p.Classes)
                .HasForeignKey(d => d.LessonTypeId)
                .HasConstraintName("FK_Class_LessonType");

            entity.HasOne(d => d.Major).WithMany(p => p.Classes)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Class_Major");

            entity.HasOne(d => d.Term).WithMany(p => p.ClassTerms)
                .HasForeignKey(d => d.TermId)
                .HasConstraintName("FK_Class_Term");
        });

        modelBuilder.Entity<ClassLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassLoc__3214EC279857B0FE");

            entity.ToTable("ClassLocation");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LocationName).HasMaxLength(50);
        });

        modelBuilder.Entity<CourseReg>(entity =>
        {
            entity.HasKey(e => e.CourseRegId).HasName("PK__CourseRe__63587D61537F56DF");

            entity.ToTable("CourseReg");

            entity.Property(e => e.CourseRegId).HasColumnName("CourseRegID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TermId).HasColumnName("TermID");

            entity.HasOne(d => d.Class).WithMany(p => p.CourseRegs)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_CourseReg_Class");

            entity.HasOne(d => d.Employee).WithMany(p => p.CourseRegs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_CourseReg_Employee");

            entity.HasOne(d => d.Status).WithMany(p => p.CourseRegs)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_CourseReg_status");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRegs)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_CourseReg_Student");

            entity.HasOne(d => d.Term).WithMany(p => p.CourseRegs)
                .HasForeignKey(d => d.TermId)
                .HasConstraintName("FK_CourseReg_Term");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF119B9378A");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.LoginId, "UQ_Employee_LoginId").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EmpCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.FullName).HasMaxLength(400);
            entity.Property(e => e.LastDegree).HasMaxLength(200);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.NationalCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Title).HasMaxLength(400);

            entity.HasOne(d => d.Login).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.LoginId)
                .HasConstraintName("FK_Employee_Login");

            entity.HasOne(d => d.Major).WithMany(p => p.Employees)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Employee_Major");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Employee__Role1");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("Lesson");

            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.LessonTitle).HasMaxLength(100);
        });

        modelBuilder.Entity<LessonType>(entity =>
        {
            entity.HasKey(e => e.LessonTypeId).HasName("PK__LessonTy__D7FA804E02644C8F");

            entity.ToTable("LessonType");

            entity.Property(e => e.LessonTypeTitle).HasMaxLength(20);
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK___Login__4DDA283845E075BF");

            entity.ToTable("_Login");

            entity.Property(e => e.LoginId).HasColumnName("LoginID");
            entity.Property(e => e.Pass).HasMaxLength(100);
            entity.Property(e => e.Uername).HasMaxLength(50);
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.MajorId).HasName("PK__Major__D5B8BFB10B54E08B");

            entity.ToTable("Major");

            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.MajorName).HasMaxLength(300);
        });

        modelBuilder.Entity<Paticipation>(entity =>
        {
            entity.HasKey(e => e.Cpid).HasName("PK__Paticipa__F5B22BE6F3B92F2C");

            entity.ToTable("Paticipation");

            entity.Property(e => e.Cpid).HasColumnName("CPID");
            entity.Property(e => e.EmpId).HasColumnName("EmpID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Emp).WithMany(p => p.Paticipations)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK_Paticipation_Employee");

            entity.HasOne(d => d.Student).WithMany(p => p.Paticipations)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Paticipation_Student");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK___Role__8AFACE3A25990DCA");

            entity.ToTable("_Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(300);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__status__36257A38A99F2502");

            entity.ToTable("status");

            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.StatusTitle)
                .HasMaxLength(50)
                .HasColumnName("statusTitle");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A795E748EFF");

            entity.ToTable("Student");

            entity.HasIndex(e => e.LoginId, "UQ_Student_LoginId").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.CurrentTermId).HasColumnName("CurrentTermID");
            entity.Property(e => e.EnteranceYearId).HasColumnName("EnteranceYearID");
            entity.Property(e => e.FullName).HasMaxLength(400);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.NationalCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.CurrentTerm).WithMany(p => p.StudentCurrentTerms)
                .HasForeignKey(d => d.CurrentTermId)
                .HasConstraintName("FK_Student_Term1");

            entity.HasOne(d => d.EnteranceYear).WithMany(p => p.StudentEnteranceYears)
                .HasForeignKey(d => d.EnteranceYearId)
                .HasConstraintName("FK_Student_Term2");

            entity.HasOne(d => d.Login).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.LoginId)
                .HasConstraintName("FK_Student_Login");

            entity.HasOne(d => d.Major).WithMany(p => p.Students)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Student_Major");
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.ToTable("Term");

            entity.Property(e => e.TermId).HasColumnName("TermID");
            entity.Property(e => e.OddOreven)
                .HasMaxLength(4)
                .HasColumnName("oddOReven");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.TermCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.TermTitle).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
