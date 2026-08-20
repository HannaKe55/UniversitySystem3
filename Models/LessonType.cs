using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class LessonType
{
    public int LessonTypeId { get; set; }

    public string LessonTypeTitle { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
