using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoTracker.Data;
using TodoTracker.Models;

namespace TodoTracker.Pages;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    // Связываем редактируемую задачу с формой на странице
    [BindProperty]
    public TodoItem TaskItem { get; set; } = new();

    // Метод срабатывает при переходе на страницу редактирования (принимает id задачи из URL)
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        // Если задачи с таким Id нет в базе — возвращаем ошибку 404 (Не найдено)
        if (task == null)
        {
            return NotFound();
        }

        TaskItem = task;
        return Page();
    }

    // Метод срабатывает при нажатии кнопки «Сохранить изменения»
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Ищем исходную задачу в базе по Id и обновляем её поля
        var taskInDb = await _context.Tasks.FindAsync(TaskItem.Id);
        if (taskInDb == null)
        {
            return NotFound();
        }

        taskInDb.Title = TaskItem.Title;
        taskInDb.Description = TaskItem.Description;
        taskInDb.DueDate = TaskItem.DueDate;
        taskInDb.IsCompleted = TaskItem.IsCompleted;

        await _context.SaveChangesAsync();

        // Возвращаемся обратно на главную страницу
        return RedirectToPage("Index");
    }
}