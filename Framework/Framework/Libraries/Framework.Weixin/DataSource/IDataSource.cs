using Framework.Weixin.Response;

namespace Framework.Weixin.DataSource
{
    public interface IDataSource
    {
        WeiXinResponse GetResponse();
    }
}
