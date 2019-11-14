using Net.Web.Api.Sdk.Injection.Attributes;

namespace Net.Web.Api.Sdk.Interfaces.Information
{
    /// <summary>
    /// Interface IInformationService
    /// </summary>
    [InjectInterfaceService]
    public interface IInformationService
    {
        /// <summary>
        /// Gets the SDK informations.
        /// </summary>
        /// <returns>dynamic.</returns>
        dynamic GetSdkInformations();
    }
}
