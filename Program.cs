// Подключаем Entity Framework и нашу папку Data, чтобы Program.cs «увидел» AppDbContext
using Microsoft.EntityFrameworkCore;
using TodoTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем стандартную поддержку страниц Razor Pages
builder.Services.AddRazorPages();

// Регистрируем наш контекст базы данных в системе (Dependency Injection).
// Указываем, что будем использовать SQLite, а сама база сохранится в файл todo.db в папке проекта
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=todo.db"));

var app = builder.Build();

// Блок ниже автоматически создаст файл базы данных todo.db и все таблицы при первом запуске сайта,
// если их еще нет на диске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Стандартные настройки безопасности и маршрутизации (уже были в файле)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// Запускаем веб-сервер
app.Run();