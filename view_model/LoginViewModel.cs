using gizindir.data;
using gizindir.model;
using gizindir.state;

namespace gizindir.viewmodel
{
    public class LoginViewModel
    {
        private UserRepository _userRepo;

        public LoginViewModel()
        {
            _userRepo = new UserRepository();
        }

        public bool Login(string email, string password)
        {
            var user = _userRepo.GetUserByEmail(email);
            if (user != null && user.Password == password)
            {
                AppState.CurrentUser = user;
                return true;
            }
            return false;
        }
    }
}
