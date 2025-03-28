using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetByUserName(string login);

        User GetById(int id);

        List<User> GetAll();

        List<User> GetAllNotClients();

        User Add(User user);

        User Update(EditUserViewModel user);

        User UpdatePassword(User user);

        User SearchByLogin(string login, int? id);

        User SearchByEmail(string email, int? id);

        bool CheckUserByLogin(string login, int? id);

        User ChangePassword(PasswordRedefinitionViewModel passwordRedefinition);
        User GetUserLoged();



    }
}
