using System.Collections.Generic;

using Minesweeper.Models;

namespace Minesweeper.Repositories
{
    public class Repository<T> where T : ModelBase
    {
        protected List<T> Entities { get; private set; }

        public Repository() => Entities = new List<T>();

        public void Add(T entity)
        {
            if (Contains(entity))
            {
                throw new RepositoryException(RepositoryException.DuplicateEntityError);
            }

            Entities.Add(entity);
        }

        public T Get(string id)
        {
            T entity = Entities.Find(e => e.Id == id);

            if (entity is null)
            {
                throw new RepositoryException(RepositoryException.EntityNotFoundError);
            }

            return entity;
        }

        public IEnumerable<T> GetAll() => Entities;

        public void Update(T entity)
        {
            if (!Contains(entity))
            {
                throw new RepositoryException(RepositoryException.EntityNotFoundError);
            }

            int index = Entities.FindIndex(e => e.Id == entity.Id);
            Entities[index] = entity;
        }

        public void Remove(T entity) => Remove(entity.Id);

        public void Remove(string id)
        {
            if (!Contains(id))
            {
                throw new RepositoryException(RepositoryException.EntityNotFoundError);
            }

            Entities.RemoveAll(e => e.Id == id);
        }

        public void Clear() => Entities.Clear();

        public bool Contains(T entity) => Contains(entity.Id);

        public bool Contains(string id) => Entities.Exists(e => e.Id == id);
    }
}
