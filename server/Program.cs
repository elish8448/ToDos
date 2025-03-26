// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();

// app.MapGet("/", () => "Hello World!");

// app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TodoApi;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(9,0,0)) 
//     ));
builder.Services.AddDbContext<ToDoDbContext>(
    options=> options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB")
    ,new MySqlServerVersion(new Version(9,0,0))));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.MapGet("/", async () =>
{
    return "Results.Accepted()";
});
app.MapGet("/tasks", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});
app.MapGet("/tasksId/{id}", async (ToDoDbContext db,int id) =>
{
    return await db.Items.FindAsync(id);
});
// app.MapPost("/tasks", async (ToDoDbContext db, TaskItem task) =>
// {
//     db.Items.Add(task);
//     await db.SaveChangesAsync();
//     return Results.Created($"/tasks/{task.Id}", task);
// });

// app.MapPut("/tasks/{id}", async (ToDoDbContext db, int id, TaskItem updatedTask) =>
// {
//     var task = await db.Items.FindAsync(id);
//     if (task == null) return Results.NotFound();

//     task. = updatedTask.Title;
//     task.IsCompleted = updatedTask.IsCompleted;

//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });
app.MapPut("/tasks/updateItem/{id}", async (ToDoDbContext db, int id,Item updateItem )=>
{
    var item= await db.Items.FindAsync(id);
    if (item == null) return Results.NotFound($"Item with ID {id} not found.");
    item.IsComplete=updateItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapPost("/addItem", async (ToDoDbContext db, Item item) =>
{
    db.Items.Add(item); // הוספת הפריט למסד הנתונים
    await db.SaveChangesAsync(); // שמירת השינויים
   // החזרת סטטוס 201 עם הפריט שנוסף
    return Results.Created($"/addItem/{item.Id}", item);
});
//מחיקה מוצר
app.MapDelete("/deleteItem/{id}", async (ToDoDbContext db, int id) =>{
    var item = await db.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound($"Item with ID {id} not found.");
    }   
    
    db.Items.Remove(item); 
    await db.SaveChangesAsync(); 
    return Results.Ok($"Item with ID {id} deleted successfully.");
});
app.Run();


public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks { get; set; }
}




