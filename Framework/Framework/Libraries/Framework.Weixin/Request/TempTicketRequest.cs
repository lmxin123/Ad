using Newtonsoft.Json;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    /// <summary>
    /// 临时二维码ticket生成请求
    /// </summary>
    public class TempTicketRequest : RequestObject<TempTicketResponse>
    {
        private readonly int _scene_id;

        public TempTicketRequest(string access_Token, int scene_Id)
        {
            _scene_id = scene_Id;
            access_token = access_Token;
        }

        public override string RequestUrl
        {
            get
            {
                return string.Format("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}", access_token);
            }
        }
        /// <summary>
        /// 该二维码有效时间，以秒为单位。 最大不超过2592000（即30天），
        /// 此字段如果不填，则默认有效期为30秒。
        /// </summary>
        public string access_token { get; set; }

        public int expire_seconds
        {
            get { return 2592000; }
        }

        public string action_name
        {
            get
            {
                return "QR_SCENE";
            }
        }

        public ActionInfo action_info
        {
            get
            {
                return new ActionInfo
                {
                    scene = new Scene
                    {
                        scene_id = _scene_id
                    }
                };
            }
        }

        [JsonIgnore]
        public string TempTicketCacheKey
        {
            get
            {
                return string.Format("TempTicketRequest_{0}", action_info.scene.scene_id);
            }
        }
    }

    /// <summary>
    /// 二维码详细信息
    /// </summary>
    public class ActionInfo
    {
        public Scene scene { get; set; }
    }

    public class Scene
    {
        /// <summary>
        /// 场景值ID，临时二维码时为32位非0整型
        /// </summary>
        public int scene_id { get; set; }
    }
}
