using System.Security.Claims;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Mvc;

namespace IWantApp.Endpoints.Categories;

public class CategoryPut
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(
        [FromRoute] Guid id,
        CategoryRequest categoryRequest,
        HttpContext http,
        ApplicationDbContext context
    )
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();
        if (category == null)
            return Results.NotFound();
        category.EditInfo(categoryRequest.Name, categoryRequest.Active, userId);
        if (!category.IsValid)
            return Results.ValidationProblem(category.Notifications.ConvertProblemDetails());

        context.SaveChanges();

        return Results.Created($"/categories/{category.Id}", category.Id);
    }
}
