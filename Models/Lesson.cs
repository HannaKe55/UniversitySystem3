using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int LessonCode { get; set; }

    public string LessonTitle { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
