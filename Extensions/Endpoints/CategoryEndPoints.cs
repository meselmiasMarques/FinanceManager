using FinanceManager.Data;
using FinanceManager.Models;
using FinanceManager.Requests.Categories;
using FinanceManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Extensions.Endpoints
{
    public static class CategoryEndPoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/categories", async (
                ICategoryService Service, 
                int skip = 1, 
                int take = 10) =>
            {
                var request = new CategoryGetAllRequest
                {
                    PageNumber = skip,
                    PageSize = take,
                   UserId = 1
                };
                var result = await Service.GetAllAsync(request);
                return result.Code == 200 ? Results.Ok(result) : Results.BadRequest(result);
            })
                 .WithDescription("Lista as categorias")
                 .WithTags("Categorias")
                 .WithOrder(1);

            app.MapGet("/categories{id:int}", async (ICategoryService Service, int id) =>
            {
                var result = await Service.GetByIdAsync(new CategoryGetByIdRequest { Id = id });
                return result.IsSuccess ?
                Results.Ok(result) 
                : Results.BadRequest(result);

            })
                .WithDescription("recupera uma categoria")
                .WithTags("Categorias")
                .WithOrder(2);

            app.MapPost("/categories", async (ICategoryService Service, CategoryCreateRequest request) =>
            {
                var result = await Service.AddAsync(new CategoryCreateRequest
                {
                    Name = request.Name,
                    Description = request.Description
                });
                return result.IsSuccess 
                    ? Results.Created($"/categories/{result.Data.Id}", result.Data) 
                    : Results.BadRequest(result);
            })
                .WithDescription("Cria uma nova categoria")
                .WithTags("Categorias")
                .WithOrder(3);

            app.MapPut("/categories/{id:int}", async (ICategoryService Service, CategoryUpdateRequest request, int id) =>
            {
                var result = await Service.UpdateAsync(new CategoryUpdateRequest
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description
                });
                return result.IsSuccess ? 
                    Results.Ok(result) 
                    : Results.BadRequest(result);
            })
                .WithDescription("Edita uma categoria")
                .WithTags("Categorias")
                .WithOrder(4);

            app.MapDelete("/categories/{id:int}", async (ICategoryService Service, int id) =>
            {
                var result = await Service.DeleteAsync(new CategoryDeleteRequest { Id = id });
                return result.IsSuccess ? 
                    Results.Ok(result) 
                    : Results.BadRequest(result);
            })
                .WithDescription("Deleta uma categoria")
                .WithTags("Categorias")
                .WithOrder(5);
        }
    }
}
