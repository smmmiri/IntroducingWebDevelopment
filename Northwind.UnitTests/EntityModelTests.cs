namespace Northwind.EntityModels;

public class EntityModelTests
{
    [Fact]
    public void DatabaseConnectTest()
    {
        using NorthwindContext db = new();
        Assert.True(db.Database.CanConnect());
    }

    [Fact]
    public void CategoryCountTest()
    {
        using NorthwindContext db = new();

        var expected = 8;
        var actual = db.Categories.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProductId1IsChaiTest()
    {
        using NorthwindContext db = new();

        var expected = "Chai";
        var product = db.Products.Find(1);
        var actual = product?.ProductName ?? string.Empty;

        Assert.Equal(expected, actual);
    }
}
