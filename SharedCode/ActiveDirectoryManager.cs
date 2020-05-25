using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace SharedCode
{
    class ActiveDirectoryManager
    {
        static public string GetUserFullname(SearchResult result) => result != null && result.GetDirectoryEntry().InvokeGet("displayName").ToString() != null ? 
            result.GetDirectoryEntry().InvokeGet("displayName").ToString() : "Sin resultados";

        static public string GetUserAlias(SearchResult result) => result != null && result.GetDirectoryEntry().InvokeGet("mail").ToString() != null ? 
            result.GetDirectoryEntry().InvokeGet("mail").ToString().Replace("@pemex.com", "") : "Sin resultados";

        static public string GetUserMail(SearchResult result) => result != null && result.GetDirectoryEntry().InvokeGet("mail").ToString() != null ? 
            result.GetDirectoryEntry().InvokeGet("mail").ToString() : "Sin resultados";

        static public object[] GetUserMemberOf(SearchResult result) => result != null && result.GetDirectoryEntry().InvokeGet("memberOf") != null ?
            (object[])result.GetDirectoryEntry().InvokeGet("memberOf") : null;

        static public SearchResult SearchInActiveDirectory(string username)
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://PEMEX");
            DirectorySearcher search = new DirectorySearcher(entry);
            
            search.Filter = $"(&(objectClass=user)(anr={username}))";

            SearchResult result = search.FindOne();

            // if (result != null)
            //     foreach (var item in result.GetDirectoryEntry().Properties.PropertyNames)
            //     {
            //         Console.WriteLine($"ACTIVE DIRECTORY RESULT: {item.ToString()},\t{result.GetDirectoryEntry().InvokeGet(item.ToString()).ToString()}");
            //     }
            // 
            // if (result.GetDirectoryEntry().InvokeGet("memberOf") != null)
            //     Console.WriteLine(result.GetDirectoryEntry().InvokeGet("memberOf").GetType());

            return result;
        }
    }
}

//https://stackoverflow.com/questions/785527/how-to-get-a-users-e-mail-address-from-active-directory#comment103715333_45469210