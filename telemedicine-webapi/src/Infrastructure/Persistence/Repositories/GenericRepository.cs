﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Common;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity<int>
{
    readonly IApplicationDbContext _context;
    readonly DbSet<T> entities;

    public GenericRepository(IApplicationDbContext context)
    {
        _context = context;
        entities = context.Set<T>();
    }

    public void Add(T entity)
    {
        entities.Add(entity);
    }

    public void Delete(T entity)
    {
        entities.Remove(entity);
    }

    public void DeleteMany(IEnumerable<T> entitiesTobeDeleted)
    {
        entities.RemoveRange(entitiesTobeDeleted);
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> expression)
    {
        return await entities.AnyAsync(expression);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        var result = await entities.Where(expression).ToListAsync();
        return result;
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        var result = entities.FirstOrDefault(expression);
        return await Task.FromResult(result!);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var result = await entities.ToListAsync();
        return result;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var result = entities.Find(id);
        return await Task.FromResult(result!);
    }

    public void Update(T entity)
    {
        entities.Update(entity);
    }
}
