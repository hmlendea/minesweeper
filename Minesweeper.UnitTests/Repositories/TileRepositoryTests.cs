using System.Collections.Generic;

using Minesweeper.Models;
using Minesweeper.Repositories;

using NUnit.Framework;

namespace Minesweeper.UnitTests.Repositories
{
    [TestFixture]
    public class TileRepositoryTests
    {
        private TileRepository repository = null!;

        private static Tile BuildTile(int x, int y)
            => new Tile { X = x, Y = y };

        [SetUp]
        public void SetUp()
        {
            repository = new TileRepository();
        }

        [Test]
        public void GivenAnEmptyRepository_WhenGettingAllTiles_ThenNoTilesAreReturned()
            => Assert.That(repository.GetAll(), Is.Empty);

        [Test]
        public void GivenAnEmptyRepository_WhenCheckingContainment_ThenFalseIsReturned()
            => Assert.That(repository.Contains("0,0"), Is.False);

        [TestCase(0, 0)]
        [TestCase(3, 7)]
        [TestCase(15, 15)]
        public void GivenARepositoryWithATile_WhenCheckingContainmentByCoordinates_ThenTrueIsReturned(int x, int y)
        {
            repository.Add(BuildTile(x, y));

            Assert.That(repository.Contains($"{x},{y}"), Is.True);
        }

        [Test]
        public void GivenARepositoryWithATile_WhenGettingItByCoordinates_ThenTheTileIsReturned()
        {
            Tile addedTile = BuildTile(4, 9);
            repository.Add(addedTile);

            Assert.That(repository.Get(4, 9), Is.EqualTo(addedTile));
        }

        [Test]
        public void GivenARepositoryWithTiles_WhenGettingAMissingTileByCoordinates_ThenNullIsReturned()
        {
            repository.Add(BuildTile(0, 0));

            Assert.That(repository.Get(5, 5), Is.Null);
        }

        [Test]
        public void GivenARepositoryWithATile_WhenGettingItById_ThenTheTileIsReturned()
        {
            Tile addedTile = BuildTile(2, 3);
            repository.Add(addedTile);

            Assert.That(repository.Get("2,3"), Is.EqualTo(addedTile));
        }

        [Test]
        public void GivenARepositoryWithoutTheTile_WhenGettingItById_ThenARepositoryExceptionIsThrown()
            => Assert.That(
                  () => repository.Get("9,9"),
                  Throws.TypeOf<RepositoryException>());

        [Test]
        public void GivenARepositoryWithATile_WhenAddingADuplicateTile_ThenARepositoryExceptionIsThrown()
        {
            repository.Add(BuildTile(1, 1));

            Assert.That(
                () => repository.Add(BuildTile(1, 1)),
                Throws.TypeOf<RepositoryException>());
        }

        [Test]
        public void GivenARepositoryWithATile_WhenUpdatingIt_ThenTheNewTileIsStored()
        {
            repository.Add(BuildTile(1, 2));

            Tile updatedTile = BuildTile(1, 2);
            updatedTile.Cleared = true;
            repository.Update(updatedTile);

            Assert.That(repository.Get(1, 2).Cleared, Is.True);
        }

        [Test]
        public void GivenARepositoryWithoutTheTile_WhenUpdating_ThenARepositoryExceptionIsThrown()
            => Assert.That(
                  () => repository.Update(BuildTile(0, 0)),
                  Throws.TypeOf<RepositoryException>());

        [Test]
        public void GivenARepositoryWithTiles_WhenRemovingOneById_ThenOnlyThatTileIsRemoved()
        {
            repository.Add(BuildTile(0, 0));
            repository.Add(BuildTile(1, 1));

            repository.Remove("0,0");

            Assert.That(repository.GetAll(), Has.Count.EqualTo(1));
            Assert.That(repository.Contains("1,1"), Is.True);
        }

        [Test]
        public void GivenARepositoryWithoutTheTile_WhenRemovingItById_ThenARepositoryExceptionIsThrown()
            => Assert.That(
                  () => repository.Remove("0,0"),
                  Throws.TypeOf<RepositoryException>());

        [Test]
        public void GivenARepositoryWithTiles_WhenClearingIt_ThenNoTilesRemain()
        {
            repository.Add(BuildTile(0, 0));
            repository.Add(BuildTile(1, 1));

            repository.Clear();

            Assert.That(repository.GetAll(), Is.Empty);
        }

        [Test]
        public void GivenMultipleTiles_WhenGettingAllTiles_ThenAllTilesAreReturned()
        {
            List<Tile> expectedTiles = new List<Tile>
            {
                BuildTile(0, 0),
                BuildTile(1, 2),
                BuildTile(3, 4)
            };

            foreach (Tile tile in expectedTiles)
            {
                repository.Add(tile);
            }

            Assert.That(repository.GetAll(), Is.EqualTo(expectedTiles));
        }
    }
}
