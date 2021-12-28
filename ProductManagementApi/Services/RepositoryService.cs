using ProductManagementApi.Data;
using ProductManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagementApi.Services
{
    public interface IRepositoryService
    {
        public AppDbContext appDbContext { get; }

        ValueTask<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity;
        ValueTask<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity;
        ValueTask<T> DisableAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity;
        ValueTask<T> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity;

    }
    public class RepositoryService : IRepositoryService
    {
        public AppDbContext appDbContext { get; }

        public RepositoryService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async ValueTask<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity
        {
            {
                try
                {
                    entity.IsActive = true;
                    entity.CreatedAt = DateTime.Now;
                    await appDbContext.AddAsync(entity, cancellationToken);
                    if (commitChanges)
                    {
                        await appDbContext.SaveChangesAsync(cancellationToken);
                    }

                    return entity;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async ValueTask<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity
        {
            {
                try
                {
                    entity.ModifiedAt = DateTime.Now;

                    appDbContext.Update<T>(entity);

                    if (commitChanges)
                    {
                        await appDbContext.SaveChangesAsync(cancellationToken);
                    }

                    return entity;
                }
                catch
                {
                    throw;
                }
            }

        }
        public async ValueTask<T> DisableAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity
        {
            {
                try
                {
                    entity.IsActive = false;
                    entity.ModifiedAt = DateTime.Now;

                    appDbContext.Update<T>(entity);

                    if (commitChanges)
                    {
                        await appDbContext.SaveChangesAsync(cancellationToken);
                    }

                    return entity;
                }
                catch
                {
                    throw;
                }
            }

        }


        public async ValueTask<T> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default, bool commitChanges = true) where T : DbEntity
        {
            {
                try
                {
                    appDbContext.Remove<T>(entity);

                    if (commitChanges)
                    {
                        await appDbContext.SaveChangesAsync(cancellationToken);
                    }

                    return entity;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
