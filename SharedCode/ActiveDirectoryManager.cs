using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace SharedCode
{
    class ActiveDirectoryManager
    {
        static public string GetADUserFullname(string username)
        {
            // RESUELVE EL NOMBRE COMPLETO DEL USUARIO CON SU USERNAME DEL DOMINIO
            try
            {
                System.DirectoryServices.DirectoryEntry ADEntry = new System.DirectoryServices.DirectoryEntry($"WinNT://PEMEX/{username}");
                return ADEntry.Properties["FullName"].Value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        static public string GetADAlias(string username)
        {
            var x = new PrincipalContext(ContextType.Domain, "PEMEX.PMX.COM");
            var user = UserPrincipal.FindByIdentity(x, username);
            Console.WriteLine(user.EmailAddress);

            // RESUELVE EL ALIAS DEL USUARIO CON SU USERNAME DEL DOMINIO
            try
            {
                System.DirectoryServices.DirectoryEntry ADEntry = new System.DirectoryServices.DirectoryEntry($"WinNT://PEMEX/{username}");
                return ADEntry.Properties["netbiosname"][0].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        static public string GetDomainMail(string username) => $"{GetADAlias(username)}@pemex.com";
    }
}

//https://stackoverflow.com/questions/785527/how-to-get-a-users-e-mail-address-from-active-directory#comment103715333_45469210