using System;
using JetBrains.Annotations;

namespace OW.Experts.Domain.Infrastructure.Repository
{
    public interface IRepository<T>
        where T : DomainObject
    {
        /// <summary>
        /// Inserts or updates entity to repository.
        /// </summary>
        /// <param name="entity">Entity to adding or updating.</param>
        void AddOrUpdate([NotNull] T entity);

        /// <summary>
        /// Gets entity by id. If entity does not exist throw InvalidOperationException.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Entity of type T.</returns>
        /// <exception cref="InvalidOperationException">Throw if entity does not exist.</exception>
        [NotNull]
        T GetById(Guid id);

        /// <summary>
        /// Removes entity from repository.
        /// </summary>
        /// <param name="entity">Entity to remove.</param>
        void Remove([NotNull] T entity);
    }
}