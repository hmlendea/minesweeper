using System.Collections.Generic;

using Minesweeper.Models;

namespace Minesweeper.Repositories
{
    public class Repository<T> where T : EntityBase
    {
        protected List<T> Entities { get; set; }

        public int Size
        {
            get { return Entities.Count; }
        }

        public Repository()
        {
            Entities = new List<T>();
        }

        public virtual void Add(T entity)
        {
            if (Entities.Contains(entity))
                throw new RepositoryException("The specified entity already exists");

            Entities.Add(entity);
        }

        public virtual T Get(string id)
        {
            T entity = Entities.Find(E => E.Id == id);

            if (entity == null)
                throw new RepositoryException("An entity with the specified identifier does not exist");

            return entity;
        }

        public virtual List<T> GetAll()
        {
            return Entities;
        }

        public virtual void Update(T entity)
        {
            T oldEntity = Get(entity.Id);
            oldEntity = entity; // TODO Real update - currently it is being done by the service
        }

        public virtual void Remove(T entity)
        {
            if (!Contains(entity))
                throw new RepositoryException("The specified entity does not exist");

            Entities.Remove(entity);
        }

        public virtual void Remove(string id)
        {
            if (!Contains(id))
                throw new RepositoryException("An entity with the specified identifier does not exist");

            Entities.RemoveAll(T => T.Id == id);
        }

        public virtual void Clear()
        {
            Entities.Clear();
        }

        public virtual bool Contains(T entity)
        {
            return Entities.Find(E => E.Equals(entity)) != null;
        }

        public virtual bool Contains(string id)
        {
            return Entities.Find(E => E.Id == id) != null;
        }
    }
}