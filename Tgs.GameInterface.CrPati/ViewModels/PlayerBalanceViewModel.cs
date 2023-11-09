using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.CrPati.ViewModels;

public class PlayerBalanceViewModel
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
    /// The member account
    /// </summary>
    [Required]
    public string PlayerCode { get; init; } = default!;

    /// <summary>
    /// The ip address
    /// </summary>
    [Required]
    public string IpAddress { get; init; } = default!;
}
