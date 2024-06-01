using KufarStatQueries;
using KufarStatApp.HelperServices;

namespace KufarStatApp.Tests
{
    [TestFixture]
    public class GetFlatsInsideShapeTests
    {
        [Test]
        public void ShouldReturnFlatsInsideShape_WhenAdsResponseAndShapePointsAreValid()
        {
            // Arrange
            FlatSearchResponse adsResponse = new FlatSearchResponse();
            adsResponse.Ads =
            [
                new Ad()
                {
                    AdId = 1,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[1.5,1.9]" }
                    ]
                },
                new Ad()
                {
                    AdId = 2,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[3,3]" }
                    ]
                },
                new Ad()
                {
                    AdId = 3,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[1.1,1.2]" }
                    ]
                },
                new Ad()
                {
                    AdId = 4,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[2.5,2.6]" }
                    ]
                },
                new Ad()
                {
                    AdId = 5,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[0.5,0.5]" }
                    ]
                }
            ];
            List<Point> shapePoints = new List<Point>()
            {
                new Point(1, 1),
                new Point(1, 3),
                new Point(3, 3)
            };

            // Act
            List<Ad> flatsInsideShape = Program.GetFlatsInsideShape(adsResponse, shapePoints);

            // Assert
            Assert.That(flatsInsideShape.Count == 3);
            Assert.That(flatsInsideShape.Select(f => f.AdId).Contains(1));
            Assert.That(flatsInsideShape.Select(f => f.AdId).Contains(3));
            Assert.That(flatsInsideShape.Select(f => f.AdId).Contains(4));
        }

        [Test]
        public void ShouldReturnEmptyList_WhenAdsResponseIsEmpty()
        {
            // Arrange
            FlatSearchResponse adsResponse = new FlatSearchResponse();
            List<Point> shapePoints = new List<Point>()
            {
                new Point(53.9, 27.5),
                new Point(53.9, 27.6),
                new Point(53.8, 27.6)
            };

            // Act
            List<Ad> flatsInsideShape = Program.GetFlatsInsideShape(adsResponse, shapePoints);

            // Assert
            Assert.That(flatsInsideShape.Count == 0);
        }

        [Test]
        public void ShouldReturnEmptyList_WhenShapePointsAreInvalid()
        {
            // Arrange
            FlatSearchResponse adsResponse = new FlatSearchResponse();
            adsResponse.Ads =
            [
                new Ad()
                {
                    AdId = 1,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[1.5,1.9]" }
                    ]
                },
                new Ad()
                {
                    AdId = 2,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[3,3]" }
                    ]
                },
                new Ad()
                {
                    AdId = 3,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[1.1,1.2]" }
                    ]
                },
                new Ad()
                {
                    AdId = 4,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[2.5,2.6]" }
                    ]
                },
                new Ad()
                {
                    AdId = 5,
                    AdParameters =
                    [
                        new AdParameter() { PropertyName = "coordinates", PropertyValue = "[0.5,0.5]" }
                    ]
                }
            ];
            List<Point> shapePoints = new List<Point>()
            {
                new Point(1, 1),
                new Point(1, 3),
                new Point(0, 2)
            };

            // Act
            List<Ad> flatsInsideShape = Program.GetFlatsInsideShape(adsResponse, shapePoints);

            // Assert
            Assert.That(flatsInsideShape.Count == 0);
        }
    }
}