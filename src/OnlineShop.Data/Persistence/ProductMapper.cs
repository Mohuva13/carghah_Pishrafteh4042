using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Enums;
using OnlineShop.Data.Dto;

namespace OnlineShop.Data.Persistence;

/// <summary>تبدیل دو طرفه بین موجودیت های سلسله مراتب کالا و <see cref="ProductDto"/> مسطح برای ذخیره در JSON.</summary>
public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        var dto = new ProductDto
        {
            Id = product.Id,
            TypeName = product.GetType().Name,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsDeleted = product.IsDeleted,
            Comments = product.Comments.Select(CommentMapper.ToDto).ToList(),
            Ratings = product.Ratings.Select(r => new RatingDto { BuyerUsername = r.BuyerUsername, ProductId = r.ProductId, Score = r.Score }).ToList()
        };

        switch (product)
        {
            case FlashMemory flash:
                dto.Weight = flash.Weight;
                dto.Dimensions = flash.Dimensions;
                dto.CapacityGb = flash.CapacityGb;
                dto.UsbVersion = flash.UsbVersion;
                break;
            case Ssd ssd:
                dto.Weight = ssd.Weight;
                dto.Dimensions = ssd.Dimensions;
                dto.CapacityGb = ssd.CapacityGb;
                dto.ReadSpeedMbps = ssd.ReadSpeedMbps;
                dto.WriteSpeedMbps = ssd.WriteSpeedMbps;
                break;
            case PersonalComputer pc:
                dto.Weight = pc.Weight;
                dto.Dimensions = pc.Dimensions;
                dto.CpuModel = pc.CpuModel;
                dto.RamCapacityGb = pc.RamCapacityGb;
                break;
            case Pencil pencil:
                dto.CountryOfOrigin = pencil.CountryOfOrigin;
                dto.PencilType = pencil.PencilType;
                break;
            case Pen pen:
                dto.CountryOfOrigin = pen.CountryOfOrigin;
                dto.Color = pen.Color;
                break;
            case Notebook notebook:
                dto.CountryOfOrigin = notebook.CountryOfOrigin;
                dto.PageCount = notebook.PageCount;
                dto.PaperType = notebook.PaperType;
                break;
            case Bicycle bicycle:
                dto.Manufacturer = bicycle.Manufacturer;
                dto.BicycleType = bicycle.BicycleType;
                break;
            case Car car:
                dto.Manufacturer = car.Manufacturer;
                dto.EngineVolume = car.EngineVolume;
                dto.IsAutomatic = car.IsAutomatic;
                break;
            case Food food:
                dto.ProductionDate = food.ProductionDate;
                dto.ExpirationDate = food.ExpirationDate;
                break;
            default:
                throw new NotSupportedException($"نوع کالای «{product.GetType().Name}» برای ذخیره سازی پشتیبانی نمی شود.");
        }

        return dto;
    }

    public static Product FromDto(ProductDto dto)
    {
        Product product = dto.TypeName switch
        {
            nameof(FlashMemory) => new FlashMemory(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.Weight ?? 0, dto.Dimensions ?? string.Empty, dto.CapacityGb ?? 0, dto.UsbVersion ?? string.Empty),

            nameof(Ssd) => new Ssd(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.Weight ?? 0, dto.Dimensions ?? string.Empty, dto.CapacityGb ?? 0,
                dto.ReadSpeedMbps ?? 0, dto.WriteSpeedMbps ?? 0),

            nameof(PersonalComputer) => new PersonalComputer(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.Weight ?? 0, dto.Dimensions ?? string.Empty, dto.CpuModel ?? string.Empty, dto.RamCapacityGb ?? 0),

            nameof(Pencil) => new Pencil(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.CountryOfOrigin ?? string.Empty, dto.PencilType ?? PencilType.HB),

            nameof(Pen) => new Pen(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.CountryOfOrigin ?? string.Empty, dto.Color ?? string.Empty),

            nameof(Notebook) => new Notebook(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.CountryOfOrigin ?? string.Empty, dto.PageCount ?? 0, dto.PaperType ?? string.Empty),

            nameof(Bicycle) => new Bicycle(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.Manufacturer ?? string.Empty, dto.BicycleType ?? OnlineShop.Core.Enums.BicycleType.Urban),

            nameof(Car) => new Car(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.Manufacturer ?? string.Empty, dto.EngineVolume ?? 0, dto.IsAutomatic ?? false),

            nameof(Food) => new Food(dto.Id, dto.Name, dto.Price, dto.StockQuantity,
                dto.ProductionDate ?? DateTime.Now, dto.ExpirationDate ?? DateTime.Now.AddDays(1)),

            _ => throw new NotSupportedException($"نوع کالای «{dto.TypeName}» در فایل داده قابل بازیابی نیست.")
        };

        product.IsDeleted = dto.IsDeleted;

        foreach (var commentDto in dto.Comments)
        {
            product.AddComment(CommentMapper.FromDto(commentDto));
        }

        foreach (var ratingDto in dto.Ratings)
        {
            product.AddOrUpdateRating(new Rating(ratingDto.BuyerUsername, ratingDto.ProductId, ratingDto.Score));
        }

        return product;
    }
}
