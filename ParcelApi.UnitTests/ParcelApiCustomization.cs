using AutoFixture;

using AutoMapper;

using Moq;

using ParcelApi.Data.Abstraction;


namespace ParcelApi.UnitTests;

public class ParcelApiCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Freeze<Mock<IParcelDbContext>>();
        fixture.Freeze<Mock<IMapper>>();
    }
}