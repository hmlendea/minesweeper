using System;
using System.Runtime.Serialization;

namespace Minesweeper.Repositories
{
    [Serializable]
    public class RepositoryException : Exception
    {
        public const string DuplicateEntityError = "An entity with the same Id already exists in the repository.";
        public const string EntityNotFoundError = "No entity with the specified Id was found in the repository.";

        public RepositoryException() { }

        public RepositoryException(string message) : base(message) { }

        public RepositoryException(string message, Exception inner) : base(message, inner) { }
    }
}
