using Xunit;

// چون موجودیت Manager از الگوی Singleton پیروی می کند و بین تست ها به اشتراک گذاشته می شود،
// اجرای موازی تست ها می تواند باعث تداخل وضعیت شود؛ بنابراین اجرای موازی غیرفعال می شود.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
