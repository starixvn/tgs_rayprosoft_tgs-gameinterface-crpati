using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.CrPati.ViewModels;

public class GameWagerViewModel
{
    /// <summary>
    /// The API URL
    /// </summary>
    [Required]
    public Uri ApiUrl { get; init; } = default!;

    /// <summary>
    /// The API key
    /// </summary>
    [Required]
    public string ApiKey { get; init; } = default!;

    /// <summary>
    /// The API secret
    /// </summary>
    [Required]
    public string ApiSecret { get; init; } = default!;

    /// <summary>
    /// The start time
    /// </summary>
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; }

    /// <summary>
    /// The end time
    /// </summary>
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; }

    /// <summary>
    /// The page number
    /// </summary>
    [Required]
    public int PageNumber { get; init; }

    /// <summary>
    /// The page size
    /// </summary>
    [Required]
    public int PageSize { get; init; }
}
