using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class ClassLocation
{
    public int Id { get; set; }

    public string? LocationName { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
