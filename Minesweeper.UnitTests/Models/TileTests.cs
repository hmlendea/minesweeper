using Minesweeper.Models;

using NUnit.Framework;

namespace Minesweeper.UnitTests.Models
{
    [TestFixture]
    public class TileTests
    {
        private static Tile BuildTile(int x, int y)
            => new Tile { X = x, Y = y };

        [TestCase(0, 0, "0,0")]
        [TestCase(3, 7, "3,7")]
        [TestCase(15, 2, "15,2")]
        public void GivenACoordinatePair_WhenConstructingATile_ThenTheIdReflectsTheCoordinates(int x, int y, string expectedId)
        {
            Tile tile = BuildTile(x, y);

            Assert.That(tile.Id, Is.EqualTo(expectedId));
        }

        [Test]
        public void GivenAFreshlyConstructedTile_WhenInspectingItsState_ThenAllFieldsHaveDefaultValues()
        {
            Tile tile = new Tile();

            Assert.That(tile.X, Is.EqualTo(0));
            Assert.That(tile.Y, Is.EqualTo(0));
            Assert.That(tile.DangerLevel, Is.EqualTo(0));
            Assert.That(tile.Flagged, Is.False);
            Assert.That(tile.Cleared, Is.False);
            Assert.That(tile.Mined, Is.False);
        }

        [Test]
        public void GivenTwoTilesWithDifferentCoordinates_WhenComparingThem_ThenTheyAreNotEqual()
        {
            Tile firstTile = BuildTile(0, 0);
            Tile secondTile = BuildTile(1, 1);

            Assert.That(firstTile.Equals(secondTile), Is.False);
        }

        [Test]
        public void GivenTwoTilesWithTheSameCoordinates_WhenComparingThem_ThenTheyAreEqual()
        {
            Tile firstTile = BuildTile(4, 5);
            Tile secondTile = BuildTile(4, 5);

            Assert.That(firstTile.Equals(secondTile), Is.True);
            Assert.That(firstTile.GetHashCode(), Is.EqualTo(secondTile.GetHashCode()));
        }

        [Test]
        public void GivenATileAndANullObject_WhenComparingThem_ThenTheyAreNotEqual()
        {
            Tile tile = BuildTile(0, 0);

            Assert.That(tile.Equals(null as ModelBase), Is.False);
        }

        [Test]
        public void GivenTwoTilesWithTheSameCoordinates_WhenMutatingOne_ThenTheOtherIsUnaffected()
        {
            Tile firstTile = BuildTile(2, 2);
            Tile secondTile = BuildTile(2, 2);

            firstTile.Mined = true;
            firstTile.Cleared = true;

            Assert.That(secondTile.Mined, Is.False);
            Assert.That(secondTile.Cleared, Is.False);
        }
    }
}
