using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoTracker.Data;
using TodoTracker.Models;

namespace TodoTracker.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<TodoItem> TodoList { get; set; } = new();

    [BindProperty]
    public TodoItem NewTask { get; set; } = new();

    // Параметры поиска и фильтрации.
    // SupportsGet = true означает, что значения будут считываться из адресной строки (URL)
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; } = "all"; // Возможные значения: "all", "active", "completed"

    public async Task OnGetAsync()
    {
        // Начинаем строить запрос к базе (на этом этапе SQL-запрос еще не отправляется!)
        IQueryable<TodoItem> query = _context.Tasks;

        // 1. Если пользователь выбрал фильтр по статусу — добавляем условие Where
        if (Filter == "active")
        {
            query = query.Where(t => !t.IsCompleted);
        }
        else if (Filter == "completed")
        {
            query = query.Where(t => t.IsCompleted);
        }

        // 2. Если введена строка поиска — ищем совпадения в названии или описании
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            string search = SearchTerm.Trim().ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(search) ||
                (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        // 3. Сортируем результат и только в момент вызова ToListAsync() отправляем готовый SQL-запрос в базу
        TodoList = await query
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        _context.Tasks.Add(NewTask);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();
        }
        // Сохраняем текущие фильтры при перезагрузке страницы
        return RedirectToPage(new { Filter, SearchTerm });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { Filter, SearchTerm });
    }
}