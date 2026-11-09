namespace LittleTalk.Model.Common;

public static class DevCode
{
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 100;
    public static readonly int[] PageSizeOptions = { 10, 20, 50, 100 };

    /// <summary>
    /// Normalizes and bounds the requested page size between 1 and MaximumPageSize (100).
    /// </summary>
    public static int NormalizePageSize(this int pageSize)
    {
        if (pageSize <= 0)
        {
            return DefaultPageSize;
        }

        return pageSize > MaximumPageSize ? MaximumPageSize : pageSize;
    }

    /// <summary>
    /// Normalizes and bounds the requested page number so it is always at least 1.
    /// </summary>
    public static int NormalizePageNumber(this int pageNumber)
    {
        return pageNumber < 1 ? 1 : pageNumber;
    }

    /// <summary>
    /// Validates whether the given page size is positive and does not exceed MaximumPageSize.
    /// </summary>
    public static bool IsValidPageSize(this int pageSize)
    {
        return pageSize > 0 && pageSize <= MaximumPageSize;
    }

    /// <summary>
    /// Validates whether the given page number is positive.
    /// </summary>
    public static bool IsValidPageNumber(this int pageNumber)
    {
        return pageNumber >= 1;
    }
}
