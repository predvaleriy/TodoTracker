using System.ComponentModel.DataAnnotations;

namespace TodoTracker.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название задачи")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }
}