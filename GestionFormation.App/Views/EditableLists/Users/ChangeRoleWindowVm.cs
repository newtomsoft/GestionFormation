using GestionFormation.App.Core;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.App.Views.EditableLists.Users
{
    public class ChangeRoleWindowVm : PopupWindowVm
    {
        private UserRole _role;

        public ChangeRoleWindowVm(UserRole role)
        {
            Role = role;
        }

        public UserRole Role
        {
            get => _role;
            set { Set(()=>Role, ref _role, value); }
        }

        public override string Title => "Changement du r�le";
    }
}