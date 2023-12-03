namespace TaskApp.Models
{
    using System.Collections.Generic;

    public class TaskViewModel
    {
        public List<Task> Tasks { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string FilterDescription { get; set; }
    }
}
