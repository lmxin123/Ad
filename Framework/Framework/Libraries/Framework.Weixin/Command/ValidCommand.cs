using System.Threading.Tasks;
using System.Collections.Generic;
using Framework.Weixin.Request;
using Framework.Weixin.Response;
using Framework.Common.Extensions;

namespace Framework.Weixin.Command
{
    public class ValidCommand : CommandBase
    {
        private readonly WeixinSettings _weixinSettings;

        public ValidCommand(WeiXinContext weiXin, WeixinSettings weixinSettings) : base(weiXin) {
            _weixinSettings = weixinSettings;
        }

        public async override Task<WeiXinResponse> ExecuteAsync()
        {
            ValidResponse response = new ValidResponse(WeiXin);
            await Task.Run(() =>
            {
                ValidRequest request = new ValidRequest(WeiXin);
                List<string> list = new List<string>() { _weixinSettings.ValidToken, request.Timestamp, request.Nonce };
                list.Sort();

                string validCode = string.Join("", list.ToArray()).SHA1Encrypt();
                if (validCode == request.Signature)
                {
                    response.EchoStr = request.EchoStr;
                }
            });
            return response;
        }

        public override bool CanExecute
        {
            get
            {
                ValidRequest request = WeiXin.WeiXinRequest as ValidRequest;
                return request != null && !string.IsNullOrEmpty(request.EchoStr);
            }
        }
    }
}