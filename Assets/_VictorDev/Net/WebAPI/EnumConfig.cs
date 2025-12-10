
namespace _VictorDev.Net.WebAPI
{
    public enum EnumAuthorizationType
    {
        NoAuth,
        Bearer 
    }
    /// Https / Http
    public enum EnumHttpType
    {
        https,
        http
    };
    /// Get / Post 
    public enum EnumHttpMethod
    {
        Get,
        Head,
        Post,
        Put,
        Create,
        Delete
    }
    public enum EnumBody
    {
        None,
        FormData,
        RawJson,
        RawText,
        Binary
    }

    public enum EnumResponseDataType
    {
        Json,
        WWWForm,
        Text,
        Excel,
        PDF,
        Image,
        Word,
        ZIP,
        Binary,
    }
}