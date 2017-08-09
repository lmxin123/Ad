using Framework.Weixin.Response;
using System.Threading.Tasks;

namespace Framework.Weixin.Command
{
    public interface ICommand
    {
        WeiXinContext WeiXin { get; }
        bool CanExecute { get; }
        Task<WeiXinResponse> ExecuteAsync();
    }
}
