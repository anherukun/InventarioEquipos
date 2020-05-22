using System;
using System.Collections.Generic;
using System.DirectoryServices;
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
                DirectoryEntry ADEntry = new DirectoryEntry($"WinNT://PEMEX/{username}");
                return ADEntry.Properties["FullName"].Value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        static public string GetADAlias(string username)
        {
            // RESUELVE EL ALIAS DEL USUARIO CON SU USERNAME DEL DOMINIO
            try
            {
                DirectoryEntry ADEntry = new DirectoryEntry($"WinNT://PEMEX/{username}");
                return ADEntry.Properties["netbiosname"][0].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        static public string GetDomainMail(string username)
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://PEMEX");

            // get a DirectorySearcher object
            DirectorySearcher search = new DirectorySearcher(entry);

            // specify the search filter
            search.Filter = $"(&(objectClass=user)(anr={username}))";

            // perform the search
            SearchResult result = search.FindOne();

            return result.GetDirectoryEntry().InvokeGet("mail").ToString();
        }
    }
}

//https://stackoverflow.com/questions/785527/how-to-get-a-users-e-mail-address-from-active-directory#comment103715333_45469210