using MongoDB.Driver;

namespace Shared.ValueObjects;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; private set; } = [];
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public long TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    private PaginatedList() { }

    public PaginatedList(List<T> items, long count, int pageNumber, int pageSize)
    {
        Items = items.AsReadOnly();
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    /// <summary>
    /// Cria uma lista paginada a partir de um IMongoCollection com filtro.
    /// </summary>
    public static async Task<PaginatedList<T>> CreateAsync(
        IMongoCollection<T> collection,
        FilterDefinition<T> filter,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var count = await collection.CountDocumentsAsync(
            filter,
            cancellationToken: cancellationToken
        );

        var items = await collection
            .Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Cria uma lista paginada a partir de um IFindFluent já construído.
    /// </summary>
    public static async Task<PaginatedList<T>> CreateAsync(
        IFindFluent<T, T> findFluent,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var count = await findFluent.CountDocumentsAsync(cancellationToken);
        var items = await findFluent
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
