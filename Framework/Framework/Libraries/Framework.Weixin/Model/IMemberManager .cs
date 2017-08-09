using System.Threading.Tasks;

namespace Framework.Weixin.Model
{
    public interface IMemberManager<TMember> where TMember : MemberModel, new()
    {
        Task<TMember> GetMemberByIdAsync(string id);
        Task<int> ModifyMemberAsync(TMember model);
        int CreateMemberAsync(TMember model, int inviteCode);
    }
}
