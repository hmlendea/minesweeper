using System.Linq;

using Minesweeper.GameLogic.GameManagers;
using Minesweeper.Models;
using Minesweeper.Settings;

using NUnit.Framework;

namespace Minesweeper.UnitTests.GameLogic.GameManagers
{
    [TestFixture]
    public class GameManagerTests
    {
        private GameManager gameManager = null!;

        [SetUp]
        public void SetUp()
        {
            gameManager = new GameManager();
            gameManager.LoadContent();
        }

        private static int CountMinedTiles(GameManager manager)
            => manager.GetAllTiles().Count(tile => tile.Mined);

        private static void ClearAllTiles(GameManager manager)
        {
            foreach (Tile tile in manager.GetAllTiles())
            {
                if (!tile.Mined)
                {
                    manager.ClearTile(tile.X, tile.Y);
                }
            }
        }

        [Test]
        public void GivenANewGameManager_WhenContentIsLoaded_ThenTheInitialGameStateIsSet()
        {
            Assert.That(gameManager.Alive, Is.True);
            Assert.That(gameManager.IsRunning, Is.True);
            Assert.That(gameManager.GameTime, Is.EqualTo(0));
            Assert.That(gameManager.FlagsPlaced, Is.EqualTo(0));
        }

        [Test]
        public void GivenANewGameManager_WhenContentIsLoaded_ThenTheTableMatchesTheConfiguredSize()
            => Assert.That(gameManager.GetAllTiles(), Has.Count.EqualTo(GameDefines.TableSize * GameDefines.TableSize));

        [Test]
        public void GivenANewGameManager_WhenContentIsLoaded_ThenTheMinesCountMatchesTheConfiguration()
        {
            Assert.That(gameManager.MinesCount, Is.EqualTo(GameDefines.MinesCount));
            Assert.That(CountMinedTiles(gameManager), Is.EqualTo(GameDefines.MinesCount));
        }

        [Test]
        public void GivenANewGameManager_WhenContentIsLoaded_ThenTheFlagsLimitMatchesTheMinesCount()
            => Assert.That(gameManager.FlagsLimit, Is.EqualTo(gameManager.MinesCount));

        [TestCase(0, 0)]
        [TestCase(7, 9)]
        [TestCase(15, 15)]
        public void GivenAStartedGame_WhenGettingAnyTileByCoordinates_ThenThatTileIsReturned(int x, int y)
        {
            Tile tile = gameManager.GetTile(x, y);

            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.X, Is.EqualTo(x));
            Assert.That(tile.Y, Is.EqualTo(y));
        }

        [Test]
        public void GivenAStartedGame_WhenClearingANonMinedTile_ThenTheTileBecomesCleared()
        {
            Tile safeTile = gameManager.GetAllTiles().First(tile => !tile.Mined && tile.DangerLevel > 0);

            gameManager.ClearTile(safeTile.X, safeTile.Y);

            Assert.That(gameManager.GetTile(safeTile.X, safeTile.Y).Cleared, Is.True);
        }

        [Test]
        public void GivenAStartedGame_WhenClearingAMinedTile_ThenThePlayerDiesAndTheGameStops()
        {
            Tile minedTile = gameManager.GetAllTiles().First(tile => tile.Mined);

            gameManager.ClearTile(minedTile.X, minedTile.Y);

            Assert.That(gameManager.Alive, Is.False);
            Assert.That(gameManager.IsRunning, Is.False);
            Assert.That(gameManager.Completed, Is.False);
        }

        [Test]
        public void GivenAClearedTile_WhenClearingItAgain_ThenNothingHappens()
        {
            Tile safeTile = gameManager.GetAllTiles().First(tile => !tile.Mined && tile.DangerLevel > 0);
            gameManager.ClearTile(safeTile.X, safeTile.Y);

            gameManager.ClearTile(safeTile.X, safeTile.Y);

            Assert.That(gameManager.GetTile(safeTile.X, safeTile.Y).Cleared, Is.True);
            Assert.That(gameManager.Alive, Is.True);
        }

        [Test]
        public void GivenAFlaggedTile_WhenClearingIt_ThenTheTileRemainsUncleared()
        {
            Tile safeTile = gameManager.GetAllTiles().First(tile => !tile.Mined);
            gameManager.FlagTile(safeTile.X, safeTile.Y);

            gameManager.ClearTile(safeTile.X, safeTile.Y);

            Assert.That(gameManager.GetTile(safeTile.X, safeTile.Y).Cleared, Is.False);
        }

        [TestCase(0, 0)]
        [TestCase(8, 8)]
        public void GivenAStartedGame_WhenFlaggingATile_ThenTheTileIsFlaggedAndTheFlagCountIsTracked(int x, int y)
        {
            gameManager.FlagTile(x, y);

            Assert.That(gameManager.GetTile(x, y).Flagged, Is.True);
            Assert.That(gameManager.FlagsPlaced, Is.EqualTo(1));
            Assert.That(gameManager.FlagsRemaining, Is.EqualTo(gameManager.FlagsLimit - 1));
        }

        [TestCase(0, 0)]
        [TestCase(3, 12)]
        public void GivenAFlaggedTile_WhenFlaggingItAgain_ThenTheFlagIsRemoved(int x, int y)
        {
            gameManager.FlagTile(x, y);

            gameManager.FlagTile(x, y);

            Assert.That(gameManager.GetTile(x, y).Flagged, Is.False);
            Assert.That(gameManager.FlagsPlaced, Is.EqualTo(0));
        }

        [Test]
        public void GivenAClearedTile_WhenFlaggingIt_ThenTheTileRemainsUnflagged()
        {
            Tile safeTile = gameManager.GetAllTiles().First(tile => !tile.Mined && tile.DangerLevel > 0);
            gameManager.ClearTile(safeTile.X, safeTile.Y);

            gameManager.FlagTile(safeTile.X, safeTile.Y);

            Assert.That(gameManager.GetTile(safeTile.X, safeTile.Y).Flagged, Is.False);
        }

        [Test]
        public void GivenTheFlagsLimitHasBeenReached_WhenFlaggingAnotherTile_ThenTheTileRemainsUnflagged()
        {
            foreach (Tile tile in gameManager.GetAllTiles())
            {
                gameManager.FlagTile(tile.X, tile.Y);
            }

            Tile unflaggedTile = gameManager.GetAllTiles().First(tile => !tile.Flagged);

            gameManager.FlagTile(unflaggedTile.X, unflaggedTile.Y);

            Assert.That(gameManager.GetTile(unflaggedTile.X, unflaggedTile.Y).Flagged, Is.False);
        }

        [Test]
        public void GivenARunningGame_WhenAnAccumulatedSecondElapses_ThenTheGameTimeIncrements()
        {
            gameManager.Update(1000);

            Assert.That(gameManager.GameTime, Is.EqualTo(1));
        }

        [TestCase(250, 4)]
        [TestCase(500, 2)]
        [TestCase(999, 1)]
        public void GivenARunningGame_WhenAccumulatingMultipleSecondFractions_ThenTheGameTimeIncrementsAccordingly(
            int elapsedMilliseconds,
            int updateCount)
        {
            for (int update = 0; update < updateCount; update++)
            {
                gameManager.Update(elapsedMilliseconds);
            }

            Assert.That(gameManager.GameTime, Is.EqualTo(elapsedMilliseconds * updateCount / 1000));
        }

        [Test]
        public void GivenACollectedSecond_WhenFurtherMillisecondFractionsElapse_ThenTheyAccumulateTowardsTheNextSecond()
        {
            gameManager.Update(500);
            gameManager.Update(600);

            Assert.That(gameManager.GameTime, Is.EqualTo(1));
        }

        [Test]
        public void GivenAStoppedGame_WhenUpdating_ThenTheGameTimeRemainsUnchanged()
        {
            Tile minedTile = gameManager.GetAllTiles().First(tile => tile.Mined);
            gameManager.ClearTile(minedTile.X, minedTile.Y);

            gameManager.Update(5000);

            Assert.That(gameManager.GameTime, Is.EqualTo(0));
        }

        [Test]
        public void GivenAStartedGame_WhenStartingANewGame_ThenTheStateIsResetAndNewMinesAreGenerated()
        {
            Tile minedTile = gameManager.GetAllTiles().First(tile => tile.Mined);
            gameManager.ClearTile(minedTile.X, minedTile.Y);

            gameManager.NewGame();

            Assert.That(gameManager.Alive, Is.True);
            Assert.That(gameManager.IsRunning, Is.True);
            Assert.That(gameManager.GameTime, Is.EqualTo(0));
            Assert.That(gameManager.FlagsPlaced, Is.EqualTo(0));
            Assert.That(CountMinedTiles(gameManager), Is.EqualTo(GameDefines.MinesCount));
        }

        [Test]
        public void GivenAStartedGame_WhenStartingANewGame_ThenAllTilesAreUnclearedAndUnflagged()
        {
            Tile safeTile = gameManager.GetAllTiles().First(tile => !tile.Mined);
            gameManager.ClearTile(safeTile.X, safeTile.Y);
            gameManager.FlagTile(safeTile.X, safeTile.Y);

            gameManager.NewGame();

            Assert.That(
                gameManager.GetAllTiles().Any(tile => tile.Cleared || tile.Flagged),
                Is.False);
        }
    }
}
