using System.Text.RegularExpressions;

namespace ApartmentManagement.Base;

public class PasswordGenerator
{
    public static string Get()
    {
        return string.Join("",ReferenceNumberGenerator.Get().ToCharArray()).Substring(0,8);
    }
}