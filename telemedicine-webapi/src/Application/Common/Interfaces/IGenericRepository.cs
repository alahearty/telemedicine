using System.Linq.Expressions;
using telemedicine_webapi.Domain.Common;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IGenericRepository<T>
{
    void Add(T entity);
    void Delete(T entity);
    void DeleteMany(IEnumerable<T> entities);
    void Update(T entity);
    Task<bool> Exists(Expression<Func<T, bool>> expression);
    T? GetById(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    T? FirstOrDefault(Expression<Func<T, bool>> expression);
}
