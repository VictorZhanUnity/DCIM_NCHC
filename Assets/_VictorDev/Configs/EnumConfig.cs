/// 各類Enum的設定
namespace VictorDev.Configs
{
    #region XYZ座標
    /// X軸 - 靠左、置中、靠右
    public enum EnumAlignmentPivotX
    {
        Left,
        Center,
        Right
    }
    
    /// Y軸 - 靠上、置中、靠下
    public enum EnumAlignmentPivotY
    {
        Top,
        Center,
        Bottom
    }

    /// Z軸 - 靠前、置中、靠後
    public enum EnumAlignmentPivotZ
    {
        Front,
        Center,
        Back
    }
    
    ///  選擇語言 (中文或英文)
    public enum EnumLanguage
    {
        en_US,
        zh_CN
    }
    #endregion

    #region Net
   
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
    #endregion
    
    #region 時間
    public enum EnumTimeFormat
    {
        時分秒_12小時制, 時分秒_24小時制, 西元年月日, 星期, 星期_縮寫, 完整年月日時分秒_12小時制, 完整年月日時分秒_24小時制
    }
    
    public enum EnumTime
    {
        時, 分, 秒
    }
    #endregion
}