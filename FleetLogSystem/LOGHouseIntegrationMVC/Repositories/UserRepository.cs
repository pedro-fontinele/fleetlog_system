
using LOGHouseSystem.Infra.Helpers;
﻿using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using NLog;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {

        public User GetByUserName(string login)
        {
            return _db.Users.FirstOrDefault(x => x.Username.ToUpper() == login.ToUpper());
        }


        public User GetById(int id)
        {
            return _db.Users.FirstOrDefault(x => x.Id == id);
        }


        public List<User> GetAll()
        {
            return _db.Users.ToList();
        }

        public List<User> GetAllNotClients()
        {
            return (from u in _db.Users
                    join c in _db.Clients
                    on u.Id equals c.UserId into clientGroup
                    from cg in clientGroup.DefaultIfEmpty()
                    where cg == null
                    select u).ToList();
        }

        public User Add(User user)
        {
            _db.Users.Add(user);
            user.SetPasswordHash();
            user.FirstAcess = Infra.Enums.YesOrNo.Yes;
            _db.SaveChanges();
            return user;
        }

        public User Update(EditUserViewModel user)
        {
            User userById = GetById(user.Id);
            if(userById == null)
                throw new System.Exception("Houve um erro na atualização do usuário");

            userById.Name = user.Name;
            userById.Username= user.Username;
            userById.Email= user.Email;
            userById.PermissionLevel = user.PermissionLevel;
            userById.IsActive = user.IsActive;
            userById.FirstAcess = user.FirstAcess;

            if (user.Password != null)
                userById.Password = user.Password.Hash();

            userById.Password = userById.Password;
            

            _db.Users.Update(userById);
            _db.SaveChanges();
            return userById;
        }

        public User UpdatePassword(User user)
        {
            User userById = GetById(user.Id);
            if (userById == null)
                throw new System.Exception("Houve um erro na atualização do usuário");

            userById.Password = user.Password;

            _db.Users.Update(userById);
            _db.SaveChanges();
            return userById;
        }

        public User SearchByLogin(string login, int? id)
        {
            var query =  _db.Users
                         .Where(x => x.Username.ToLower() == login.ToLower());
            
            if (id > 0)
            {
                //Traz se o id for diferente do dele
                query = query.Where(x => x.Id != id);
            }

            return query.FirstOrDefault();
        }

        public User SearchByEmail(string email, int? id)
        {
            var query = _db.Users
                           .Where(x => x.Email.ToLower() == email.ToLower());
                
            if(id > 0)
            {
                query = query.Where(x => x.Id != id);
            }

            return query.FirstOrDefault();
        }

        public bool CheckUserByLogin(string login, int? id)
        {

            User userByEmail = SearchByEmail(login, id);

            if (userByEmail != null)
            {
                return false;
            }

            User userByLogin = SearchByLogin(login, id);

            if (userByLogin != null)
            {
                return false;
            }

            return true;
        }

        public User ChangePassword(PasswordRedefinitionViewModel passwordRedefinition)
        {
            User userById = GetById(passwordRedefinition.Id);

            if (userById == null) throw new Exception("Houve um erro na atualização da senha, usuário não encontrado");

            userById.SetNewPasswordHash(passwordRedefinition.Password);
            userById.FirstAcess = Infra.Enums.YesOrNo.No;

            _db.Update(userById);
            _db.SaveChanges();

            return userById;

        }

        public User GetUserLoged()
        {
            return _session.SearchUserSession();
        }

       

    }
}
